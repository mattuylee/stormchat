package main

import (
	"encoding/base64"
	"time"
)

//消息结构
type Message struct {
	When time.Time //接收时间
	From *UserInfo //发送者
	To   *UserInfo //接收者
	Msg  []byte    //消息体
}

//消息存储结构，用于消息数据入库/出库
type MessageInfo struct {
	Id   int
	When string
	From string
	To   string
	Msg  string
}

func NewMessage(msgInfo *MessageInfo) *Message {
	m := new(Message)
	m.When, _ = time.Parse(time.RFC3339, msgInfo.When)
	m.From = QueryUserInfo(msgInfo.From)
	m.To = QueryUserInfo(msgInfo.To)
	var err error
	m.Msg, err = base64.StdEncoding.DecodeString(msgInfo.Msg)
	if m.From == nil || m.To == nil || err != nil {
		return nil
	}
	return m
}

//将消息存储到数据库
func (message *Message) Save() bool {
	_, err := server.db.Exec(str_sql_save_message, message.When.Format(time.RFC3339),
		message.From.User, message.To.User, base64.StdEncoding.EncodeToString(message.Msg))
	if err != nil {
		WriteLog("SaveMessage Error: " + err.Error())
		return false
	}
	return true
}

//查询用户未读消息
func QuerySavedMessages(user *UserInfo) map[int]Message {
	messageMap := make(map[int]Message)
	rows, err := server.db.Query(str_sql_get_message, user.User)
	if err != nil {
		WriteLog("[QuerySavedMessage]Quering Failed: " + err.Error())
		return nil
	}
	for rows.Next() {
		msgInfo := new(MessageInfo)

		scanErr := rows.Scan(&msgInfo.Id, &msgInfo.When, &msgInfo.From, &msgInfo.To, &msgInfo.Msg)
		if scanErr != nil {
			WriteLog("[QuerySavedMessage]Scan Failed: " + scanErr.Error())
			rows.Close()
			return nil
		}
		m := NewMessage(msgInfo)
		if m == nil {
			WriteLog("[QuerySavedMessage]Bad Message. " + err.Error())
			DeleteSavedMessage(msgInfo.Id) //删除无效消息
		} else {
			messageMap[msgInfo.Id] = *m
		}
	}
	err = rows.Err()
	if err != nil {
		WriteLog("[QuerySavedMessage]Rows Failed: " + err.Error())
		return nil
	}
	return messageMap
}

//删除数据库中的未读消息
func DeleteSavedMessage(id int) bool {
	_, err := server.db.Exec(str_sql_delete_message, id)
	if err != nil {
		WriteLog("[DeleteSavedMessage]Delete Message Failed: " + err.Error())
		return false
	}
	return true
}
