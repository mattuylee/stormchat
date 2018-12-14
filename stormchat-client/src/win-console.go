package main

import (
	"math"
	"strings"
	"syscall"
	"unsafe"
)

const (
	FOREGROUND_BLUE      int = 1
	FOREGROUND_GREEN     int = 2
	FOREGROUND_RED       int = 4
	FOREGROUND_INTENSITY int = 8
	BACKGROUND_BLUE      int = 16
	BACKGROUND_GREEN     int = 32
	BACKGROUND_RED       int = 64
	BACKGROUND_INTENSITY int = 128

	FORGROUND_WHITE int = 7
)

var conctrl *syscall.LazyDLL    //Windows控制台窗口辅助输出库
var consoleWindow uintptr       //控制台窗口控制器指针
var messagePannel uintptr       //消息输出窗格
var operationPannel uintptr     //操作提示窗格
var usrenamePannel uintptr      //用户名显示窗格
var currentFriendPannel uintptr //当前聊天对象显示窗格
var inputPannel uintptr         //输入窗格
var spliter uintptr             //分割线
//初始化
func InitConsole() {
	conctrl = syscall.NewLazyDLL("conctrl_x64.dll")
	proc := conctrl.NewProc("CreateConsoleWindow")
	consoleWindow, _, _ = proc.Call(100, 999)
	if consoleWindow == 0 {
		panic("Failed to init console. conctrl.dll not found.")
	}
	proc = conctrl.NewProc("CreatePannel")
	messagePannel, _, _ = proc.Call(consoleWindow, 0, 0, 100, 20)
	operationPannel, _, _ = proc.Call(consoleWindow, 0, 21, 20, 1)
	usrenamePannel, _, _ = proc.Call(consoleWindow, 35, 21, 30, 1)
	currentFriendPannel, _, _ = proc.Call(consoleWindow, 65, 21, 35, 1)
	inputPannel, _, _ = proc.Call(consoleWindow, 0, 23, 100, 20)
	proc = conctrl.NewProc("CreateSpliter")
	spliter, _, _ = proc.Call(consoleWindow, 0, 22, 100, 0, uintptr(FORGROUND_WHITE))
	proc = conctrl.NewProc("FocusOnPannel")
	proc.Call(inputPannel, 0, 0)
	SetTitle("StormChat")
}

//滚动输出区域
func ScrollOutputArea(lineCount int) {
	var proc *syscall.LazyProc
	if lineCount > 0 {
		proc = conctrl.NewProc("ScrollPannelForward")
	} else {
		proc = conctrl.NewProc("ScrollPannelBackward")
	}
	proc.Call(messagePannel, uintptr(math.Abs(float64(lineCount))))
}

//设置控制台标题
func SetTitle(title string) {
	proc := conctrl.NewProc("SetConsoleWindowTitle")
	titleB := append([]byte(title), 0)
	pTtile := *(*uintptr)(unsafe.Pointer(&titleB))
	proc.Call(uintptr(pTtile), 1)
}

//设置当前操作提示文本
func SetOperationText(op string) {
	proc := conctrl.NewProc("ClearPannel")
	proc.Call(operationPannel)
	AddPannelLine(operationPannel, op, false, FORGROUND_WHITE)
}

//设置用户昵称显示区文本
func SetUserNameText(name string) {
	blank := (30 - len(name)) / 2
	if blank > 0 {
		name = strings.Repeat(" ", blank) + name
	}
	proc := conctrl.NewProc("ClearPannel")
	proc.Call(usrenamePannel)
	AddPannelLine(usrenamePannel, name, false, FORGROUND_WHITE)
}

//设置当前聊天好友提示文本
func SetCurrentFriendText(friend string) {
	if friend != "" {
		friend = "To: " + friend
	}
	blank := 34 - len(friend)
	if blank > 0 {
		friend = strings.Repeat(" ", blank) + friend
	}
	proc := conctrl.NewProc("ClearPannel")
	proc.Call(currentFriendPannel)
	AddPannelLine(currentFriendPannel, friend, false, FORGROUND_WHITE)
}

//向输出区写行
func PrintOutputLine(text string, attribute int) {
	AddPannelLine(messagePannel, text, false, attribute)
}

//向输入区写文本
func PrintInputTip(text string) {
	ClearInput()
	AddPannelText(inputPannel, text, true, FORGROUND_WHITE)
}

//清空输入区
func ClearInput() {
	proc := conctrl.NewProc("ClearPannel")
	proc.Call(inputPannel)
	proc = conctrl.NewProc("FocusOnPannel")
	proc.Call(inputPannel, 0, 0)
}

func ClearOutput() {
	proc := conctrl.NewProc("ClearPannel")
	proc.Call(messagePannel)
}

//销毁控制器，恢复控制台
func DestroyConsoleWindow() {
	proc := conctrl.NewProc("FocusOnPannel")
	proc.Call(messagePannel, 0, 0)
	proc = conctrl.NewProc("DestroyConsoleWindow")
	proc.Call(consoleWindow)
}

//向窗格加入文本行
func AddPannelLine(pannel uintptr, text string, focus bool, attribute int) {
	proc := conctrl.NewProc("AddPannelLine")
	addPannel(proc, pannel, text, focus, attribute)
}

//向窗格加入文本
func AddPannelText(pannel uintptr, text string, focus bool, attribute int) {
	proc := conctrl.NewProc("AddPannelText")
	addPannel(proc, pannel, text, focus, attribute)
}

//向窗格加入文本（行）
func addPannel(proc *syscall.LazyProc, pannel uintptr, text string, focus bool, attribute int) {
	line := append([]byte(text), 0)
	pLine := *(*uintptr)(unsafe.Pointer(&line))
	var focusInt int
	if focus {
		focusInt = 1
	} else {
		focusInt = 0
	}
	proc.Call(pannel, uintptr(pLine), uintptr(focusInt), 1, uintptr(attribute))
}
