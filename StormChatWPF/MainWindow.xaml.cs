using Interact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StormChatWPF
{
    /// <summary>
    /// UserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instence;
        public MainWindow()
        {
            InitializeComponent();
            LoadContactsList();
            Instence = this;
        }
        private void LoadContactsList()
        {
            if (Chat.ContactsList.Any())
            {
                foreach (var user in Chat.ContactsList)
                {
                    UsersList.Items.Add(user.NickName);
                }
            }
        }//向联系人列表listview中添加元素

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersList.SelectedItem != null)
            {
                Chat.CurrentContact = Chat.ContactsList[UsersList.SelectedIndex];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Chat.CurrentContact == null)
            {
                MessageBox.Show("请选择联系人！");
                return;
            }
            if (InputBox.Text!="")
            {
                Chat.chat.SendMessage(InputBox.Text, Chat.CurrentContact);
                InputBox.Text = "";
            }
        }

        internal void ShowMessage(Message message)
        {
            if (message.To == User.Me)
            {
                App.Current.Dispatcher.Invoke(
                delegate ()
                {
                    OutBox.Children.Add(new ChatBubble(message,HorizontalAlignment.Left));
                });
            }//接受到的的消息
            else
            {
                App.Current.Dispatcher.Invoke(
                delegate ()
                {
                    OutBox.Children.Add(new ChatBubble(message,HorizontalAlignment.Right));
                });
            }//发送的的消息
        }//将消息展现于UI界面
    }
}
