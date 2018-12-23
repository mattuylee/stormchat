package main

import (
	"encoding/binary"
	"encoding/json"
	"net"
	"sync/atomic"
	"time"
)

//当前session的状态
const (
	session_status_created = iota //已创建，但未登录
	session_status_running        // 通道已建立，session正常运行
	session_status_stoped         // 标识连接已断开
	session_status_destroyed
)

type Session struct {
	sender        *UserInfo    //消息发送者
	receiver      string       //消息接收者
	friends       []UserInfo   //好友列表
	conn          *net.TCPConn //TCP连接
	status        int32        //session运行状态
	stopChan      chan bool    //Session终止标识，传递数据即终止。仅由读消息循环用于终止写循环
	writeCh       chan []byte  //写消息通道
	writeResultCh chan bool    //是否写成功
}

//初始化
func NewSession() *Session {
	tcpAddr, _ := net.ResolveTCPAddr("tcp", server_addr)
	conn, err := net.DialTCP("tcp", nil, tcpAddr)
	if err != nil {
		PrintError("连接服务器失败：" + err.Error())
		return nil
	}
	var session = new(Session)
	session.sender = nil
	session.receiver = ""
	session.status = session_status_created
	session.stopChan = make(chan bool)
	session.writeCh = make(chan []byte)
	session.writeResultCh = make(chan bool)
	session.conn = conn
	go session.SendLoop()
	go session.ReceiveLoop()
	return session
}

//销毁聊天连接
func (session *Session) destroy() {
	if session.status == session_status_destroyed {
		return
	}
	session.status = session_status_destroyed
	session.conn.Close()
	close(session.writeCh)
	close(session.writeResultCh)
}

//消息写循环
func (session *Session) SendLoop() {
	defer session.destroy()
	for {
		select {
		case data, ok := <-session.writeCh:
			if !ok {
				atomic.SwapInt32(&session.status, session_status_stoped)
				continue
			}
			writeLen, err := session.conn.Write(data)
			if err != nil || (writeLen != len(data)) {
				PrintError(err.Error())
				atomic.SwapInt32(&session.status, session_status_stoped)
				session.writeResultCh <- false
			} //写失败
			session.writeResultCh <- true
		case <-session.stopChan:
			return
		}
	}
}

//消息读循环
func (session *Session) ReceiveLoop() {
	defer func() {
		session.stopChan <- true
	}() //终止写循环
	var err error //错误
	for {
		if atomic.LoadInt32(&session.status) >= session_status_stoped {
			return
		} //检查session是否已终止
		//读取数据
		head := session.readDataWithLength()
		data := session.readDataWithLength()

		if head == nil || data == nil {
			PrintError("获取数据失败")
			atomic.SwapInt32(&session.status, session_status_stoped)
			continue
		}
		//解析包头
		headInfo := make(map[string]string)
		err = json.Unmarshal(head, &headInfo)
		if err != nil {
			PrintError("解析包头失败")
			continue
		}
		switch headInfo[keyname_operation] {
		case operation_send_message:
			session.ResultHandler(headInfo)
			break
		case operation_update_userinfo:
			session.ResultHandler(headInfo)
			break
		case operation_offline:
		case operation_panic:
			session.PanicHandler(headInfo)
			break
		case operation_login:
			session.LoginDoneHandler(headInfo, data)
			break
		case operation_trans_message:
			session.MessageHandler(headInfo, data)
			break
		case operation_get_user_list:
			session.GetUserListHandler(headInfo, data)
			break
		default:
			continue
		}
	}
}

//处理请求结果反馈消息
func (session *Session) ResultHandler(headInfo map[string]string) {
	if headInfo["Error"] == "" {
		PrintLog(headInfo[keyname_operation] + " successfully.")
	} else {
		PrintError("Failed to " + headInfo[keyname_operation] + ": " + headInfo["Error"])
	}
}

//服务器报错
func (session *Session) PanicHandler(headInfo map[string]string) {
	PrintError("Server Error: " + headInfo["Error"])
	atomic.SwapInt32(&session.status, session_status_stoped)
}

//登录反馈
func (session *Session) LoginDoneHandler(headInfo map[string]string, data []byte) {
	session.ResultHandler(headInfo)
	if headInfo["Error"] == "" {
		user := new(UserInfo)
		err := json.Unmarshal(data, user)
		if err != nil {
			PrintError("解析用户信息失败")
			return
		} else {
			session.sender = user
			SetUserNameText(user.NickName)
		}
		atomic.SwapInt32(&session.status, session_status_running)
		session.GetUserList()
	}
}

//获取用户列表
func (session *Session) GetUserListHandler(headInfo map[string]string, data []byte) {
	session.ResultHandler(headInfo)
	if headInfo["Error"] != "" {
		return
	}
	err := json.Unmarshal(data, &session.friends)
	if err != nil {
		PrintError("解析用户信息失败: " + err.Error())
	}
}

//输出消息
func (session *Session) MessageHandler(headInfo map[string]string, message []byte) {
	if headInfo["Error"] != "" {
		PrintError("发送消息失败: " + headInfo["Error"])
		return
	}
	var user *UserInfo = nil
	for _, item := range session.friends {
		if item.User == headInfo["From"] {
			user = &item
			break
		}
	}
	when, _ := time.Parse(time.RFC3339, headInfo["When"])
	if user == nil {
		PrintMessageHead(headInfo["From"]+"   ["+when.Format("2006-01-02 15:04:05")+"]", false)
	} else {
		PrintMessageHead(user.NickName+"   ["+when.Format("2006-01-02 15:04:05")+"]", false)
	}
	PrintMessage(string(message))
	if session.receiver == "" && user != nil {
		session.receiver = user.User
		SetCurrentFriendText(user.NickName)
	} //如果无当前聊天对象则将聊天对象设置为消息发送者
}

//发送数据
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

//读取数据
func (session *Session) readDataWithLength() []byte {
	var dataLen uint32    //数据长度
	var lenBuffer [4]byte //数据长度缓存区
	var data []byte       //数据缓存区
	//获取数据长度
	readedLen, err := session.conn.Read(lenBuffer[0:4])
	if err != nil || readedLen != 4 {
		PrintError("获取消息失败。 Error: " + err.Error())
		return nil
	}
	dataLen = binary.BigEndian.Uint32(lenBuffer[0:4])
	data = make([]byte, dataLen)
	if dataLen == 0 {
		return data
	}
	readedLen, err = session.conn.Read(data)
	if err != nil || readedLen != int(dataLen) {
		PrintError("获取消息失败。Error：" + err.Error())
		return nil
	}
	return data
}

//发送数据
func (session *Session) SendData(headInfo map[string]string, data []byte) bool {
	head, err := json.Marshal(headInfo)
	if err != nil {
		PrintError("Marshal Failed.")
		return false
	}
	return session.send(head, data)
}

//发送消息
func (session *Session) SendMessage(msg []byte) bool {
	if session.sender == nil {
		PrintError("请先登录")
		return false
	}
	if session.receiver == "" {
		PrintError("请先设定当前聊天好友")
		return false
	}
	if len(msg) == 0 {
		PrintError("无法发送空消息")
		return false
	}
	PrintMessageHead(session.sender.NickName+"   ["+time.Now().Format("2006-01-02 15:04:05")+"]", true)
	PrintMessage(string(msg))
	headInfo := MakeHead(operation_send_message)
	headInfo["To"] = session.receiver
	return session.SendData(headInfo, msg)
}

//登录
func (session *Session) Login(user string, pwd string) bool {
	logInfo := MakeHead(operation_login)
	logInfo["User"] = user
	logInfo["Pwd"] = pwd
	return session.SendData(logInfo, nil)
}

//请求登出
func (session *Session) Logout() {
	if session.sender == nil {
		PrintError("当前未登录")
		return
	} //未登录
	headInfo := MakeHead(operation_logout)
	session.SendData(headInfo, nil)
	atomic.SwapInt32(&session.status, session_status_created)
	session.sender = nil
	session.friends = nil
	session.receiver = ""
	SetUserNameText("未登录")
	SetCurrentFriendText("")
	PrintLog("已登出.")
}

//获取好友列表
func (session *Session) GetUserList() {
	headInfo := MakeHead(operation_get_user_list)
	session.SendData(headInfo, nil)
}
