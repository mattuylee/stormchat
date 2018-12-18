using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//定义Json序列化与反序列化时使用的数据结构，仅内部使用

namespace Interact
{
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
}
