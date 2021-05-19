using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer.ship_customer;
using biubiu.view_model.customer.ship_customer;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace biubiu.views.marketing.customer.ship_customer
{
    /// <summary>
    /// ShipCustomerStatementWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShipCustomerStatementWindow : Window
    {
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        public ShipCustomerStatementWindow()
        {
            InitializeComponent();
            DataContext = new ShipCustomerStatementViewModel();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0,0,0,0, Config.REQUEST_DATETIME);
        }

        private void CustomerComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as ShipCustomerStatementViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }

        /// <summary>
        /// 自动生成行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            DataGridRow dataGridRow = e.Row;
            ShipCustomerMoney dataRow = e.Row.Item as ShipCustomerMoney;
            if (dataRow.Deleted == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
            else
            {
                dataGridRow.Foreground = Brushes.Black;
            }
        }

        private void PayTypeComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as ShipCustomerStatementViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPayTypeItems(sender as ComboBox);
                }
            }
        }

        private void CustomerComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomerComboBox2.AccountTextBox.Tag == null)
                szmCustomerTimer.Start();
            else
                CustomerComboBox2.AccountTextBox.Tag = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 客户搜索框
            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding {
                    Source = (DataContext as ShipCustomerStatementViewModel),
                    Path = new PropertyPath("ShipCustomerItems")
                });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, Selector.SelectedItemProperty,
                new Binding {
                    Source = (DataContext as ShipCustomerStatementViewModel),
                    Path = new PropertyPath("CurrentShipCustomer"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            CustomerComboBox2.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerComboBox2_SelectionChanged);
            CustomerComboBox2.AccountListBox.DisplayMemberPath = "Name";
            #endregion
        }

        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerComboBox2.AccountTextBox.Tag = new object();
            if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as ShipCustomer)?.Name;
            CustomerComboBox2.AccountPopup.IsOpen = false;
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
                var dataContext = DataContext as ShipCustomerStatementViewModel;
                //dataContext.GetShipCustomerItems(CustomerComboBox.Text); 修改中
                dataContext.GetShipCustomerItems(CustomerComboBox2.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomerComboBox2.AccountListBox.SelectedItem = null;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
        }
    }
}
