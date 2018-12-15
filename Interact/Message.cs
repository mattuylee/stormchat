using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interact
{
	//定义一则用户接收到的消息
	public class Message
	{
		/*保留
		public const UInt32 MaxHeadLength = 4096; //最大包头长度
		public const UInt32 MaxMessageLength = 0x6400000; //最大数据长度
		*/
		public DateTime When { get; } //服务器接收时间
		public User From; //发送者
		public string Content; //消息内容
		
		//将消息保存到数据库（保留）
		protected bool Save() { return false; }
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
