using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
using biubiu.view_model.ship_order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace biubiu.views.marketing.ship_order
{
    /// <summary>
    /// ShipOrderMendPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShipOrderMendPage : UserControl
    {
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        public ShipOrderMendPage()
        {
            InitializeComponent();
            DataContext = new ShipOrderMendViewModel();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);

            #region 搜索框的注册
            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
               new Binding
               {
                   Source = (DataContext as ShipOrderMendViewModel),
                   Path = new PropertyPath("CustomerItems"),
                   UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
               });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, Selector.SelectedItemProperty,
                new Binding
                {
                    Source = (DataContext as ShipOrderMendViewModel),
                    Path = new PropertyPath("Order.Customer"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            CustomerComboBox2.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerComboBox2_SelectionChanged);
            CustomerComboBox2.AccountListBox.DisplayMemberPath = "Name";
            #endregion
        }

        private void CustomerComboBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomerComboBox2.AccountTextBox.Tag == null)
                szmCustomerTimer.Start();
            else
                CustomerComboBox2.AccountTextBox.Tag = null;
        }

        /// <summary>
        /// 选择客户ComboBox SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = (DataContext as ShipOrderMendViewModel);
            CustomerComboBox2.AccountTextBox.Tag = new object();
            if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as ShipCustomer)?.Name;
            dataContext.SetPriceByGoods(0);
            CustomerTogBtn.IsChecked = true;
            CustomerComboBox2.AccountPopup.IsOpen = false;
        }

        /// <summary>
        /// 限制输入非负实数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

        /// <summary>
        /// TextBox输入变换响应
        /// 刷新计算使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var datacontext = DataContext as ShipOrderMendViewModel;
            var textBox = sender as TextBox;
            var text = textBox.Text;
            if (text == "" || text.Substring(text.Length - 1, 1).Equals(".")
                || Regex.IsMatch(text, Config.REGEXSTR_00)) return;
            switch (textBox.Name)
            {
                case "CarGrossWeightTextBox":
                    datacontext.Order.CarGrossWeight = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "CarTareTextBox":
                    datacontext.Order.CarTare = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "GoodsRealPriceTextBox":
                    datacontext.Order.GoodsRealPrice = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "DiscountMoneyTextBox":
                    datacontext.Order.DiscountMoney = Convert.ToDouble(text);
                    datacontext.Calculate(1);
                    break;
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
            textBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var textBox = sender as TextBox;
                textBox.SelectAll();
                textBox.PreviewMouseDown -= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 禁用输入框的复制、剪切、粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Paste)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 选择客户ComboBox SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            var dataContext = (DataContext as ShipOrderMendViewModel);
            dataContext.SetPriceByGoods(0);
            CustomerTogBtn.IsChecked = true;
            */
        }

        private void CustomerComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as ShipOrderMendViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }

        /// <summary>
        /// 料品选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!((sender as ComboBox).SelectedItem is ShipGoods goods)) return;
            var datacontext = DataContext as ShipOrderMendViewModel;
            if (datacontext.Order.Customer != null) { datacontext.SetPriceByGoods(); }
            else
            {
                datacontext.Order.GoodsPrice = goods.Price;
                datacontext.Order.GoodsRealPrice = goods.RealPrice;
                datacontext.Calculate(0);
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as ShipOrderMendViewModel).SetPriceByGoods();
        }

        /// <summary>
        /// 零售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ShipOrderMendViewModel;
            CustomerComboBox2.AccountListBox.SelectedItem = null;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
            szmCustomerTimer.Start();
            dataContext.Order.CustomerType = 0;
            dataContext.SetPriceByGoods(0);
        }

        /// <summary>
        /// 客户下拉框文本输入，用于查找客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
            if (CustomerComboBox.Text == null ||
                (!CustomerComboBox.Text.Equals((CustomerComboBox?.SelectedItem as ShipCustomer)?.Name ?? "")))
            {
                CustomerComboBox.IsDropDownOpen = true;
                szmCustomerTimer.Start();
            }
            */
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
                var dataContext = DataContext as ShipOrderMendViewModel;
                dataContext.GetShipCustomerItems(CustomerComboBox2.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }
    }
}
