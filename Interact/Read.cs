using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Interact
{
	//读取数据相关内容
	public partial class StormClient
	{
		// 数据读取线程，从tcp连接读取数据
		private static void ReadLoop()
		{
			NetworkStream stream = StormClient.tcpClient.GetStream();
			try
			{
				while (tcpClient.Connected)
				{
					byte[] head = ReadDataWithLength(stream);
					byte[] data = ReadDataWithLength(stream);
					HandleAll(head, data);
				}
			}
			catch (System.IO.IOException ex)
			{
				tcpClient.Close();
				OnDisconnect(ex);
			}
			catch (ObjectDisposedException ex)
			{
				tcpClient.Close();
				OnDisconnect(ex);
			}
		}
		// 从网络流读取数据
		private static byte[] ReadDataWithLength(NetworkStream stream)
		{
			byte[] lenBuffer = new byte[4]; //数据长度
			//获取数据长度
			stream.Read(lenBuffer, 0, 4);
			int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenBuffer, 0));
			if (length <= 0)
				return null;
			//读取数据
			byte[] data = new byte[length];
			stream.Read(data, 0, length);
			return data;
		}
	}
}
