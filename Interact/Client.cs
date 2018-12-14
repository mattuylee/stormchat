using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace Interact
{
	//连接中断
	public delegate void DisconnectHandler(Exception ex);
	//登录反馈到达
	public delegate void LoginDoneHandler(JObject head, User user);

	public static partial class StormClient
	{
		//连接中断事件
		public static event DisconnectHandler OnDisconnect;
		//登录结果处理事件
		public static event LoginDoneHandler OnLoginDone;

		//TCP客户端
		private static TcpClient tcpClient;
		//消息发送队列
		private static BlockingCollection<Packet> sendQueue;
		//数据读取线程
		private static Thread readLoopThread;
		//数据发送线程
		private static Thread sendLoopThread;

		static StormClient()
		{
			tcpClient = new TcpClient();
			sendQueue = new BlockingCollection<Packet>();
		}
		/// <summary>
		/// 初始化客户端，连接服务器。如果成功则启动数据接收和发送线程
		/// </summary>
		/// <returns>如果已连接到服务器返回true，否则返回false。</returns>
		public static bool Initialize()
		{
			if (StormClient.tcpClient.Connected)
				return true;
			StormClient.tcpClient.Connect(Interact.Properties.Resources.RemoteServerAddr, int.Parse(Interact.Properties.Resources.RemoteServerPort));
			if (!StormClient.tcpClient.Connected)
				return false;
			if (sendLoopThread != null)
				sendLoopThread.Abort();
			if (readLoopThread != null)
				readLoopThread.Abort();
			//启动数据收发线程
			sendLoopThread = new Thread(new ThreadStart(SendLoop));
			readLoopThread = new Thread(new ThreadStart(ReadLoop));
			//设置为后台线程
			sendLoopThread.IsBackground = readLoopThread.IsBackground = true;
			sendLoopThread.Start();
			readLoopThread.Start();
			return true;
		}

		/// <summary>
		/// 登录。此操作为异步操作，完成后产生OnLoginDone事件。
		/// </summary>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		/// <param name="user">用户名</param>
		/// <param name="password">密码</param>
		/// <param name="callback">数据发送完成回调函数。注意，结果处理回调请设置OnLoginDone事件</param>
		public static bool RequestLogin(string user, string password, Action<BaseHead> callback = null)
		{
			JsonLogInfoHead logInfo = new JsonLogInfoHead()
			{
				Token = "",
				Operation = Operations.Login.ToString(),
				User = user,
				Pwd = password
			};
			Packet packet = new Packet
			{
				Head = logInfo,
				Data = null,
				CallBack = callback
			};
			return Send(packet);
		}

	}
}
