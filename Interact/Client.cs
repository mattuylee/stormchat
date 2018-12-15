using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing;

namespace Interact
{
	//连接中断
	public delegate void DisconnectHandler(Exception ex);
	//通用请求结果处理
	public delegate void ResultHandler(ResultHead head);
	//消息到达事件处理
	public delegate void MessageHandler(Message msg);
	//登录反馈到达
	public delegate void LoginDoneHandler(ResultHead head, User user);
	//获取用户列表结果处理
	public delegate void GetUserListDoneHandler(ResultHead head, User[] users);
	//获取用户头像结果处理
	public delegate void GetUserPhotoDoneHandler(ResultHead head, Image image);


	/// <summary>
	/// 与服务器进行数据通信，并向外部提供一系列通信接口
	/// </summary>
	public static partial class StormClient
	{
		//连接中断事件
		public static event DisconnectHandler OnDisconnect;
		//服务器异常消息处理事件
		public static event ResultHandler OnPanic;
		//服务器强制注销登陆事件
		public static event ResultHandler OnOffline;
		//消息到达事件
		public static event MessageHandler OnMessage;
		//发送消息处理完成反馈事件（仅在DoesSendMessageReturn为true时有效）
		public static event ResultHandler OnSendMessageDone;
		//更新用户信息结果处理事件
		public static event ResultHandler OnUpdateUserInfoDone;
		//登录结果处理事件
		public static event LoginDoneHandler OnLoginDone;
		//获取用户列表结果处理事件
		public static event GetUserListDoneHandler OnGetUserListDone;
		//获取用户头像结果处理事件
		public static event GetUserPhotoDoneHandler OnGetUserPhotoDoneHandler;
		//发送消息时是否要求服务器返回处理结果
		public static bool DoesSendMessageReturn
		{
			get
			{
				return doesSendMessageReturn != 0;
			}
			set
			{
				Interlocked.Exchange(ref StormClient.doesSendMessageReturn, value ? 1 : 0);
			}
		}

		//TCP客户端
		private static TcpClient tcpClient;
		//消息发送队列
		private static BlockingCollection<Packet> sendQueue;
		//数据读取线程
		private static Thread readLoopThread;
		//数据发送线程
		private static Thread sendLoopThread;
		//发送消息时是否要求服务器返回处理结果
		private static volatile int doesSendMessageReturn = 0;

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
		/// 登录。此操作为异步操作，服务器返回结果后产生OnLoginDone事件。
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
