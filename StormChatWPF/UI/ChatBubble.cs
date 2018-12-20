using Interact;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StormChatWPF
{
    /// <summary>
    /// 聊天气泡
    /// </summary>
    class ChatBubble:Label
    {
        TextBlock text = new TextBlock()
        {
            MaxWidth = 400,
            MinWidth = 10,
            MinHeight = 20,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Comic Sans MS"),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">传入消息体</param>
        /// <param name="alignment">设置水平对齐方式，参数为HorizontalAlignment枚举类型</param>
        public ChatBubble(Message msg,HorizontalAlignment alignment)
        {
            Background = new ImageBrush()
            {
                ImageSource=new BitmapImage(new Uri("pack://application..../UI/Resource/聊天气泡.png"))
            };
            HorizontalAlignment = alignment;
            text.Text = msg.Text;
            Content = text;
        }
    }
}
