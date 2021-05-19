using biubiu.Domain;
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
    /// SetShipCustomerPriceDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SetShipCustomerPriceDialog : UserControl
    {
        public SetShipCustomerPriceDialog()
        {
            InitializeComponent();
        }

        private void CreateShipCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            //if (grid.BindingGroup.CommitEdit()) 暂留
            (DataContext as SetShipCustomerPriceViewModel).SubmitCommand.Execute(this);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (AllDiscountmoneyTextBox.Text.Equals("")) return;
            (DataContext as SetShipCustomerPriceViewModel).SetAllDiscountmoney(Convert.ToDouble(AllDiscountmoneyTextBox.Text));
        }

        private void AllDiscountmoneyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text == "-" && !textBox.Text.Contains("-")) return;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

        private void AllDiscountmoneyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllDiscountmoneyTextBox.Text.Equals("-")) return;
            if (AllDiscountmoneyCheckBox.IsChecked == true && RegexChecksum.IsReal(AllDiscountmoneyTextBox.Text)) {
                var text = (sender as TextBox).Text;
                (DataContext as SetShipCustomerPriceViewModel).SetAllDiscountmoney(Convert.ToDouble(text));
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
    }
}
