using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

//StormClient发送数据相关内容
namespace Interact
{
	public partial class StormClient
	{
		// 消息写入线程，向tcp连接写入数据
		private static void SendLoop()
		{
			NetworkStream stream = tcpClient.GetStream();
			try
			{
				foreach (Packet packet in sendQueue.GetConsumingEnumerable())
				{
					//结束循环
					if (StormClient.Status != ClientStatus.Running)
						break;
					//向tcp连接写数据
					byte[] lenBuffer; //数据长度缓存
					byte[] headData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet.Head));
					//写包头
					lenBuffer = BitConverter.GetBytes((UInt32)IPAddress.HostToNetworkOrder(headData.Length));
					stream.Write(lenBuffer, 0, 4);
					stream.Write(headData, 0, headData.Length);
					//写数据
					int length = 0;
					if (packet.Data != null)
						length = packet.Data.Length;
					lenBuffer = BitConverter.GetBytes((UInt32)IPAddress.HostToNetworkOrder(length));
					stream.Write(lenBuffer, 0, 4);
					if (length > 0)
						stream.Write(packet.Data, 0, packet.Data.Length);
					//发送完成回调
					packet.CallBack?.Invoke(packet.Head);
				}
			}
			catch (System.IO.IOException ex)
			{
				HandleConnectionBroken(ex);
			}
			catch (ObjectDisposedException ex)
			{
                HandleConnectionBroken(ex);
			}
		}
		//发送数据
		private static bool Send(Packet packet)
		{
			if (sendLoopThread == null || !sendLoopThread.IsAlive)
				return false;
			//加入数据发送队列等待
			sendQueue.Add(packet);
			return true;
		}
	}
}
