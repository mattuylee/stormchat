using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Media;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace StormChatWPF
{
    class HeadPicture : Image
    {
        public HeadPicture(/*Stream imgstream*/)
        {

            FileStream fileStream = new FileStream(@"C:\Users\Administrator\Desktop\闪电.png", FileMode.Open);
            
            Source = new BitmapImage()
            {
                StreamSource = fileStream
            };
            //border.Content = this;
        }
        private Label border = new Label()
        {
            
        };
    }
}
