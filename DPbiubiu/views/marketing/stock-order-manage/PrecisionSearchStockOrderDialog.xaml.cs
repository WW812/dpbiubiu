using biubiu.Domain;
using biubiu.view_model.stock_order_manage;
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

namespace biubiu.views.marketing.stock_order_manage
{
    /// <summary>
    /// PrecisionSearchStockOrderDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PrecisionSearchStockOrderDialog : UserControl
    {
        public PrecisionSearchStockOrderDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as PrecisionSearchStockOrderViewModel;
            if (DateStart.SelectedDate != null)
            {
                var ds = DateStart.SelectedDate ?? new DateTime();
                var ts = TimeStart.SelectedTime ?? new DateTime(1970, 1, 1, 0, 0, 0);
                datacontext.SearchOrder.ExitTime = Common.DateTime2TimeStamp(new DateTime(
                    ds.Year, ds.Month, ds.Day, ts.Hour, ts.Minute, 0
                    ));
            }
            if (DateEnd.SelectedDate != null)
            {
                var de = DateEnd.SelectedDate ?? new DateTime();
                var te = TimeEnd.SelectedTime ?? new DateTime(1970, 1, 1, 23, 59, 59);
                datacontext.SearchOrder.ExitTimeEnd = Common.DateTime2TimeStamp(new DateTime(
                    de.Year, de.Month, de.Day, te.Hour, te.Minute, 59
                    ));
            }
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
        }

        private void ComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as PrecisionSearchStockOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }
    }
}
