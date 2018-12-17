//session已不推荐使用的的接口
package main

import (
	"encoding/json"
)

//登录事件处理函数。已弃用，请使用LoginHandler
func (session *Session) Old1_LoginHandler(headInfo map[string]string) {
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

//获取用户列表。已废弃，请使用GetUsers接口
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
