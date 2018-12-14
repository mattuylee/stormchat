package main

import (
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
	"net"
	"os"
)

type StormServer struct {
	listener   *net.TCPListener
	logFile    *os.File //日志文件
	db         *sql.DB
	sessionMap map[string]*Session //session列表
}

//创建服务
func NewStormServer() *StormServer {
	var newServer = new(StormServer)
	var err error
	//打开日志文件
	if serveMode {
		newServer.logFile, err = os.OpenFile(str_log_file, os.O_APPEND|os.O_CREATE|os.O_WRONLY, 002)
		if err != nil {
			Debug("Failed to open log file.")
			return nil
		}
	} else {
		newServer.logFile = os.Stderr
	}
	//连接数据库
	newServer.db, _ = sql.Open("mysql", str_db_conn_str)
	err = newServer.db.Ping()
	if err != nil {
		Debug("Failed to connect to the database.")
		return nil
	}

	newServer.sessionMap = make(map[string]*Session)
	return newServer
}

//销毁服务
func (server *StormServer) Destroy() {
	if !serveMode {
		server.logFile.Close()
	}
	server.db.Close()
}

//监听新的TCP连接
func (server *StormServer) AcceptLoop(tcpListener *net.TCPListener) {
	for {
		tcpConn, _ := tcpListener.AcceptTCP()
		var newChat = NewSession(tcpConn)
		go newChat.SendLoop()
		go newChat.ReceiveLoop()
	}
}
