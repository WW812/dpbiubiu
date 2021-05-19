using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
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

namespace biubiu.views
{
    /// <summary>
    /// DefaultPage.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultPage : UserControl
    {
        public DefaultPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!PonderationHelper.GetInstance().Running)
                PonderationHelper.GetInstance().RunPond();
            if (Config.SnapshotPicture is null)
                Config.SnapshotPicture = new Snapshot();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                (Application.Current.MainWindow as MainWindow).JumpPage(btn.Tag.ToString());
            }
        }
    }
}
