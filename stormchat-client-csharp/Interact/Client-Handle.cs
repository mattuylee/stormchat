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
		//处理连接中断异常。
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
		private static void HandleAll(byte[] head, byte[] data)
		{
			string headStr = Encoding.UTF8.GetString(head);
			JObject jsonObj = (JObject)JsonConvert.DeserializeObject(headStr);
			switch (jsonObj[AttrNames.Operation].ToString())
			{
			case Operations.Panic:
				HandlePanic(GetResultHead(jsonObj));
				break;
			case Operations.Login:
				HandleLoginResult(jsonObj, data);
				break;
			case Operations.TransMessage:
				HandleMessage(jsonObj, data);
				break;
			case Operations.SendMessage:
				HandleSendMessageResult(GetResultHead(jsonObj));
				break;
			case Operations.Offline:
				HandlePanic(GetResultHead(jsonObj));
				break;
			case Operations.UpdateUserInfo:
				HandleUpdateUserInfoDone(GetResultHead(jsonObj));
				break;
			case Operations.GetUsers:
				HandleGetUsersPack(jsonObj, data);
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
			};
			if (head[AttrNames.Error] != null)
				resultHead.Error = head[AttrNames.Error].ToString();
			return resultHead;
		}

		//处理服务器异常断开连接消息
		private static void HandlePanic(ResultHead head)
		{
			SetStatus(ClientStatus.Stopped);
			OnPanic?.Invoke(head);
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

		//收到消息
		private static void HandleMessage(JObject head, byte[] data)
		{
			//提取结果包头
			ResultHead resultHead = GetResultHead(head);
			Message message = new Message()
			{
				When = DateTime.Parse(head[AttrNames.When].ToString()),
				From = User.FromUserName(head[AttrNames.From].ToString()),
				To = User.Me,
				Text = Encoding.UTF8.GetString(data)
			};
			OnMessage?.Invoke(message);
		}

		//发送消息执行结果
		private static void HandleSendMessageResult(ResultHead head)
		{
			OnSendMessageDone?.Invoke(head);
		}

		//处理掉线、强制登出
		private static void HandleOffiline(ResultHead head)
		{
			SetStatus(ClientStatus.Stopped);
			OnOffline?.Invoke(head);
		}

		//处理更改信息结果
		private static void HandleUpdateUserInfoDone(ResultHead head)
		{
			OnUpdateUserInfoDone?.Invoke(head);
		}

		//处理获取用户列表的不连续返回包。当所有包接收完毕后返回给客户
		private static void HandleGetUsersPack(JObject head, byte[] data)
		{
			ResultHead result = GetResultHead(head);
			if (head[AttrNames.Error].ToString() != string.Empty)
			{
				OnGetUserListDone?.Invoke(result, new User[0]);
				return;
			} //获取失败
			else if (int.Parse(head[AttrNames.Count].ToString()) <= 0)
			{
				User[] us;
				if (workingDictionary.ContainsKey(result.Token))
					us = workingDictionary[result.Token] as User[];
				else
					us = new User[0];
				User.Users = us;
				OnGetUserListDone?.Invoke(result, us);
				return;
			} //获取完成
			//获取用户数据
			User user = new User
			{
				Name = head[AttrNames.User].ToString(),
				NickName = head[AttrNames.NickName].ToString(),
				Motto = head[AttrNames.Motto].ToString(),
				Group = (UserGroup)Enum.Parse(typeof(UserGroup), head[AttrNames.UGroup].ToString()),
				Photo = User.DefaultPhoto
			}; //基础数据
			if (int.Parse(head[AttrNames.Photo].ToString()) > 0)
			{
				MemoryStream ms = new MemoryStream(data);
				user.Photo = Image.FromStream(ms);
			} //头像数据
			//加入临时用户列表缓存
			User[] users;
			int total = int.Parse(head[AttrNames.Total].ToString()); //总用户数
			int count = int.Parse(head[AttrNames.Count].ToString()); //已接收用户数
			if (count > total)
				return;
			if (workingDictionary.ContainsKey(head[AttrNames.Token].ToString()))
				users = workingDictionary[result.Token] as User[];
			else
				users = new User[total];
			users[count - 1] = user;
			workingDictionary[result.Token] = users;
		}
		#endregion

	}

}
