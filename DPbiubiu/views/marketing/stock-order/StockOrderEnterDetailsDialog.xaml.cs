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
    /// StockOrderEnterDetailsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StockOrderEnterDetailsDialog : UserControl
    {
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();
        private readonly List<Image> _images = new List<Image>();

        public StockOrderEnterDetailsDialog()
        {
            InitializeComponent();
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as StockOrderDetailsViewModel).SetPriceByGoods();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                (DataContext as StockOrderDetailsViewModel).SubmitCommand.Execute(this);
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerComboBox2.AccountTextBox.Tag = new object();
            if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as StockCustomer)?.Name;
            CustomerComboBox2.AccountPopup.IsOpen = false;
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
