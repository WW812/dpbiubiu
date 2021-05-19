using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.view_model.stock_order;
using biubiu.views.CommonUI;
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

namespace biubiu.views.marketing.stock_order
{
    /// <summary>
    /// StockOrderDetailsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StockOrderDetailsDialog : UserControl
    {
        // false 可以修改 true不能修改
        public Boolean IsChange = true;

        private readonly List<Image> _images = new List<Image>();

        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        public StockOrderDetailsDialog()
        {
            InitializeComponent();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as StockOrderDetailsViewModel).SetPriceByGoods();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            /*
            var textStr = textBox.Text + e.Text;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
                */
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(textBox.Text + e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsChange)
            {
                ChangeOrderBtn.Visibility = Visibility.Visible;
                DeleteOrderBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ChangeOrderBtn.Visibility = Visibility.Hidden;
                DeleteOrderBtn.Visibility = Visibility.Hidden;
            }

            #region 客户下拉框绑定
            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = (DataContext as StockOrderDetailsViewModel),
                    Path = new PropertyPath("CustomerItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, Selector.SelectedItemProperty,
               new Binding
               {
                   Source = (DataContext as StockOrderDetailsViewModel),
                   Path = new PropertyPath("Order.Customer"),
                   Mode = BindingMode.TwoWay,
                   UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
               });

            CustomerComboBox2.AccountListBox.DisplayMemberPath = "Name";
            CustomerComboBox2.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerComboBox2_SelectionChanged);

            var d = DataContext as StockOrderDetailsViewModel;
            if (d.Order.CustomerType == 1 && d.Order.Customer != null)
            {
                CustomerComboBox2.AccountTextBox.Tag = new object();
                CustomerComboBox2.AccountTextBox.Text = d.Order.Customer.Name;
            }
            #endregion

            DeductComboBox.SelectionChanged += new SelectionChangedEventHandler(DeductComboBox_SelectionChanged);

            if (d.Order.DeductWeightType == 0)
            {
                DeductSignText.Text = "吨";
                //(DataContext as StockOrderDetailsViewModel).Order.DeductWeightType = 0;
            }
            else
            {
                DeductSignText.Text = "%";
                //(DataContext as StockOrderDetailsViewModel).Order.DeductWeightType = 1;
            }

            #region 加载图片
            var datacontext = DataContext as StockOrderDetailsViewModel;
            var list = Task.Run(() => {
                return ModelHelper.GetInstance().GetApiDataArg(
                           ModelHelper.ApiClient.GetPictureURL,
                           new { id = datacontext.Order.ID }).Result.Data;
            }).Result;
            foreach (var item in list)
            {
                var bitmap = new BitmapImage()
                {
                    CreateOptions = BitmapCreateOptions.DelayCreation,
                };
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(item, UriKind.RelativeOrAbsolute);
                bitmap.DecodePixelWidth = 300;
                bitmap.EndInit();
                Image img = new Image
                {
                    Width = 300,
                    Height = 200,
                    Margin = new Thickness(2),
                    Source = bitmap
                };
                Button btn = new Button
                {
                    FontSize = 14,
                    Content = "查看大图",
                    Tag = item,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Width = 140,
                    Height = 20
                };
                btn.Click += new RoutedEventHandler(WatchBigImgEvent);
                btn.SetValue(Button.StyleProperty, Application.Current.Resources["MaterialDesignFlatButton"]);

                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };
                _images.Add(img);
                sp.Children.Add(img);
                sp.Children.Add(btn);
                PictureArea.Children.Add(sp);
            }
            #endregion
        }

        private void WatchBigImgEvent(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || btn.Tag is null) return;
            WatchBigImgWindow wbiw = new WatchBigImgWindow(btn.Tag.ToString());
            wbiw.ShowDialog();
        }

        /// <summary>
        /// 客户选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = DataContext as StockOrderDetailsViewModel;
            CustomerComboBox2.AccountTextBox.Tag = new object();
            if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as StockCustomer)?.Name;
            CustomerComboBox2.AccountPopup.IsOpen = false;
            dataContext.SetPriceByGoods();
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
                var dataContext = DataContext as StockOrderDetailsViewModel;
                dataContext.GetShipCustomerItems(CustomerComboBox2.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var datacontext = DataContext as StockOrderDetailsViewModel;
            var textBox = sender as TextBox;
            var text = textBox.Text;
            if (text == "" || text.Substring(text.Length - 1, 1).Equals(".")
                || Regex.IsMatch(text, Config.REGEXSTR_00)) return;
            switch (textBox.Name)
            {
                case "CarTareText":
                    datacontext.Order.CarTare = Convert.ToDouble(text);
                    break;
                case "CarGrossWeightText":
                    datacontext.Order.CarGrossWeight = Convert.ToDouble(text);
                    break;
                case "GoodsRealPriceTextBox":
                    datacontext.Order.GoodsRealPrice = Convert.ToDouble(text);
                    datacontext.Order.Calculate(0);
                    break;
                case "DeductWeightText":
                    datacontext.Order.DeductWeight = Convert.ToDouble(text);
                    datacontext.Order.Calculate(0);
                    break;
                case "FreightOfTonText":
                    datacontext.Order.FreightOfTon = Convert.ToDouble(text);
                    datacontext.Order.Calculate(0);
                    break;
                case "DiscountMoneyTextBox":
                    datacontext.Order.DiscountMoney = Convert.ToDouble(text);
                    datacontext.Order.Calculate(1);
                    break;
            }
        }

        private void DeductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeductSignText == null || DataContext == null) return;
            if ((sender as ComboBox).SelectedIndex == 0)
            {
                DeductSignText.Text = "吨";
                //(DataContext as StockOrderDetailsViewModel).Order.DeductWeightType = 0;
            }
            else
            {
                DeductSignText.Text = "%";
                //(DataContext as StockOrderDetailsViewModel).Order.DeductWeightType = 1;
            }
            (DataContext as StockOrderDetailsViewModel).Order.DeductWeight = 0;
            (DataContext as StockOrderDetailsViewModel).Order?.Calculate(0);
        }

        private void DeductWeightTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
            {
                var datacontext = DataContext as StockOrderDetailsViewModel;
                e.Handled = !(RegexChecksum.IsNonnegativeReal(e.Text)
                    && (DeductComboBox.SelectedIndex == 1 && Common.Double2DecimalCalculate(Convert.ToDouble(textBox.Text + e.Text)) < 50
                        || DeductComboBox.SelectedIndex == 0 && Common.Double2DecimalCalculate(Convert.ToDouble(textBox.Text + e.Text)) < datacontext.Order.CarNetWeight));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                (DataContext as StockOrderDetailsViewModel).SubmitExitCommand.Execute(this);
        }

        private void CustomerComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as StockOrderDetailsViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (DataContext == null) return;
            (DataContext as StockOrderDetailsViewModel).SetPriceByGoods();
            */

            if ((sender as ComboBox).SelectedIndex == 0)
            {
                (DataContext as StockOrderDetailsViewModel).Order.Customer = null;
                CustomerComboBox2.AccountTextBox.Text = "";
                CustomerComboBox2.AccountListBox.SelectedItem = null;
                CustomerComboBox2.AccountPopup.IsOpen = false;
                szmCustomerTimer.Start();
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (DataContext == null) return;
            (DataContext as StockOrderDetailsViewModel).SetPriceByGoods();
            */
        }

        private void CustomerComboBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomerComboBox2.AccountTextBox.Tag == null)
                szmCustomerTimer.Start();
            else
                CustomerComboBox2.AccountTextBox.Tag = null;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // 清除图片缓存
            foreach (var item in _images)
            {
                item.Source = null;
            }
            _images.Clear();
        }
    }
}
