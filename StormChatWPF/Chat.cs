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
        public static List<User> ContactsList;//联系人集合
        public  static  Chat chat = new Chat();
        private Chat()
        {
            StormClient.OnLoginDone += HaveLogin;
            StormClient.OnGetUserListDone += GetContactsList;
            StormClient.OnMessage +=ReciveMessage;
        }


        private User ChatNow;
        internal User SetContact
        {
            get
            {
                return ChatNow;
            }
            set
            {
                ChatNow = value;
            }
        }//设置当前联系人对象
        internal static void GetContactsList(ResultHead head, User[] users)
        {
            if (head.Error != "")
            {
                MessageBox.Show("无法获取联系人列表");
            }
            else
                ContactsList =new List<User>( users);
                return;
        }//获取联系人列表
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
                        LogWindow.Instence.Close();
                    }
                    else
                    {
                        MessageBox.Show("登录失败！");
                    }
                });
        }//登录完成
        internal static void  ReciveMessage(Message message)
        {
            LogWindow.Instence.ShowMessage(message);
        }//接受到新消息的时候
        internal static void SendMessage(string text,User target)
        {
            //    Message msg = new Message(text);
            //    Action<BaseHead> f  = delegate(BaseHead head) 
            //    {

            //    };
            //    StormClient.QueueSendMessage(msg, target, f);
            //    LogWindow.Instence.ShowUserMessage();
        }
    }
}
