package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
	"time"
)

/*
#include <conio.h>
#include <stdlib.h>
int getkey() {
	return getch();
}
*/
import "C"

var commandMode bool //指示当前是否处于命令模式
func main() {
	defer func() {
		fmt.Print("按任意键退出程序")
		C.getkey()
		DestroyConsoleWindow()
	}()
	InitConsole()
	client := NewSession()
	if client == nil {
		return
	}
	SetUserNameText("未登录")
	SetCurrentFriendText("")
	PrintInfo(help_message)
	if len(os.Args) >= 3 {
		client.Login(os.Args[1], os.Args[2])
		commandMode = false
	}
	commandMode = true
	PrintInfo(command_list)
	if InputCommand(client) {
		return
	}
	var message string
	var input string
	for {
		if message == "" {
			ClearInput()
		}
		reader := bufio.NewReader(os.Stdin)
		input, _ = reader.ReadString('\n')
		input = strings.TrimSuffix(input, "\r\n")
		input = strings.TrimSuffix(input, "\n")
		switch input {
		case ".":
			message = ""
			if InputCommand(client) {
				return
			}
		case ".quit":
			return
		case ".exit":
			return
		default:
			message += input
			if strings.HasSuffix(input, "\\") {
				message = strings.TrimSuffix(message, "\\")
				message += "\r\n"
			} else {
				client.SendMessage([]byte(message))
				message = ""
			} //发送成功，清空消息缓存
		}
	}
}

//输入指令
func InputCommand(client *Session) bool {
	defer SetOperationText("聊天")
	SetOperationText("命令模式")
	for {
		ClearInput()
		var input string
		reader := bufio.NewReader(os.Stdin)
		input, _ = reader.ReadString('\n')
		input = strings.TrimSuffix(input, "\r\n")
		input = strings.TrimSuffix(input, "\n")
		switch strings.TrimRight(input, " ") {
		case "scroll":
			ScrollMessage()
		case "login":
			success := LoginWithInput(client)
			if !success {
				client.destroy()
				client = NewSession()
				PrintError("登录失败，请重试。")
			} //如果连接已中断则重新创建session
		case "logout":
			client.Logout()
		case "to":
			SwitchReceiver(client)
		case "friends":
			ViewFriends(client)
		case "clear":
			ClearOutput()
		case "help":
			PrintInfo(command_list)
		case ".":
			return false
		case "exit":
			return true
		default:
			PrintError("无效命令")
		}
	}
}

//发起登录。如果返回值为false则session已失效（根据发送数据失败判断）
func LoginWithInput(session *Session) bool {
	SetOperationText("登录")
	defer func() {
		if commandMode {
			SetOperationText("命令模式")
		} else {
			SetOperationText("聊天")
		}
	}()
	if session.status == session_status_running {
		PrintError("已经登录，请先注销。")
		return true //虽然登录失败但连接仍有效，因此返回true
	}
	reader := bufio.NewReader(os.Stdin)
	PrintInputTip("user: ")
	user, _ := reader.ReadString('\n')
	user = strings.TrimSuffix(user, "\r\n")
	user = strings.TrimSuffix(user, "\n")
	PrintInputTip("password: ")
	pwd, _ := reader.ReadString('\n')
	pwd = strings.TrimSuffix(pwd, "\r\n")
	pwd = strings.TrimSuffix(pwd, "\n")
	if !session.Login(user, pwd) {
		return false
	}
	return true
}

//更换聊天对象
func SwitchReceiver(session *Session) {
	if session.status != session_status_running {
		PrintError("请先登录")
		return
	}
	SetOperationText("切换好友")
	PrintInputTip("好友ID或昵称(输入*启用广播模式): ")
	var found bool = false //好友是否存在
	var friend string
	reader := bufio.NewReader(os.Stdin)
	friend, _ = reader.ReadString('\n')
	friend = strings.TrimSuffix(friend, "\r\n")
	friend = strings.TrimSuffix(friend, "\n")
	if friend == "*" {
		SetCurrentFriendText("广播模式")
		session.receiver = friend
	} //广播消息
	//查询好友列表
	for _, item := range session.friends {
		if strings.ToLower(item.NickName) == strings.ToLower(friend) || item.User == friend {
			SetCurrentFriendText(item.NickName)
			session.receiver = item.User
			found = true
			break
		}
	}
	if commandMode {
		SetOperationText("命令模式")
	} else {
		SetOperationText("聊天")
	}
	if !found {
		PrintError("未找到名为【" + friend + "】的好友")
	}
}

//查看好友列表
func ViewFriends(session *Session) {
	list := session.friends
	if session.status != session_status_running {
		PrintError("请先登录")
		return
	}
	if len(list) == 0 {
		PrintInfo("好友列表为空")
		return
	}
	for _, item := range list {
		PrintInfo(item.User + "    " + item.NickName)
	}
}

//滚动消息面板
func ScrollMessage() {
	if commandMode {
		defer SetOperationText("命令模式")
	} else {
		defer SetOperationText("聊天")
	}
	defer ClearInput()
	SetOperationText("翻页模式")
	PrintInputTip("上翻：P\n下翻：N\n退出翻页：ESC\n")
	for {
		key := C.getkey()
		switch key {
		case 'p':
			ScrollOutputArea(-1)
		case 'n':
			ScrollOutputArea(1)
		case 27:
			return
		}
	}
}

//输出错误
func PrintError(err string) {
	PrintOutputLine("[ERROR] "+err, FOREGROUND_RED)
}

//输出信息
func PrintInfo(text string) {
	PrintOutputLine(text, FOREGROUND_RED|FOREGROUND_GREEN)
}
func PrintLog(str string) {
	PrintOutputLine("[INFO] "+str, FOREGROUND_INTENSITY)
}

//输出消息头
func PrintMessageHead(head string, intensity bool) {
	if intensity {
		PrintOutputLine(head, FOREGROUND_INTENSITY)
	} else {
		PrintOutputLine(head, FOREGROUND_GREEN)
	}
}

//输出消息体
func PrintMessage(msg string) {
	PrintOutputLine(msg, FORGROUND_WHITE)
}

//构造消息头
func MakeHead(operation string) map[string]string {
	headInfo := make(map[string]string)
	headInfo[keyname_token] = string(time.Now().UnixNano())
	headInfo[keyname_operation] = operation
	return headInfo
}
