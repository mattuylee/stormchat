using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

//定义StormClient私有字段

namespace Interact
{
	public partial class StormClient
	{
		//TCP客户端
		private static TcpClient tcpClient;
		//消息发送队列
		private static BlockingCollection<Packet> sendQueue;
		//数据读取线程
		private static Thread readLoopThread;
		//数据发送线程
		private static Thread sendLoopThread;
		//某些请求需要持续的等待服务器返回数据包，然后将所有数据包处理后提供给用户，
		//	 此字段用于暂存这些未接收完全的数据包。它的key是对应请求的Token字串。
		private static Dictionary<string, object> workingDictionary;

		//属性控制字段，任何地方都不应该修改它们
		//发送消息时是否要求服务器返回处理结果。由DoesSendMessageReturn属性控制，不要直接修改这个字段
		private static volatile bool doesSendMessageReturnResult = false;
		//当前连接状态。由Status属性控制，不要直接修改这个字段
		private static volatile ClientStatus clientStatus = ClientStatus.Uninitialized;

		//线程安全-锁
		//更改连接状态锁
		private static object statusLocker = new object();
		//"发送消息时是否要求服务器返回处理结果"Falg锁
		private static object doesSendMessageReturnLocker = new object();
	}
}
