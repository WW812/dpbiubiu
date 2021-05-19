using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.paytype;
using biubiu.view_model.customer.ship_customer;
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

namespace biubiu.views.marketing.customer.ship_customer
{
    /// <summary>
    /// SetShipCustomerMoneyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SetShipCustomerMoneyDialog : UserControl
    {
        public SetShipCustomerMoneyDialog()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text == "-" && !textBox.Text.Contains("-")) return;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PayDate.SelectedDate = DateTime.Now;
            PayTime.SelectedTime = DateTime.Now;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as SetShipCustomerMoneyViewModel;
            if (datacontext.CustomerMoney.PayType == 1 && !RegexChecksum.IsReal(HonourDiscountTextBox.Text)) { BiuMessageBoxWindows.BiuShow("承兑贴现扣款请输入数字!"); return; }
            if (datacontext.CustomerMoney.PayType == 1 && HonourDate.SelectedDate == null) { BiuMessageBoxWindows.BiuShow("请选择承兑到期日!"); return; }
            if (datacontext.CustomerMoney.PayType == 0 && PayTypeComboBox.SelectedItem == null) { BiuMessageBoxWindows.BiuShow("请选择支付类型!"); return; }
            if (datacontext.CustomerMoney.PayType == 0)
            {
                HonourDiscountTextBox.Text = "0";
                datacontext.CustomerMoney.HonourDiscount = System.Convert.ToDouble(HonourDiscountTextBox.Text);
                datacontext.CustomerMoney.Account = (PayTypeComboBox.SelectedItem as PayType).ID;
            }
            if (PayDate.SelectedDate != null && PayTime.SelectedTime != null)
            {
                var ds = PayDate.SelectedDate ?? DateTime.Now;
                var ts = PayTime.SelectedTime ?? DateTime.Now;
                datacontext.CustomerMoney.PayTime = Common.DateTime2TimeStamp(new DateTime(
                   ds.Year, ds.Month, ds.Day, ts.Hour, ts.Minute, ts.Second));
            }

            var d = HonourDate.SelectedDate ?? DateTime.Now;
            datacontext.CustomerMoney.HonourTime = Common.DateTime2TimeStamp(new DateTime(
                   d.Year, d.Month, d.Day));
            if (grid.BindingGroup.CommitEdit())
            {
                datacontext.SubmitCommand.Execute(this);
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

        /// <summary>
        /// 实现点击TextBox 选中文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var textBox = sender as TextBox;
                textBox.SelectAll();
                textBox.PreviewMouseDown -= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            }
        }

        /// <summary>
        /// 实现点击TextBox 选中文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
            }
        }

        private void PayTypeComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as SetShipCustomerMoneyViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPayTypeItems(sender as ComboBox);
                }
            }
        }
    }
}
