using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.stock_order;
using biubiu.view_model.stock_order;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// StockOrderPage.xaml 的交互逻辑
    /// </summary>
    public partial class StockOrderPage : UserControl
    {
        /// <summary>
        /// 搜索客户计时器，延迟发送
        /// </summary>
        private static readonly DispatcherTimer readDataTimer = new DispatcherTimer();
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        private bool _selectedCarId = false;

        public StockOrderPage()
        {
            InitializeComponent();
            DataContext = new StockOrderViewModel();
            readDataTimer.Tick += new EventHandler(CustomerCarByCarIdTimer);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);

            #region 搜索框的注册
            BindingOperations.SetBinding(CarIdComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = (DataContext as StockOrderViewModel),
                    Path = new PropertyPath("CustomerCarItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CarIdComboBox2.AccountTextBox, TextBox.TextProperty,
               new Binding
               {
                   Source = (DataContext as StockOrderViewModel),
                   Path = new PropertyPath("Order.CarId"),
                   UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
               });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
               new Binding
               {
                   Source = (DataContext as StockOrderViewModel),
                   Path = new PropertyPath("CustomerItems"),
                   UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
               });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, Selector.SelectedItemProperty,
                new Binding
                {
                    Source = (DataContext as StockOrderViewModel),
                    Path = new PropertyPath("Order.Customer"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            CarIdComboBox2.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CarIdComboBox2_SelectionChanged);
            CustomerComboBox2.AccountListBox.SelectionChanged += new SelectionChangedEventHandler(CustomerComboBox2_SelectionChanged);
            //CustomerComboBox2.AccountTextBox.PreviewTextInput += new TextCompositionEventHandler(CustomerComboBox2_TextInput);
            CustomerComboBox2.AccountListBox.DisplayMemberPath = "Name";
            #endregion
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
                var dataContext = DataContext as StockOrderViewModel;
                //dataContext.GetShipCustomerItems(CustomerComboBox.Text); 修改中
                dataContext.GetShipCustomerItems(CustomerComboBox2.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CustomerComboBox2.AccountListBox.Tag == null)
                {
                    var dataContext = (DataContext as StockOrderViewModel);
                    CustomerComboBox2.AccountTextBox.Tag = new object();
                    if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                        CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as StockCustomer)?.Name;
                    dataContext.SetPriceByGoods();
                    CustomerTogBtn.IsChecked = true;
                    CustomerComboBox2.AccountPopup.IsOpen = false;
                }
                else
                {
                    CustomerComboBox2.AccountListBox.Tag = null;
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }

        }

        /// <summary>
        /// 车牌号SelectionChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarIdComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (StockCustomerCar)CarIdComboBox2.AccountListBox.SelectedItem;
                if (item == null) return;
                var dataContext = DataContext as StockOrderViewModel;
                CarIdComboBox2.AccountTextBox.Text = item.CarId;
                dataContext.Order.CustomerType = 1;
                var c = dataContext.CustomerItems.FirstOrDefault(x => x.ID == item.Customer.ID);
                if (c == null)
                {
                    dataContext.CustomerItems.Add(item.Customer);
                    dataContext.Order.Customer = item.Customer;
                }
                else
                {
                    dataContext.Order.Customer = c;
                }
                CarIdComboBox2.AccountPopup.IsOpen = false;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        /// <summary>
        /// 地磅点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as StockOrderViewModel).RefreshWeight();
        }

        /// <summary>
        /// 车牌号下拉框输入文字变换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarIdComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CarIdComboBox2.AccountTextBox.Text)
                && CarIdComboBox2.AccountTextBox.Tag == null)
            {
                readDataTimer.Start();
            }
            else
            {
                CarIdComboBox2.AccountTextBox.Tag = null;
                CarIdComboBox2.AccountPopup.IsOpen = false;
            }
        }

        private void CustomerCarByCarIdTimer(object sender, EventArgs e)
        {
            var dataContext = DataContext as StockOrderViewModel;
            dataContext.GetCustomerCarByCarId();
            CarIdComboBox2.AccountPopup.IsOpen = true;
            readDataTimer.Stop();
        }

        /// <summary>
        /// 车牌号输入框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarIdComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        }

        /// <summary>
        /// 车号下拉框选项变换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        private void CarIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarIdComboBox.SelectedItem == null) return;
            var dataContext = DataContext as StockOrderViewModel;
            var customer = (CarIdComboBox.SelectedItem as StockCustomerCar).Customer;
            _selectedCarId = true;
            dataContext.Order.CustomerType = 1;
            dataContext.Order.Customer = dataContext.CustomerItems.FirstOrDefault(x => x.ID == customer.ID);
        }
        */

        /// <summary>
        /// 车牌号按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            SoftKeyBoardPopup.IsOpen = !SoftKeyBoardPopup.IsOpen;
        }

        /// <summary>
        /// 料品点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            var chip = sender as MaterialDesignThemes.Wpf.Chip;
            var dataContext = DataContext as StockOrderViewModel;
            //重置所有chip颜色
            var sg = chip.Tag as StockGoods;
            dataContext.Order.Goods = sg;
            dataContext.SetPriceByGoods();
            SelectedGoodsChips(sg);
        }

        /// <summary>
        /// 根据料品更新料品控件选中状态
        /// </summary>
        /// <param name="goods">goods为null时，无选中</param>
        private void SelectedGoodsChips(StockGoods goods)
        {
            for (int i = 0; i < GoodsListView.Items.Count; i++)
            {
                var item = GoodsListView.ItemContainerGenerator.ContainerFromIndex(i);
                MaterialDesignThemes.Wpf.Chip c = Common.FindSingleVisualChildren<MaterialDesignThemes.Wpf.Chip>(item);
                if (goods != null && goods.ID == (GoodsListView.Items[i] as StockGoods).ID)
                { //选中状态
                    c.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                    c.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
                }
                else
                {
                    c.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
                    c.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                }
            }
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
            var datacontext = DataContext as StockOrderViewModel;
            if (datacontext.Order.Status != 1) return;
            var textBox = sender as TextBox;
            var text = textBox.Text;
            if (text == "" || text.Substring(text.Length - 1, 1).Equals(".")
                || Regex.IsMatch(text, Config.REGEXSTR_00)) return;
            switch (textBox.Name)
            {
                case "GoodsRealPriceTextBox":
                    datacontext.Order.GoodsRealPrice = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "DeductWeightTextBox":
                    datacontext.Order.DeductWeight = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "FreightOfTonTextBox":
                    datacontext.Order.FreightOfTon = Convert.ToDouble(text);
                    datacontext.Calculate(0);
                    break;
                case "DiscountMoneyTextBox":
                    datacontext.Order.DiscountMoney = Convert.ToDouble(text);
                    datacontext.Calculate(1);
                    break;
            }
        }

        /// <summary>
        /// 选择客户ComboBox SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as StockOrderViewModel).SetPriceByGoods();
            CustomerTogBtn.IsChecked = true;
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
        /// 新单按钮点击事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOrder();
        }

        /// <summary>
        /// 新单处理，重置界面状态
        /// </summary>
        private void ResetOrder()
        {
            var dataContext = DataContext as StockOrderViewModel;
            dataContext.ResetOrder(true); //重置单据
            dataContext.GetGoods();  //拉取数据
            dataContext.GetCustomer();  //拉取数据
            dataContext.GetOrders();  //拉取数据
            SelectedGoodsChips(null);  //重置料品控件颜色
            OrderDataGrid.SelectedItem = null; //清空选择状态
            dataContext.CurrentCustomerPage.Page = 0;
            DeductComboBox.SelectedIndex = 0; //重置扣吨
            //CustomerComboBox.Text = null;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
            CarIdComboBox2.AccountPopup.IsOpen = false;
        }

        /// <summary>
        /// 进厂单据表单SelectionChanged响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OrderDataGrid.IsEnabled = false;
                //_selectedCarId = true;
                var dataContext = DataContext as StockOrderViewModel;
                if (OrderDataGrid.SelectedItem == null || dataContext.MenuIndex != 0) return;
                //dataContext.ResetOrder(true);
                CustomerComboBox2.AccountTextBox.Tag = new object();
                CarIdComboBox2.AccountTextBox.Tag = new object();
                dataContext.Order = Common.DeepCopy(OrderDataGrid.SelectedItem as StockOrder);
                dataContext.Order.Status = 1;
                SelectedGoodsChips(dataContext.Order.Goods);
                if (dataContext.Order.Customer != null)
                {
                    CustomerComboBox2.AccountListBox.Tag = new object();
                    var r = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetStockCustomerAsync, new { Page = 0, Size = 500 });
                    dataContext.CustomerItems = new ObservableCollection<StockCustomer>(r.Data);
                    foreach (StockCustomer item in CustomerComboBox2.AccountListBox.Items)
                    {
                        //if (item.ID == dataContext.Order.Customer.ID) CustomerComboBox.SelectedItem = item;
                        if (item.ID == (OrderDataGrid.SelectedItem as StockOrder).Customer?.ID)
                        {
                            //Application.Current.Dispatcher.Invoke(new Action(() =>
                            //{
                            CustomerComboBox2.AccountListBox.SelectedItem = item;
                            CustomerComboBox2.AccountTextBox.Text = item.Name;
                            //}));
                        }
                    }
                }
                CarIdComboBox2.AccountPopup.IsOpen = false;
                CustomerComboBox2.AccountPopup.IsOpen = false;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            finally
            {
                OrderDataGrid.IsEnabled = true;
            }
        }

        /// <summary>
        /// 选择不同列表(单据)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (!(DataContext is StockOrderViewModel datacontext)) return;
            var selectedIndex = MenuBtn.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex > 2)
                MenuBtn.SelectedIndex = datacontext.MenuIndex;
            else
                datacontext.MenuIndex = MenuBtn.SelectedIndex;
                */
                /*
            if(selectedIndex == 0)
            {
                MenuBtn.Items.RemoveAt(1);
            }
            else
            {
                MenuItem mi = new MenuItem
                {
                    FontSize = 20,
                    Header = "补印票据",
                };
            }
            */
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as StockOrderViewModel;
            datacontext.GetCustomer();
            datacontext.GetGoods();
            datacontext.GetOrders();
            datacontext.Order.Reset();
            datacontext.CurrentCustomerPage.Page = 0;
            datacontext.OpenPond();
            datacontext.RunPond();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as StockOrderViewModel;
            datacontext.ClosePond();
            //datacontext.ResetSnapshotPicture();
        }

        private void CustomerComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as StockOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeductSignText == null) return;
            if ((sender as ComboBox).SelectedIndex == 0)
            {
                DeductSignText.Text = "吨";
                (DataContext as StockOrderViewModel).Order.DeductWeightType = 0;
            }
            else
            {
                DeductSignText.Text = "%";
                (DataContext as StockOrderViewModel).Order.DeductWeightType = 1;
            }
            DeductWeightTextBox.Text = "0";
            (DataContext as StockOrderViewModel).Order.Calculate(0);
        }

        private void DeductWeightTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
            {
                var datacontext = DataContext as StockOrderViewModel;
                e.Handled = !(RegexChecksum.IsNonnegativeReal(e.Text)
                    && (DeductComboBox.SelectedIndex == 1 && Common.Double2DecimalCalculate(Convert.ToDouble(textBox.Text + e.Text)) < 50
                        || DeductComboBox.SelectedIndex == 0 && Common.Double2DecimalCalculate(Convert.ToDouble(textBox.Text + e.Text)) < datacontext.Order.CarNetWeight));
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as StockOrderViewModel).SetPriceByGoods();
        }

        /// <summary>
        /// 零售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as StockOrderViewModel;
            CustomerComboBox2.AccountListBox.SelectedItem = null;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
            dataContext.Order.CustomerType = 0;
            dataContext.SetPriceByGoods();
        }

        private void OrderDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            StockOrder dataRow = e.Row.Item as StockOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
            else
            {
                dataGridRow.Foreground = Brushes.Black;
            }
        }

        private void OrderDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var dataContext = DataContext as StockOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    dataContext.NextPageOrderItems(OrderDataGrid);
                }
            }
        }

        private void CustomerComboBox2_TextInput(object sender, TextChangedEventArgs e)
        {
            if (CustomerComboBox2.AccountTextBox.Tag == null)
                szmCustomerTimer.Start();
            else
                CustomerComboBox2.AccountTextBox.Tag = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeductComboBox.SelectedIndex = 0; //重置扣吨
        }
    }
}
