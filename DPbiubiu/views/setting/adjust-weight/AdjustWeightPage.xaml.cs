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

namespace biubiu.views.setting.adjust_weight
{
    /// <summary>
    /// Interaction logic for AdjustWeightPage.xaml
    /// </summary>
    public partial class AdjustWeightPage : UserControl
    {
        public AdjustWeightPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            P1.Text = Config.P1.AdjustWeight.ToString();
            P2.Text = Config.P2.AdjustWeight.ToString();
            P3.Text = Config.P3.AdjustWeight.ToString();
            P4.Text = Config.P4.AdjustWeight.ToString();
            TB_Print.Text = Config.SHIP_EXIT_CONFIG.AdjustWeight.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Double.Parse(P1.Text);
                Double.Parse(P2.Text);
                Double.Parse(P3.Text);
                Double.Parse(P4.Text);
            }
            catch(Exception)
            {
                BiuMessageBoxWindows.BiuShow("请输入符合要求的数字! \n (最多支持小数点后两位)", image: BiuMessageBoxImage.Error);
                return;
            }
            try
            {
                MainArea.IsEnabled = false;
                Config.P1.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(P1.Text), 2);
                Config.P1.WriteToFile(new INIClass(Config.DEVICE_CONFIG_PATH), "1Ponderation");
                Config.P2.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(P2.Text), 2);
                Config.P2.WriteToFile(new INIClass(Config.DEVICE_CONFIG_PATH), "2Ponderation");
                Config.P3.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(P3.Text), 2);
                Config.P3.WriteToFile(new INIClass(Config.DEVICE_CONFIG_PATH), "3Ponderation");
                Config.P4.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(P4.Text), 2);
                Config.P4.WriteToFile(new INIClass(Config.DEVICE_CONFIG_PATH), "4Ponderation");
                MainArea.IsEnabled = true;
                SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                MainArea.IsEnabled = true;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Double.Parse(TB_Print.Text);
            }
            catch (Exception)
            {
                BiuMessageBoxWindows.BiuShow("请输入符合要求的数字! \n (最多支持小数点后两位)", image: BiuMessageBoxImage.Error);
                return;
            }

            try
            {
                MainArea.IsEnabled = false;
                Config.STOCK_ENTER_CONFIG.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(TB_Print.Text), 2);
                Config.STOCK_ENTER_CONFIG.WriteToFile(new INIClass(Config.BILL_CONFIG_PATH), "StockEnter");
                Config.STOCK_EXIT_CONFIG.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(TB_Print.Text), 2);
                Config.STOCK_EXIT_CONFIG.WriteToFile(new INIClass(Config.BILL_CONFIG_PATH), "StockExit");
                Config.SHIP_ENTER_CONFIG.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(TB_Print.Text), 2);
                Config.SHIP_ENTER_CONFIG.WriteToFile(new INIClass(Config.BILL_CONFIG_PATH), "ShipEnter");
                Config.SHIP_EXIT_CONFIG.AdjustWeight = Common.Double2DecimalCalculate(Double.Parse(TB_Print.Text), 2);
                Config.SHIP_EXIT_CONFIG.WriteToFile(new INIClass(Config.BILL_CONFIG_PATH), "ShipExit");
                MainArea.IsEnabled = true;
                SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                MainArea.IsEnabled = true;
            }
        }
    }
}
