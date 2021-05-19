using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.ship_order;
using biubiu.view_model.customer.ship_customer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace biubiu.views.marketing.ship_customer
{
    /// <summary>
    /// ShipCustomerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShipCustomerPage : UserControl
    {
        /// <summary>
        /// 搜索客户计时器，延迟发送
        /// </summary>
        private static readonly DispatcherTimer readDataTimer = new DispatcherTimer();

        public ShipCustomerPage()
        {
            InitializeComponent();
            DataContext = new ShipCustomerViewModel();
            // 客户计时器，延时发送设置
            readDataTimer.Tick += new EventHandler(SearchCus);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
        }

        /// <summary>
        /// 自动生成行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as ShipCustomerViewModel;
            datacontext.GetShipCustomerItems();
            SearchCustomerTextBox.Text = "";
            DeletedTogBtn.IsChecked = false;
        }

        /// <summary>
        /// 滚动分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as ShipCustomerViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(CustomerListView);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = !ExportPopup.IsOpen;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dataContext = DataContext as ShipCustomerViewModel;
            var text = (sender as TextBox).Text;
            dataContext.SearchCusSeed = text;
            readDataTimer.Start();
        }

        private void SearchCus(object sender, EventArgs e)
        {
            var dataContext = DataContext as ShipCustomerViewModel;
            dataContext.GetShipCustomerItems();
            readDataTimer.Stop();
        }

        private void DataGrid_LoadingRow_1(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            ShipOrder dataRow = e.Row.Item as ShipOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            } 
        }

        private void SearchCustomerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            var textBox = (TextBox)sender;
            if (e.Key == Key.Enter)
                SearchButton.Command.Execute(textBox.Text);
                */
        }

        private void SearchCustomerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox).Text;
            if (text.Equals(""))
                readDataTimer.Start();
        }

        private void SettleButton_Click(object sender, RoutedEventArgs e)
        {
            SettlePopup.IsOpen = !SettlePopup.IsOpen;
        }

        // 取消结算Popup的显示
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SettlePopup.IsOpen = false;
        }

        private void ExportPopup_Closed(object sender, EventArgs e)
        {
            StartDate.SelectedDate = null;
            EndtDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndTime.SelectedTime = null;
        }
    }
}
