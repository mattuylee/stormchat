using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
				HandleLoginResult(GetResultHead(jsonObj), data);
				break;
			case Operations.Panic:
				HandlePanic(GetResultHead(jsonObj));
				break;
			case Operations.TransMessage:

			case Operations.SendMessage:
			case Operations.Logout:
			case Operations.Offline:
			case Operations.UpdateUserInfo:
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
		private static void HandleLoginResult(ResultHead head, byte[] data)
		{
			//发起登录完成事件
			if (head.Error != "")
			{
				OnLoginDone?.Invoke(head, null);
				return;
			}
			JObject dataObj = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data));
			User user = new User
			{
				Name = dataObj[AttrNames.User].ToString(),
				NickName = dataObj[AttrNames.NickName].ToString(),
				Motto = dataObj[AttrNames.Motto].ToString(),
				Group = (UserGroup)Enum.Parse(typeof(UserGroup), dataObj[AttrNames.UGroup].ToString())
			};
			OnLoginDone?.Invoke(head, user);
		}
		#endregion

	}

}
