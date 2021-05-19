using biubiu.Domain;
using biubiu.model.ship_order;
using biubiu.view_model.customer.ship_customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// EditOrderForCusDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditOrderForCusDialog : UserControl
    {
        public EditOrderForCusDialog()
        {
            InitializeComponent();
        }

        private void CustomerOrderDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as EditOrderForCusViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageOrderItems(CustomerOrderDataGrid);
                }
            }
        }

        public bool IsVerticalScrollBarAtButtom(ScrollViewer s)
        {
            double dVer = s.VerticalOffset;
            double dViewport = s.ViewportHeight;
            double dExtent = s.ExtentHeight;

            bool isAtButtom;
            if (dVer != 0)
            {
                if (dVer + dViewport == dExtent)
                {
                    isAtButtom = true;
                }
                else
                {
                    isAtButtom = false;
                }
            }
            else
            {
                isAtButtom = false;
            }
            /*
            if (s.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled
                || s.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
            {
                isAtButtom = true;
            }
            */
            return isAtButtom;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var text = textBox.Text;
            // 匹配输入字符不为""， 末尾为. ，末尾为.00
            if (text == "" || text.Substring(text.Length - 1, 1).Equals(".")
                || Regex.IsMatch(text, Config.REGEXSTR_00)) return;
            if (textBox.Tag != null) {
                var order = textBox.Tag as ShipOrder;
                order.GoodsRealPrice = System.Convert.ToDouble(text);
                order.Calculate(0);
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var dataContext = DataContext as EditOrderForCusViewModel;
            if (string.IsNullOrWhiteSpace(textBox.Text)) textBox.Text = "0";
            textBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            var order = textBox.Tag as ShipOrder;
            if (dataContext.ChangeOrders.ContainsKey(order.ID))
                dataContext.ChangeOrders[order.ID] = order;
            else
                dataContext.ChangeOrders.Add(order.ID, order);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as EditOrderForCusViewModel;
            dataContext.ChangeOrders?.Clear();
            dataContext.GetShipCustomerOrderItems();
        }

        private void CustomerOrderDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            DataGridRow dataGridRow = e.Row;
            ShipOrder dataRow = e.Row.Item as ShipOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
            else
            {
                dataGridRow.Foreground = Brushes.Black;
            }
        }
    }
}
