using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interact
{
	public class ConnectionBrokenException : Exception
	{
		public ConnectionBrokenException(string message) : base(message) { }
	}
}
