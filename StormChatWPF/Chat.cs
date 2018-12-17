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
        public static List<User> userlist = new List<User>();//联系人集合

        
        internal static void GetUsers(ResultHandler head, User user)
        {

        }//获取联系人对象
        internal static void GetUserList(ResultHead head, User[] users)
        {
            if (head.Error != "")
            {
                MessageBox.Show("无法获取联系人列表");
            }
            else
                return;
        }//获取用户列表
        internal static void GetUserPhoto(ResultHead head, Image image)
        {
            if (head.Error != "")
            {
                MessageBox.Show("无法读取用户图片");
            }   
        }//对用户头像进行赋值
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
                        LogWindow.a.Close();
                    }
                    else
                    {
                        MessageBox.Show("登录失败！");
                    }
                });
        }//登录完成


    }
}
