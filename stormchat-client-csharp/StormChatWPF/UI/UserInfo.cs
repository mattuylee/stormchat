using Interact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace StormChatWPF
{
    class UserInfo:Label
    {
        public UserInfo(User user)
            {
                
                
            }
        private Image headpicture = new Image
        {
            HorizontalAlignment = HorizontalAlignment.Left
        };
        private TextBlock UserNmae=new TextBlock
        {
            FontFamily = new FontFamily("Comic Sans MS"),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment=VerticalAlignment.Top
        };
    }
}
