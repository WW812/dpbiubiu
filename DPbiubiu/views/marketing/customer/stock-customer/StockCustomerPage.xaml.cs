using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model.customer.stock_customer;
using biubiu.model.stock_order;
using biubiu.view_model.customer.stock_customer;
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
using System.Windows.Threading;

namespace biubiu.views.marketing.customer.stock_customer
{
    /// <summary>
    /// StockCustomerPage.xaml 的交互逻辑
    /// </summary>
    public partial class StockCustomerPage : UserControl
    {
        /// <summary>
        /// 搜索客户计时器，延迟发送
        /// </summary>
        private static DispatcherTimer readDataTimer = new DispatcherTimer();

        public StockCustomerPage()
        {
            InitializeComponent();
            DataContext = new StockCustomerViewModel();
            // 客户计时器，延时发送设置
            readDataTimer.Tick += new EventHandler(SearchCus);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as StockCustomerViewModel;
            datacontext.CurrentCustomerPage = new PageModel();
            datacontext.GetStockCustomerItems();
            SearchCustomerTextBox.Text = "";
            DeletedTogBtn.IsChecked = false;
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

        private void CustomerListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as StockCustomerViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(CustomerListView);
                }
            }
        }

        /// <summary>
        /// 输入框失去焦点时如果内容为空 置零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text)) textBox.Text = "0";
        }

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dataContext = DataContext as StockCustomerViewModel;
            var text = (sender as TextBox).Text;
            dataContext.SearchCusSeed = text;
            readDataTimer.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = !ExportPopup.IsOpen;
        }

        private void SearchCus(object sender, EventArgs e)
        {
            var dataContext = DataContext as StockCustomerViewModel;
            dataContext.GetStockCustomerItems();
            readDataTimer.Stop();
        }

        private void DataGrid_LoadingRow_1(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            StockOrder dataRow = e.Row.Item as StockOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
        }
    }
}
