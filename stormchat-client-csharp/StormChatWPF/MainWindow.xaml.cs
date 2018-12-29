﻿using Interact;
using StormChatWPF.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StormChatWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void LoadFinish(object sender,EventArgs e); 
        public static MainWindow Instence;
        public MainWindow()
        {
            InitializeComponent();
            Instence = this;
            MainWindowLoad += Window_Load;
            MainWindowLoad.Invoke(this,null);
        }
        void Window_Load(object sender, EventArgs e)
        {
            LoadContactsList();
            LoadUserMe();
        }
        private void LoadUserMe()
        {
            BitmapImage image = new BitmapImage();
            User_HeadPicture.BeginInit();
            image.StreamSource = User.Me.Photo;
            User.Me.Photo.Seek(0, SeekOrigin.Begin);
            User_HeadPicture.EndInit();
            User_HeadPicture.Source = image;
            User_NickNmae.Content = User.Me.NickName;
            User_motto.Content = User.Me.Motto;
        }

        private void LoadContactsList()
        {
            if (Chat.ContactsList.Any())
            {
                foreach (var user in Chat.ContactsList)
                {
                    if (user.Name !=User.Me.Name)
                    {
                        UsersList.Items.Add(new UI.ContactsInfo(user)
                        {
                            HorizontalAlignment = HorizontalAlignment.Center
                        });
                    }
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
            if (InputBox.Text != "")
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
                (Action)delegate ()
                {
                    OutBox.Children.Add(new StormChatWPF.UI.ChatBubble(message, HorizontalAlignment.Left));
                });
            }//接受到的的消息
            else
            {
                App.Current.Dispatcher.Invoke(
                    (Action)delegate ()
                {
                    OutBox.Children.Add(new StormChatWPF.UI.ChatBubble(message, HorizontalAlignment.Right));
                });
            }//发送的的消息
        }//将消息展现于UI界面

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(
                (Action)delegate ()
                {
                });
        }

        event LoadFinish MainWindowLoad;
    }
}
