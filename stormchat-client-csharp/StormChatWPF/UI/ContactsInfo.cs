using Interact;
using System.Windows.Media.Imaging;
using System.IO;

namespace StormChatWPF.UI
{
    internal class ContactsInfo : Contacts
    {
        public ContactsInfo(User user)
        {
            image.BeginInit();
            image.StreamSource = user.Photo;
            User.DefaultPhoto.Seek(0, SeekOrigin.Begin);
            image.EndInit();
            HeadPicture.Source = image;
            text.Content = user.NickName;
        }
        BitmapImage image = new BitmapImage();
    }
}
