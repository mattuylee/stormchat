using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;

//StormClient对数据的处理相关内容

namespace Interact
{
	public partial class StormClient
	{
		//处理连接中断异常
		private static void HandleConnectionBroken(Exception ex)
		{
            lock (statusLocker)
            {
				if (Status != ClientStatus.Running)
					return;
				statusLocker = ClientStatus.Stopped;
				tcpClient.Close();
				OnDisconnect?.Invoke(ex);
			}
		}

		#region 处理服务器数据
		//根据包头信息处理数据
		private static void HandleAll(byte[] headData, byte[] data)
		{
			string head = Encoding.UTF8.GetString(headData);
			JObject jsonObj = (JObject)JsonConvert.DeserializeObject(head);
			switch (jsonObj[AttrNames.Operation].ToString())
			{
			case Operations.Login:
				HandleLoginResult(jsonObj, data);
				break;
			case Operations.Panic:
				HandlePanic(GetResultHead(jsonObj));
				break;
			case Operations.TransMessage:
				break;
			case Operations.SendMessage:
				break;
			case Operations.Logout:
				break;
			case Operations.Offline:
				break;
			case Operations.UpdateUserInfo:
				break;
			case Operations.GetUsers:
				break;
			default:
				break;
			}
		}
		//提取基本结果包头结构
		private static ResultHead GetResultHead(JObject head)
		{
			ResultHead resultHead = new ResultHead
			{
				Token = head[AttrNames.Token].ToString(),
				Operation = head[AttrNames.Operation].ToString(),
				Error = head[AttrNames.Error].ToString()
			};
			return resultHead;
		}

		//处理服务器异常断开连接消息
		private static void HandlePanic(ResultHead head)
		{
			ConnectionBrokenException ex = new ConnectionBrokenException(head.Error);
			HandleConnectionBroken(ex);
		}

		//处理登录反馈消息
		private static void HandleLoginResult(JObject head, byte[] data)
		{
			//提取结果包头
			ResultHead resultHead = GetResultHead(head);
			//发起登录完成事件
			if (head[AttrNames.Error].ToString() != "")
			{
				OnLoginDone?.Invoke(resultHead, null);
				return;
			}
			User user = new User
			{
				Name = head[AttrNames.User].ToString(),
				NickName = head[AttrNames.NickName].ToString(),
				Motto = head[AttrNames.Motto].ToString(),
				Group = (UserGroup)Enum.Parse(typeof(UserGroup), head[AttrNames.UGroup].ToString()),
				Photo = User.DefaultPhoto
			};
			if (int.Parse(head[AttrNames.Photo].ToString()) > 0)
			{
				MemoryStream ms = new MemoryStream(data);
				user.Photo = Image.FromStream(ms);
			} //用户头像数据
			OnLoginDone?.Invoke(resultHead, user);
		}

		//处理获取用户列表的不连续返回包。当所有包接收完毕后返回给客户
		private static void HandleGetUsersPack(JObject head, byte[] data)
		{
			if (head[AttrNames.Error].ToString() != string.Empty)
			{
				OnGetUserListDone?.Invoke(GetResultHead(head), null);
				return;
			} //获取失败

		}
		#endregion

	}

}
