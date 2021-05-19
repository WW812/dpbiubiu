using biubiu.Domain;
using biubiu.view_model.device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
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

namespace biubiu.views.setting.device
{
    /// <summary>
    /// DevicePage.xaml 的交互逻辑
    /// </summary>
    public partial class DevicePage : UserControl
    {
        public DevicePage()
        {
            InitializeComponent();
            DataContext = new DeviceViewModel();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as DeviceViewModel;
            //datacontext.IsConnectionTest = false;
            datacontext.sPort?.Close();
            PonderationHelper.GetInstance().ReRunPond();
        }

        private void PondListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hikvision1.Logout();
            Hikvision2.Logout();
            Hikvision1.CheckBoxStatus();
            Hikvision2.CheckBoxStatus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PonderationHelper.GetInstance().StopPond();
        }
    }
}
