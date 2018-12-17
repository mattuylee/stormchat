﻿using Interact;
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
        public static LogWindow a;
        public LogWindow()
        {
            InitializeComponent();
            StormClient.OnLoginDone += Chat.HaveLogin;
            StormClient.OnGetUserListDone += Chat.GetUserList;
            StormClient.OnGetUserPhotoDone += Chat.GetUserPhoto;
            a = this;
        }
        Chat chat = new Chat();
        private void Login_button_Click(object sender, RoutedEventArgs e)
        {
            chat.Log(AccountBox.Text,passwordBox.Password);
        }

    }
}
