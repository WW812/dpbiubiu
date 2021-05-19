using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer.ship_customer;
using biubiu.view_model.accept;
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

namespace biubiu.views.finance.accept
{
    /// <summary>
    /// AcceptPage.xaml 的交互逻辑
    /// </summary>
    public partial class AcceptPage : Page
    {
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        private bool _r = true;

        public AcceptPage()
        {
            InitializeComponent();
            DataContext = new AcceptViewModel();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);

            #region 客户搜索框
            BindingOperations.SetBinding(CustomerSearchListBox.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = (DataContext as AcceptViewModel),
                    Path = new PropertyPath("CustomerItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CustomerSearchListBox.AccountListBox, Selector.SelectedItemProperty,
                new Binding
                {
                    Source = (DataContext as AcceptViewModel),
                    Path = new PropertyPath("AcceptCustomerFeed"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            CustomerSearchListBox.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerSearchListBox_SelectionChanged);
            CustomerSearchListBox.AccountListBox.DisplayMemberPath = "Name";
            #endregion
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
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
            if (CustomerSearchListBox.AccountListBox.SelectedItem == null) return;
            CustomerSearchListBox.AccountTextBox.Text = (CustomerSearchListBox.AccountListBox.SelectedItem as ShipCustomer).Name;
            CustomerSearchListBox.AccountPopup.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as AcceptViewModel;
            AcceptNumTextBox.Text = "";
            AcceptMoneyTextBox.Text = "";
            CustomerSearchListBox.AccountTextBox.Text = "";
            CustomerSearchListBox.AccountListBox.SelectedItem = null;
            dataContext.AcceptCustomerFeed = null;
            dataContext.AcceptMoneyFeed = null;
            dataContext.AcceptNumFeed = null;
            CustomerSearchListBox.AccountPopup.IsOpen = false;
            dataContext.CurrentPage.Reset();
            dataContext.GetAcceptItems();
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
                var dataContext = DataContext as AcceptViewModel;
                //dataContext.GetShipCustomerItems(CustomerComboBox.Text); 修改中
                dataContext.GetCustomerItems(CustomerSearchListBox.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (_r) { _r = false; return; }
            try
            {
                var dataContext = DataContext as AcceptViewModel;
                if (StatusTreeView.SelectedItem == null) return;
                if ("-1" == (StatusTreeView.SelectedItem as TreeViewItem).Tag.ToString())
                    dataContext.SelectedAcceptStatus = null;
                else
                    dataContext.SelectedAcceptStatus = System.Convert.ToInt32((StatusTreeView.SelectedItem as TreeViewItem).Tag);
                dataContext.GetAcceptItems();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AcceptMoneyTextBox.Text) && !RegexChecksum.IsReal(AcceptMoneyTextBox.Text))
            {
                BiuMessageBoxWindows.BiuShow("若要根据金额查询，请输入数字!");
                return;
            }
            var dataContext = DataContext as AcceptViewModel;
            dataContext.AcceptCustomerFeed = CustomerSearchListBox.AccountListBox.SelectedItem as ShipCustomer;
            dataContext.AcceptNumFeed = string.IsNullOrWhiteSpace(AcceptNumTextBox.Text) ? null : AcceptNumTextBox.Text;
            if (string.IsNullOrWhiteSpace(AcceptMoneyTextBox.Text))
            {
                dataContext.AcceptMoneyFeed = null;
            }
            else
            {
                dataContext.AcceptMoneyFeed = System.Convert.ToDouble(AcceptMoneyTextBox.Text);
            }
            dataContext.CurrentPage.Reset();
            dataContext.GetAcceptItems();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as AcceptViewModel;
            (StatusTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsSelected = true;
            dataContext.CurrentPage.Reset();
            dataContext.GetAcceptItems();
        }
    }
}
