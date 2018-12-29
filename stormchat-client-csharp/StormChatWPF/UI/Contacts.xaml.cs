using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StormChatWPF.UI
{
    /// <summary>
    /// Contacts.xaml 的交互逻辑
    /// </summary>
    public partial class Contacts : UserControl
    {
        public Contacts()
        {
            InitializeComponent();
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
        }
        
        public Image Head
        {
            get
            {
                return this.HeadPicture;
            }
            set
            {
                this.HeadPicture = value;
            }
        }
    }
}
