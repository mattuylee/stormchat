//主程序
package main

import (
	"bufio"
	"fmt"
	"net"
	"os"
)

var server = NewStormServer()

func main() {
	if server == nil {
		fmt.Println("Failed to create service.")
		return
	}
	tcpAddr, _ := net.ResolveTCPAddr("tcp", ":3727")
	tcpListener, err := net.ListenTCP("tcp", tcpAddr)
	if err != nil {
		fmt.Println("Failed to start listening.")
		return
	}
	if serveMode {
		server.AcceptLoop(tcpListener)
		return
	} //服务模式
	//交互模式
	go server.AcceptLoop(tcpListener)
	for {
		inputReader := bufio.NewReader(os.Stdin)
		cmd, _ := inputReader.ReadString('\n')
		switch cmd {
		case "help\n":
			fmt.Println("help")
			break
		case "quit\n":
		case "exit\n":
			fmt.Println("bye")
			break
		default:
			fmt.Println("invalid command.")
			break
		}
	}
}
