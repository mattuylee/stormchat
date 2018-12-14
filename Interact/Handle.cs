using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Interact
{
	//对数据的处理相关内容
	public partial class StormClient
	{
		//根据包头信息处理数据
		private static void HandleAll(byte[] headData, byte[] data)
		{
			string head = Encoding.UTF8.GetString(headData);
			JObject jsonObj = (JObject)JsonConvert.DeserializeObject(head);
			switch ((Operations)Enum.Parse(typeof(Operations), jsonObj[Strings.Operation].ToString()))
			{
			case Operations.Login:
				HandleLoginResult(jsonObj, data);
				break;
			case Operations.Panic:
			case Operations.TransMessage:
			case Operations.SendMessage:
			case Operations.Ping:
			case Operations.Logout:
			case Operations.Offline:
			case Operations.GetUserList:
			case Operations.UpdateUserInfo:
			case Operations.GetUserPhoto:
			default:
				break;
			}
		}
		//处理登录反馈消息
		private static void HandleLoginResult(JObject head, byte[] data)
		{
			User user = null;
			if (head[Strings.Error].ToString() == "")
				user = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(data)) as User;
			OnLoginDone?.Invoke(head, user);
		}
	}

}
