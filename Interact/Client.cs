using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Interact
{
	//连接中断
	public delegate void ConnectionBrokenHandler(Exception ex);
	//通用请求结果处理
	public delegate void ResultHandler(ResultHead head);
	//消息到达事件处理
	public delegate void MessageHandler(Message msg);
	//登录反馈到达
	public delegate void LoginDoneHandler(ResultHead head, User user);
	//获取用户列表结果处理
	public delegate void GetUserListDoneHandler(ResultHead head, User[] users);

	/// <summary>
	/// 与服务器进行数据通信，并向外部提供一系列通信接口
	/// </summary>
	public static partial class StormClient
	{
		//连接中断事件
		public static event ConnectionBrokenHandler OnDisconnect;
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
		
		//当前连接状态
		public static ClientStatus Status
		{
			get { lock (statusLocker) { return clientStatus; } }
			set { lock (statusLocker) { clientStatus = value; } }
		}
		//发送消息时是否要求服务器返回处理结果
		public static bool DoesSendMessageReturn
		{
			get { lock (doesSendMessageReturnLocker) { return doesSendMessageReturnResult; } }
			set { lock (doesSendMessageReturnLocker) { doesSendMessageReturnResult = value; } }
		}
		

		static StormClient()
		{
			tcpClient = new TcpClient();
			sendQueue = new BlockingCollection<Packet>();
			workingDictionary = new Dictionary<string, object>();
			Status = ClientStatus.Uninitialized;
		}

		/// <summary>
		/// 初始化客户端，连接服务器。如果成功则启动数据接收和发送线程。
		/// 注意，当网络不稳定时此方法可能会阻塞。
		/// </summary>
		/// <returns>如果已连接到服务器返回true，否则返回false。</returns>
		public static bool Initialize()
		{
			if (StormClient.tcpClient.Connected)
				return true;
			try
			{
				StormClient.tcpClient.Connect(Interact.Properties.Resources.RemoteServerAddr, int.Parse(Interact.Properties.Resources.RemoteServerPort));
			}
			catch (Exception)
			{
				return false;
			}
			workingDictionary?.Clear();	//清空临时记录
			if (sendLoopThread != null)
				sendLoopThread.Abort();
			if (readLoopThread != null)
				readLoopThread.Abort();
			//启动数据收发线程
			sendLoopThread = new Thread(new ThreadStart(SendLoop));
			readLoopThread = new Thread(new ThreadStart(ReadLoop));
			sendLoopThread.IsBackground = readLoopThread.IsBackground = true;
			StormClient.Status = ClientStatus.Running;
			sendLoopThread.Start();
			readLoopThread.Start();
			return true;
		}

		/// <summary>
		/// 登录。此操作为异步操作，服务器返回结果后产生OnLoginDone事件。
		/// </summary>
		/// <param name="user">用户名</param>
		/// <param name="password">密码</param>
		/// <param name="callback">数据发送完毕回调函数。注意，结果处理回调请设置OnLoginDone事件</param>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		public static bool QueueLogin(string user, string password, Action<ResultHead> callback = null)
		{
			JsonLogInfoHead logInfo = new JsonLogInfoHead()
			{
				Token = Guid.NewGuid().ToString(),
				Operation = Operations.Login,
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

		/// <summary>
		/// 请求发送一条消息。异步，此方法将请求放入请求队列后返回。
		/// </summary>
		/// <param name="text">要发送的消息文本</param>
		/// <param name="to">消息接收者</param>
		/// <param name="callback">数据发送完毕时回调函数</param>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		public static bool QueueSendMessage(Message message, Action<ResultHead> callback = null)
		{
			JsonSendMessageHead jsonObj = new JsonSendMessageHead()
			{
				Token = "",
				Operation = Operations.SendMessage,
				To = message.To.Name,
				NeedResult = DoesSendMessageReturn ? "1" : "0"
			};
			Packet packet = new Packet
			{
				Head = jsonObj,
				Data = Encoding.UTF8.GetBytes(message.Text),
				CallBack = callback
			};
			return Send(packet);
		}

		/// <summary>
		/// 请求登出。异步，此方法将请求放入请求队列后返回。
		/// </summary>
		/// <param name="callback">数据发送完毕时回调函数</param>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		public static bool QueueLogout(Action<ResultHead> callback = null)
		{
			BaseHead jsonObj = new BaseHead()
			{
				Token = "",
				Operation = Operations.Logout,
			};
			Packet packet = new Packet
			{
				Head = jsonObj,
				Data = null,
				CallBack = callback
			};
			return Send(packet);
		}

		/// <summary>
		/// 请求用户列表。异步，此方法将请求放入请求队列后返回。
		/// </summary>
		/// <param name="callback">数据发送完毕时回调函数</param>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		public static bool QueueGetUserList(Action<ResultHead> callback = null)
		{
			BaseHead jsonObj = new BaseHead()
			{
				Token = "",
				Operation = Operations.GetUsers,
			};
			Packet packet = new Packet
			{
				Head = jsonObj,
				Data = null,
				CallBack = callback
			};
			return Send(packet);
		}

		/// <summary>
		/// 请求更新当前登录用户信息。异步，此方法将请求放入请求队列后返回。
		/// </summary>
		/// <param name="callback">数据发送完毕时回调函数</param>
		/// <returns>成功将请求加入发送队列返回true，否则返回false。</returns>
		public static bool QueueUpdateUserInfo(UserInfo userInfo, Action<ResultHead> callback = null)
		{
			byte[] photoData = null;    //头像数据
										//将头像数据转换到字符数组
			if (userInfo.Photo != null)
			{
				MemoryStream memStream = new MemoryStream();
				userInfo.Photo.Save(memStream, userInfo.Photo.RawFormat);
				photoData = new byte[memStream.Length];
				memStream.Seek(0, SeekOrigin.Begin);
				memStream.Read(photoData, 0, photoData.Length);
			}
			//构建数据包
			JsonUserInfoHead jsonObj = new JsonUserInfoHead()
			{
				Token = "",
				Operation = Operations.UpdateUserInfo,
				NickName = userInfo.NickName,
				Password = userInfo.Password,
				Motto = userInfo.Motto,
				Photo = userInfo.Photo == null ? 0 : photoData.Length
			};
			Packet packet = new Packet
			{
				Head = jsonObj,
				Data = photoData,
				CallBack = callback
			};
			return Send(packet);
		}

		//客户端连接状态
		public enum ClientStatus
		{
			Uninitialized,  //未启动
			Running,        //运行中
			Stopped         //已停止
		}
	}
}
