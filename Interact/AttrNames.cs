﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interact
{
	//Json数据的字段访问字符串
	public static class AttrNames
	{
		//基本信息
		public const string Token = "Token";
		public const string Operation = "Operation";

		//处理结果信息
		public const string Error = "Error";
		
		//用户信息
		public const string User = "User";
		public const string NickName = "NickName";
		public const string Motto = "Motto";
		public const string UGroup = "UGroup";
		public const string Photo = "Photo";
	}
}
