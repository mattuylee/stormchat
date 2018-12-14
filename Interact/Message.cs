using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interact
{
	//定义一则用户消息
	public class Message
	{
		public static UInt32 MaxHeadLength; //最大包头长度
		public static UInt32 MaxMessageLength; //最大消息长度

		public DateTime When { get; } //服务器接收时间
		public User From; //发送者
		public string Content; //消息内容
	}

	//包头基本结构
	public class BaseHead
	{
		//请求ID，由发送方定义的随机字符串。如果接受方有返回数据应包含相同的Token
		public string Token;
		//操作类型，决定数据的其他内容
		public string Operation;
	}
	// 接受方返回的执行结果反馈包头，包含错误文本
	public class ResultHead : BaseHead
	{
		//错误文本，为空则执行成功
		public string Error;
	}

	//从服务器接收的原始数据封包
	internal class Packet
	{
		public BaseHead Head;   //包头
		public byte[] Data;     //包内容
		public Action<BaseHead> CallBack; //数据发送完成的回调函数。注意，此时仅保证数据已发送，
										  //服务器可能并没有处理完成请求
	}
	//允许请求的操作列表。具体定义见设计文档
	public enum Operations
	{
		Panic,
		TransMessage,
		SendMessage,
		Ping,
		Login,
		Logout,
		Offline,
		GetUserList,
		UpdateUserInfo,
		GetUserPhoto
	}
}
