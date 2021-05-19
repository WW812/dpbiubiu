using biubiu.Domain;
using biubiu.model.ship_order;
using biubiu.view_model.ship_order_manage;
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

namespace biubiu.views.marketing.ship_order_manage
{
    /// <summary>
    /// ShipOrderManage.xaml 的交互逻辑
    /// </summary>
    public partial class ShipOrderManage : UserControl
    {
        public ShipOrderManage()
        {
            InitializeComponent();
            DataContext = new ShipOrderManageViewModel();
            SimpleDatePicker.Language = System.Windows.Markup.XmlLanguage.GetLanguage("zh-CN");
        }

        private void SimpleDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SimpleDatePicker.SelectedDate == null) return;
            var datacontext = DataContext as ShipOrderManageViewModel;
            datacontext.SearchOrder.ExitTime = Common.DateTime2TimeStamp(SimpleDatePicker.SelectedDate ?? new DateTime());
            datacontext.SearchOrder.ExitTimeEnd = datacontext.SearchOrder.ExitTime + 86400000;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SimpleDatePicker.SelectedDate = null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as ShipOrderManageViewModel;
            datacontext.SearchOrder.Reset();
            datacontext.CurrentPage.Reset();
            SimpleDatePicker.SelectedDate = null;
            datacontext.GetOrders(datacontext.SearchOrder);
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            ShipOrder dataRow = e.Row.Item as ShipOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
        }

        private void ExportOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = !ExportPopup.IsOpen;
        }
    }
}
