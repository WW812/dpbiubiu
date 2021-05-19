using System;
using System.Collections.Generic;
using System.IO;
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

namespace biubiu.views.CommonUI
{
    /// <summary>
    /// Interaction logic for WatchBigImgWindow.xaml
    /// </summary>
    public partial class WatchBigImgWindow : Window
    {
        public WatchBigImgWindow(string uri)
        {
            if (string.IsNullOrEmpty(uri)) Close();
            InitializeComponent();
            var bitmap = new BitmapImage()
            {
                CreateOptions = BitmapCreateOptions.DelayCreation,
            };

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            Img_Shot.Source = bitmap;
        }

        protected override void OnClosed(EventArgs e)
        {
            //Img_Shot.Source = null; 
        }
    }
}
