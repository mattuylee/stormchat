using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interact;
using System.Drawing;
using System.Windows;

namespace StormChatWPF
{
    class Chat
    {
        public static User[] userlist;//联系人集合
        public static User ChatNow = new User();//当前聊天对象
        
        internal static void GetUserList(ResultHead head, User[] users)
        {
            if (head.Error != "")
            {
                MessageBox.Show("无法获取联系人列表");
            }
            else
                userlist = users;
        }//获取联系人用户列表       
        internal void Log(string user,string password)
        {
            if (StormClient.Initialize())
            {
                StormClient.QueueLogin(user,password);
            }
            else
            {
                MessageBox.Show("连接服务器失败！");
            }
        }//登录
        internal static void Reconnect(Exception e)
        {
            StormClient.Initialize();
        }//尝试重连接一次，如果失败，则关闭mainwindow，打开登录窗口
        internal  static void HaveLogin(ResultHead head, User user)
        {
            App.Current.Dispatcher.Invoke(
                (Action)delegate ()
                {
                    if (head.Error == "")
                    {
                        User.Me = user;
                        MainWindow userWindow = new MainWindow();
                        userWindow.Show();
                        LogWindow.entry.Close();
                    }
                    else
                    {
                        MessageBox.Show("登录失败！");
                    }
                });
        }//登录完成


    }
}
