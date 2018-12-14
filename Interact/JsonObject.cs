using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//此文件定义Json序列化与反序列化时使用的数据结构，仅内部使用

namespace Interact
{
	//登录信息包头
	internal class JsonLogInfoHead : BaseHead
	{
		public string User;
		public string Pwd;
	}

}
