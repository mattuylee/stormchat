using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interact
{
	#region 定义数据交互用到的数据结构（可能作为事件或回调函数的参数）
	//包头基本结构
	public class BaseHead
	{
		//请求ID，由发送方定义的随机字符串。如果接受方有返回数据应包含相同的Token
		//此特性暂时未启用，传空字符串即可
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
	//获取单一用户信息结果包头
	public class GetUserInfoResultHead : ResultHead
	{
		
	}
	#endregion

	#region 定义Json序列化与反序列化时使用的数据结构，仅内部使用
	//登录信息包头
	internal class JsonLogInfoHead : BaseHead
	{
		public string User;
		public string Pwd;
	}

	//发送消息包头
	internal class JsonSendMessageHead : BaseHead
	{
		public string To; //消息接收者用户名
		public string NeedResult = "0"; //是否要求服务器返回处理结果。否传“0”
	}

	//更新用户信息包头
	internal class JsonUserInfoHead : BaseHead
	{
		public string NickName = null;
		public string Password = null;
		public string Motto = null;
		public int Photo = 0;  //新头像大小
	}

	//获取用户头像请求的包头
	internal class JsonGetUserPhotoHead : BaseHead
	{
		public string User;	//要获取头像的用户名
	}
	
	#endregion
}
