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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StormChatWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogWindow : Window
    {
        public static LogWindow Instence;
        public LogWindow()
        {
            InitializeComponent();
            Instence = this;
        }
        private void Login_button_Click(object sender, RoutedEventArgs e)
        {
            Chat.chat.Log(AccountBox.Text,passwordBox.Password);
        }
        internal void ShowMessage(Message message)
        {

        }//将收到的消息展现于UI界面
        internal void ShowUserMessage()
        {
        }//展现自己发出的消息

    }
}
