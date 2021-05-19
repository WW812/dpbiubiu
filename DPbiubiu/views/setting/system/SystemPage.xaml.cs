using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.system;
using biubiu.view_model.system;
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
using static biubiu.model.ModelHelper;

namespace biubiu.views.setting.system
{
    /// <summary>
    /// SystemPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemPage : UserControl
    {
        public SystemPage()
        {
            InitializeComponent();
            DataContext = new SystemPageViewModel();
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainArea.IsEnabled = false;
                var r = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.SetSystemSetting,
                    new { ID = Config.SYSTEM_SETTING.ID, ShipOrderDiscount = System.Convert.ToInt32((sender as RadioButton).Tag) },
                    delegate (DataInfo<SystemModel> result)
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("出料优惠保存成功!");
                    });
                Config.SYSTEM_SETTING = r.Data;
                MainArea.IsEnabled = true;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                MainArea.IsEnabled = true;
            }
        }

        private async void CusRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainArea.IsEnabled = false;
                var r = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.SetSystemSetting,
                    new { ID = Config.SYSTEM_SETTING.ID, CustomerDiscount = System.Convert.ToInt32((sender as RadioButton).Tag) },
                    delegate (DataInfo<SystemModel> result)
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("出料优惠保存成功!");
                    });
                Config.SYSTEM_SETTING = r.Data;
                MainArea.IsEnabled = true;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                MainArea.IsEnabled = true;
            }
        }

        private void SetShipOrdeDiscountrControl()
        {
            switch (Config.SYSTEM_SETTING.ShipOrderDiscount)
            {
                case 0:
                    ShipOrderDiscountRdioBtn1.IsChecked = true;
                    break;
                case 1:
                    ShipOrderDiscountRdioBtn2.IsChecked = true;
                    break;
                case 2:
                    ShipOrderDiscountRdioBtn3.IsChecked = true;
                    break;
                case 3:
                    ShipOrderDiscountRdioBtn4.IsChecked = true;
                    break;
                default:
                    break;
            }

            switch (Config.SYSTEM_SETTING.CustomerDiscount)
            {
                case 0:
                    ShipOrderDiscountCusRdioBtn1.IsChecked = true;
                    break;
                case 1:
                    ShipOrderDiscountCusRdioBtn2.IsChecked = true;
                    break;
                case 2:
                    ShipOrderDiscountCusRdioBtn3.IsChecked = true;
                    break;
                case 3:
                    ShipOrderDiscountCusRdioBtn4.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetShipOrdeDiscountrControl();
            PhoneTextBox.Text = Config.SYSTEM_SETTING.WeightHomePhone;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainArea.IsEnabled = false;
                var r = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.SetSystemSetting,
                    new { ID = Config.SYSTEM_SETTING.ID, WeightHomePhone = PhoneTextBox.Text },
                    delegate (DataInfo<SystemModel> result)
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("磅房电话(短信)保存成功!");
                    });
                Config.SYSTEM_SETTING = r.Data;
                MainArea.IsEnabled = true;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                MainArea.IsEnabled = true;
            }
        }

        
    }
}
