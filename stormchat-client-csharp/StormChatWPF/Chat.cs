using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interact;
using System.Drawing;
using System.Windows;
using System.IO;

namespace StormChatWPF
{
    class Chat
    {
        public static List<User> ContactsList;//联系人集合
        public static Chat chat=new Chat();
        private Chat()
        {
            StormClient.OnLoginDone += OnHaveLogin;
            StormClient.OnGetUserListDone += OnGetContactsList;
            StormClient.OnMessage +=OnMessage;//事件挂接
            User.DefaultPhoto = new MemoryStream(File.ReadAllBytes(@"../../UI/Resources/默认头像.png"));
        }
        internal static User CurrentContact { get; set; }//设置当前联系人对象
        /// <summary>
        /// 获取联系人列表(完成后打开主窗体)
        /// </summary>
        private void OnGetContactsList(ResultHead head, User[] users)
        {
            if (head.Error != "")
            {
                MessageBox.Show("无法获取联系人列表");
                return;
            }
            App.Current.Dispatcher.Invoke(
                (Action)delegate ()
                {
                    ContactsList = new List<User>(users);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    LogWindow.Instence.Close();
                });
        }//获取联系人列表完成
        private  void OnHaveLogin(ResultHead head, User user)
        {
                    if (head.Error == "")
                    {
                        User.Me = user;
                        StormClient.QueueGetUserList();                       
                    }
                    else
                    {
                App.Current.Dispatcher.Invoke(
                    (Action)delegate ()
                    {
                        LogWindow.Instence.Login_button.IsEnabled = true;
                    });
                        MessageBox.Show("请核对账号密码！");
                    }
        }//登录完成
        private  void OnMessage(Message message)
        {
            if (MainWindow.Instence != null)
            {
                MainWindow.Instence.ShowMessage(message);
            }
        }//接受到新消息

        internal void SendMessage(string text,User target)
        {
            Message msg = new Message(text,target);
            Action<ResultHead> f = delegate (ResultHead head)
           {
               MainWindow.Instence.ShowMessage(msg);
           };
            StormClient.QueueSendMessage(msg,f);
        }//发送消息
        internal static void Log(string user, string password)
        {
            if (StormClient.Initialize())
            {
                if (!StormClient.QueueLogin(user, password))
                {
                    App.Current.Dispatcher.Invoke(
                    (Action)delegate ()
                    {
                        LogWindow.Instence.Login_button.IsEnabled = true;
                    }); 
                    MessageBox.Show("登录失败");
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(
                    (Action)delegate ()
                    {
                        LogWindow.Instence.Login_button.IsEnabled = true;
                    });
                MessageBox.Show("连接服务器失败！");
            }
        }//登录
        
    }
}
