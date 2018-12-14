using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interact
{
	//用户
	public class User
	{
		public static User Me;	//当前用户

		public string Name;     //用户名
		public string NickName; //昵称
		public string Motto;	//个性签名
		public UserGroup Group;	//用户组
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
