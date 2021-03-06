﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Interact
{
	//用户
	public class User
	{
		//根据用户名匹配用户。如果用户不存在，则返回一个仅用户名有效的实例
		internal static User FromUserName(string userName)
		{
			if(User.Users != null)
			{
				foreach (User user in User.Users)
				{
					if (user.Name == userName)
						return user;
				}
			}
			User unidentifiedUser = new User
			{
				Name = userName,
				NickName = "",
				Motto = "",
				Group = UserGroup.User,
				Photo = User.DefaultPhoto
			};
			return unidentifiedUser;
		}
		internal static User[] Users;		//用户列表。仅Interact内部访问

		public static User Me;				//当前用户
		public static Image DefaultPhoto;	//默认头像

		public string Name;     //用户名
		public string NickName; //昵称
		public string Motto;	//个性签名
		public UserGroup Group; //用户组
		public Image Photo;		//头像
	}

	//更新用户信息时新的用户信息。如果项值为null则不更改相应的信息
	public class UserInfo
	{
		public string Password = null;	//新密码
		public string NickName = null;	//新昵称
		public string Motto = null;		//新签名
		public Image Photo = null;		//新头像
	}

	//用户类型
	public enum UserGroup
	{
		User,	//普通用户
		Vip,	//会员
		Admin,	//管理员
		Group	//群组
	}
}
