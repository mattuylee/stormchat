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
	#endregion

	#region 定义Json序列化与反序列化时使用的数据结构，仅内部使用
	//登录信息包头
	internal class JsonLogInfoHead : BaseHead
	{
		public string User;
		public string Pwd;
	}

	//用户信息
	internal class JsonUserInfo
	{
		public string User;     //用户名
		public string NickName; //昵称
		public string Motto;    //个性签名
		public string UGroup;	//用户组
	}
	#endregion
}
