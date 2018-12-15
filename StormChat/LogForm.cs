using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Interact;

namespace StormChat
{
	public partial class LogForm : Form
	{
		internal static MainForm _mainForm; //主窗口，程序启动时设置
		public LogForm()
		{
			InitializeComponent();
		}

		private void LogForm_Load(object sender, EventArgs e)
		{
			StormClient.OnLoginDone += this.LoginDone;
		}
		private void btnLogin_Click(object sender, EventArgs e)
		{
			if(!StormClient.Initialize())
			{
				MessageBox.Show(this, "连接服务器失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			btnLogin.Enabled = false;
			StormClient.RequestLogin(txtUser.Text, txtPassword.Text);
		}
		
		//登录结果处理
		private void LoginDone(ResultHead head, User user)
		{
			Action f = delegate ()
			{
				btnLogin.Enabled = true;
				if (head.Error != "")
				{
					MessageBox.Show("登录失败：" + head.Error);
					return;
				}
				User.Me = user;
				_mainForm.Opacity = 1;
				_mainForm.Show();
				this.Close();
			};
			//处理跨线程调用
			if (this.InvokeRequired)
				this.Invoke(f);
			else
				f.Invoke();
		}
	}
}
