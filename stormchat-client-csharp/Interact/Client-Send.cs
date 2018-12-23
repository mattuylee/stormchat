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
			Packet packet = null;
			NetworkStream stream = tcpClient.GetStream();
			//json序列化时忽略值为null的项
			JsonSerializerSettings setting = new JsonSerializerSettings();
			setting.NullValueHandling = NullValueHandling.Ignore;
			try
			{
				//阻塞式的从发送队列中取出数据
				foreach (Packet i in sendQueue.GetConsumingEnumerable())
				{
					packet = i;  //为了在代码块外引用i
					//结束循环
					if (StormClient.Status != ClientStatus.Running)
						break;
					//向tcp连接写数据
					byte[] lenBuffer; //数据长度缓存
					byte[] headData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet.Head, setting));
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
					//发送成功回调
					ResultHead result = new ResultHead
					{
						Token = packet.Head.Token,
						Operation = packet.Head.Operation,
						Error = ""
					};
					packet.CallBack?.Invoke(result);
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
			finally
			{
				List<Packet> packets = new List<Packet>(sendQueue.Count + 1);
				if (packet != null)
					packets.Add(packet);
				while (sendQueue.TryTake(out packet))
				{
					if (packet != null)
						packets.Add(packet);
				}
				foreach (Packet p in packets)
				{
					//发送失败回调
					ResultHead result = new ResultHead
					{
						Token = packet.Head.Token,
						Operation = packet.Head.Operation,
						Error = "Client has stopped running."
					};
					packet.CallBack?.Invoke(result);
				}
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
