package main

import (
	"fmt"
	"time"
)

//写日志
func WriteLog(err string) {
	fmt.Fprintln(server.logFile, time.Now().String(), " ", err)
}

//输出调试信息
func Debug(msg string) {
	if serveMode {
		return
	}
	fmt.Println(msg)
}
