using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer.ship_customer;
using biubiu.view_model.ship_order_manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace biubiu.views.marketing.ship_order_manage
{
    /// <summary>
    /// PrecisionSearchShipOrderDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PrecisionSearchShipOrderDialog : UserControl
    {
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();
        public PrecisionSearchShipOrderDialog()
        {
            InitializeComponent();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as PrecisionSearchShipOrderViewModel;
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

        /// <summary>
        /// 搜索客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchCus(object sender, EventArgs e)
        {
            try
            {
                var dataContext = DataContext as PrecisionSearchShipOrderViewModel;
                //dataContext.GetShipCustomerItems(CustomerComboBox.Text); 修改中
                dataContext.GetCustomer(CustomerSearchListBox.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void ComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as PrecisionSearchShipOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }

        private void CustomerSearchListBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomerSearchListBox.AccountTextBox.Tag == null)
                szmCustomerTimer.Start();
            else
                CustomerSearchListBox.AccountTextBox.Tag = null;
        }

        private void CustomerSearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerSearchListBox.AccountTextBox.Tag = new object();
            if (CustomerSearchListBox.AccountListBox.SelectedItem != null)
                CustomerSearchListBox.AccountTextBox.Text = (CustomerSearchListBox.AccountListBox.SelectedItem as ShipCustomer)?.Name;
            CustomerSearchListBox.AccountPopup.IsOpen = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            #region 搜索框注册
            BindingOperations.SetBinding(CustomerSearchListBox.AccountListBox,ItemsControl.ItemsSourceProperty,
                new Binding {
                    Source = (DataContext as PrecisionSearchShipOrderViewModel),
                    Path = new PropertyPath("ShipCustomerItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CustomerSearchListBox.AccountListBox, Selector.SelectedItemProperty,
                new Binding
                {
                    Source = (DataContext as PrecisionSearchShipOrderViewModel),
                    Path = new PropertyPath("SearchOrder.Customer"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            CustomerSearchListBox.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerSearchListBox_SelectionChanged);
            CustomerSearchListBox.AccountListBox.DisplayMemberPath = "Name";
            CustomerSearchListBox.AccountPopup.IsOpen = false;
            #endregion
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CustomerSearchListBox.AccountTextBox.Text = "";
        }
    }
}
