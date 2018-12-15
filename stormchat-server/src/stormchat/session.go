package main

import (
	"encoding/binary"
	"encoding/json"
	"net"
	"strconv"
	"strings"
	"sync/atomic"
	"time"
)

//当前session的状态
const (
	session_status_created   = iota //已创建，但未确认发送者和接收者身份
	session_status_running          // 通道已建立，session正常运行
	session_status_stoped           // 标识服务已停止
	session_status_destroyed        //消息读写线程已关闭，连接已断开
)

type Session struct {
	sender        *UserInfo    //消息发送者
	receiver      string       //消息接收者
	addr          string       //客户端地址
	conn          *net.TCPConn //TCP连接
	status        int32        //session运行状态
	stopChan      chan bool    //Session终止标识，传递数据即终止。仅由读消息循环用于终止写循环
	writeCh       chan []byte  //写消息通道
	writeResultCh chan bool    //是否写成功
}

//初始化
func NewSession(conn *net.TCPConn) *Session {
	var session = new(Session)
	session.sender = nil
	session.receiver = ""
	session.status = session_status_created
	session.stopChan = make(chan bool)
	session.writeCh = make(chan []byte)
	session.writeResultCh = make(chan bool)
	session.conn = conn
	session.addr = conn.RemoteAddr().String()
	return session
}

//销毁聊天连接
func (session *Session) Destroy() {
	if session.status == session_status_destroyed {
		return
	}
	if session.sender != nil {
		delete(server.sessionMap, session.sender.User)
	}
	session.status = session_status_destroyed
	session.conn.Close()
	close(session.writeCh)
	close(session.writeResultCh)
}

//消息处理总控制器，根据控制字调用相应处理函数
//关闭连接时先退出读循环，再关闭写循环，避免造成chan panic
func (session *Session) ReceiveLoop() {
	defer func() {
		session.stopChan <- true
		Debug("[" + session.addr + "]Receive loop ended.")
	}() //终止写循环
	Debug("[" + session.addr + "]Receive loop started.")
	var err error //错误
	for {
		if atomic.LoadInt32(&session.status) == session_status_stoped {
			return
		} //检查session是否已终止
		//设置超时并等待客户数据
		if !session.waitData(time.Now().Add(timeout_message * time.Second)) {
			continue
		}
		//读取数据
		head := session.readDataWithLength(max_head_length)
		data := session.readDataWithLength(max_message_length)
		if head == nil || data == nil {
			Debug("Failed to read data.")
			session.SendPanic("", "Failed to read data.")
			continue
		}
		//解析包头
		headInfo := make(map[string]string)
		err = json.Unmarshal(head, &headInfo)
		if err != nil {
			Debug("Failed to unmarshal head.")
			session.SendPanic("", "Failed to unmarshal head.")
			continue
		}
		if session.sender == nil && headInfo[keyname_operation] != operation_login {
			session.SendPanic("", "MUST LOGIN")
			continue
		} //抛弃未登录用户的消息
		switch headInfo[keyname_operation] {
		case operation_send_message:
			go session.MessageHandler(headInfo, data)
			break
		case operation_ping:
			go session.PingHandler()
			break
		case operation_login:
			session.LoginHandler(headInfo)
			break
		case operation_logout:
			session.LogoutHandler()
			break
		case operation_get_user_list:
			go session.GetUserListHandler(headInfo)
			break
		case operation_update_userinfo:
			go session.UpdateUserInfoHandler(headInfo, data)
			break
		default:
			session.SendPanic(headInfo[keyname_operation], "Unknwon Request")
			continue
		}
	}
}

//消息写循环(由读循环终止)
func (session *Session) SendLoop() {
	Debug("[" + session.addr + "]Send loop started.")
	defer session.Destroy()
	for {
		select {
		case data, ok := <-session.writeCh:
			if !ok {
				atomic.SwapInt32(&session.status, session_status_stoped)
				continue
			}
			writeLen, err := session.conn.Write(data)
			if err != nil || (writeLen != len(data)) {
				WriteLog(err.Error())
				atomic.SwapInt32(&session.status, session_status_stoped)
				session.writeResultCh <- false
				continue
			} //写失败
			session.writeResultCh <- true
		case <-session.stopChan:
			Debug("[" + session.addr + "]Send loop ended.")
			return
		}
	}
}

//响应客户端心跳包
func (session *Session) PingHandler() {
	return
}

//登录事件处理函数
func (session *Session) LoginHandler(headInfo map[string]string) {
	returnInfo := make(map[string]string)
	returnInfo[keyname_operation] = headInfo[keyname_operation]
	returnInfo[keyname_token] = headInfo[keyname_token]
	returnInfo["Error"] = ""
	if session.status != session_status_created {
		session.SendData(returnInfo, nil)
		return
	} //已登录
	user := Login(headInfo["User"], headInfo["Pwd"])
	if user == nil {
		Debug("Bad login: illegal user.")
		returnInfo["Error"] = "Illegal User."
		session.SendData(returnInfo, nil)
		return
	}
	session.sender = user
	session.status = session_status_running
	session.Offline(server.sessionMap[session.sender.User], "The account is logged by another client.")
	server.sessionMap[session.sender.User] = session
	userdata, _ := json.Marshal(user)
	session.SendData(returnInfo, userdata)
	//转发用户未读消息
	go session.TransUnreadedMessages(session.sender)
	Debug("Log in successfully.")
}

//用户登出
func (session *Session) LogoutHandler() {
	//防止其他session传递数据。等待用户断开连接或者再次登录
	delete(server.sessionMap, session.sender.User)
	session.sender = nil
	session.receiver = ""
	if atomic.LoadInt32(&session.status) == session_status_running {
		atomic.SwapInt32(&session.status, session_status_created)
	} //恢复未登录状态
}

//消息处理函数
func (session *Session) MessageHandler(headInfo map[string]string, msg []byte) {
	result := make(map[string]string)
	result[keyname_token] = headInfo[keyname_token]
	result[keyname_operation] = operation_send_message
	result["Error"] = ""

	if headInfo["To"] == "" {
		headInfo["To"] = session.receiver
	}
	var destChat *Session = nil
	for key, value := range server.sessionMap {
		if key == headInfo["To"] {
			destChat = value
			break
		}
	} //判断接收者是否在线
	message := new(Message)
	message.When = time.Now()
	message.From = session.sender
	message.Msg = msg
	if destChat != nil {
		message.To = destChat.sender
		destChat.TransMessage(message)
	} else {
		message.To = QueryUserInfo(headInfo["To"])
		if message.To != nil {
			message.Save()
		} else {
			result["Error"] = "The message doesn't has a valid receiver."
		} //消息接收者不存在
	}
	if needResult, ok := headInfo["NeedResult"]; ok && needResult != "0" {
		session.SendData(result, nil)
	} //返回执行结果
}

//获取用户列表
func (session *Session) GetUserListHandler(headInfo map[string]string) bool {
	head := make(map[string]string)
	head[keyname_token] = headInfo[keyname_token]
	head[keyname_operation] = headInfo[keyname_operation]
	head["Count"] = "0"
	head["Error"] = ""
	users := GetUserList()
	if users == nil {
		head["Error"] = "Server failed to get user list."
		session.SendData(head, nil)
		return false
	}
	data, err := json.Marshal(users)
	if err != nil {
		head["Error"] = "JSON marshal failed: " + err.Error()
		WriteLog(head["Error"])
		session.SendData(head, nil)
		return false
	}
	head["Count"] = string(len(users))
	return session.SendData(head, data)
}

//获取用户头像
func (session *Session) GetUserPhotoHandler(headInfo map[string]string) {
	head := make(map[string]string)
	head[keyname_token] = headInfo[keyname_token]
	head[keyname_operation] = headInfo[keyname_operation]
	head["Error"] = ""
	photo := GetUserPhoto(session.sender.User)
	session.SendData(head, photo)
}

//更改用户信息
func (session *Session) UpdateUserInfoHandler(headInfo map[string]string, data []byte) {
	//准备反馈数据
	result := make(map[string]string)
	result[keyname_token] = headInfo[keyname_token]
	result[keyname_operation] = headInfo[operation_update_userinfo]
	result["Error"] = ""

	if item, ok := headInfo["NickName"]; ok {
		result["Error"] += session.sender.UpdateNickName(item) + ";"
	} //修改昵称
	if item, ok := headInfo["Password"]; ok {
		result["Error"] += session.sender.UpdatePassword(item) + ";"
	} //修改密码
	if item, ok := headInfo["Motto"]; ok {
		result["Error"] += session.sender.UpdateMotto(item) + ";"
	} //修改签名
	item, ok := headInfo["Photo"]
	i, _ := strconv.Atoi(item)
	if ok && i > 0 {
		result["Error"] += session.sender.UpdatePhoto(data)
	} //修改头像
	session.SendData(result, nil)
}

/**
 * 服务器数据异常，向客户端发送Panic并关闭连接
 * @param curOperation, 当前操作
 * @param errStr, 错误描述
 */
func (session *Session) SendPanic(curOperation string, errStr string) bool {
	if atomic.LoadInt32(&session.status) >= session_status_stoped {
		return false
	}
	Debug("[SendPanic]" + errStr)
	defer atomic.SwapInt32(&session.status, session_status_stoped)
	data := make(map[string]string)
	data[keyname_token] = ""
	data[keyname_operation] = operation_panic
	data["Job"] = curOperation
	data["Error"] = errStr
	slice, err := json.Marshal(data)
	if err != nil {
		return false
	}
	return session.send(slice, nil)
}

//发送数据到客户端，包头将被json序列化
func (session *Session) SendData(headInfo map[string]string, data []byte) bool {
	head, err := json.Marshal(headInfo)
	if err != nil {
		Debug("TransMessage Failed.")
		session.SendPanic(headInfo[keyname_operation], "Failed to marshal data.")
		return false
	}
	return session.send(head, data)
}

//勒令用户下线。调用条件（满足一条）：
//相同账户被重复登录
//用户修改密码
func (newChat *Session) Offline(oldChat *Session, reason string) bool {
	if oldChat == nil {
		return true
	}
	data := make(map[string]string)
	data[keyname_token] = ""
	data[keyname_operation] = operation_offline
	data["Error"] = "Another Login"
	data["Addr"] = newChat.addr
	defer atomic.SwapInt32(&oldChat.status, session_status_stoped)
	return oldChat.SendData(data, nil)
}

//转发消息
func (session *Session) TransMessage(msg *Message) bool {
	data := make(map[string]string)
	data[keyname_token] = ""
	data[keyname_operation] = operation_trans_message
	data["When"] = msg.When.Format(time.RFC3339)
	data["From"] = msg.From.User
	return session.SendData(data, []byte(msg.Msg))
}

//转发存储在数据库中的未读消息
func (session *Session) TransUnreadedMessages(user *UserInfo) bool {
	messages := QuerySavedMessages(user)
	if messages == nil {
		return false
	}
	for id, message := range messages {
		if session.TransMessage(&message) {
			DeleteSavedMessage(id)
		} else {
			return false
		}
	}
	Debug("Unreaded message transmitting done.")
	return true
}

//从TCP连接读取数据，取到的前2字节作为数据长度
func (session *Session) readDataWithLength(max_length uint32) []byte {
	var dataLen uint32    //数据长度
	var lenBuffer [4]byte //数据长度缓存区
	var data []byte       //数据缓存区
	//获取数据长度
	readedLen, err := session.conn.Read(lenBuffer[0:4])
	if err != nil || readedLen != 4 {
		Debug("Failed to read data. Error: " + err.Error())
		return nil
	}
	dataLen = binary.BigEndian.Uint32(lenBuffer[0:4])
	if dataLen > max_length || dataLen < 0 {
		WriteLog("[ReadData]Data length exceeds.")
		return nil
	} //数据长度超出限制
	data = make([]byte, dataLen)
	if dataLen == 0 {
		return data
	}
	readedLen, err = session.conn.Read(data)
	if err != nil || readedLen != int(dataLen) {
		Debug("[ReadData]Failed to read data." + err.Error())
		return nil
	}
	return data
}

//等待客户端数据
func (session *Session) waitData(deadline time.Time) bool {
	var buf = make([]byte, 0, 0) //空缓冲区
	for time.Now().Before(deadline) {
		session.conn.SetReadDeadline(time.Now().Add(time.Second))
		_, err := session.conn.Read(buf)
		if err == nil {
			//消息到达时临时禁止读取超时
			session.conn.SetReadDeadline(time.Now().Add(time.Hour * 24 * 360 * 100))
			return true
		} //数据到达
		if atomic.LoadInt32(&session.status) >= session_status_stoped {
			return false
		}
		if strings.HasSuffix(err.Error(), "i/o timeout") {
			continue
		} //等待数据时正常超时
		atomic.SwapInt32(&session.status, session_status_stoped)
		return false
	}
	return false
}

/**
* 将数据加工后发送给客户端
* 	@param head, 消息头
* 	@param data, 消息体
 */
func (session *Session) send(head []byte, data []byte) bool {
	if atomic.LoadInt32(&session.status) >= session_status_stoped {
		return false
	}
	var buf = make([]byte, 4)                             //长度标识
	var packet = make([]byte, 0, len(head)+len(data)+4+4) //headLen + head + msgLen + data
	//写包头
	//取包头长
	binary.BigEndian.PutUint32(buf[0:4], uint32(len(head)))
	packet = append(packet, buf...) //写包头长度
	if len(head) > 0 {
		packet = append(packet, head...) //写包头
	}
	//写数据
	//取数据长
	binary.BigEndian.PutUint32(buf[0:4], uint32(len(data)))
	packet = append(packet, buf...) //写数据长度
	if len(data) > 0 {
		packet = append(packet, data...) //写数据
	}

	if len(packet) != cap(packet) {
		return false
	} //数据异常
	if atomic.LoadInt32(&session.status) >= session_status_stoped {
		return false
	}
	session.writeCh <- packet
	return <-session.writeResultCh
}
