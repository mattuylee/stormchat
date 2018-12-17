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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StormClient.OnLoginDone += HaveLogin;
        }

        private void Login_button_Click(object sender, RoutedEventArgs e)
        {
            if (StormClient.Initialize())
            {
                StormClient.QueueLogin(AccountBox.Text, passwordBox.Password, null);
            }
            else
            {
                MessageBox.Show("连接服务器失败！");
            }
        }

        private void HaveLogin(ResultHead head, User user)
        {
           App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if (head.Error == "")
                {
                    User.Me = user;
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("登录失败！");
                }
            });
        }
    }
}
