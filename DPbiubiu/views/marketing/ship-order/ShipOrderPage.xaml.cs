using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
using biubiu.view_model.ship_order;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using WebApiClient;

namespace biubiu.views.marketing.ship_order
{
    /// <summary>
    /// ShipOrderPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShipOrderPage : UserControl
    {
        /// <summary>
        /// 搜索客户计时器，延迟发送
        /// </summary>
        private static readonly DispatcherTimer readDataTimer = new DispatcherTimer();
        private static readonly DispatcherTimer szmCustomerTimer = new DispatcherTimer();

        private bool _customerChangedEnabled = true; // 为true的时候，客户TextBox进行搜索

        public ShipOrderPage()
        {
            InitializeComponent();
            DataContext = new ShipOrderViewModel();
            readDataTimer.Tick += new EventHandler(CustomerCarByCarIdTimer);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
            szmCustomerTimer.Tick += new EventHandler(SearchCus);
            szmCustomerTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);

            #region 搜索框的注册
            BindingOperations.SetBinding(CarIdComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = (DataContext as ShipOrderViewModel),
                    Path = new PropertyPath("CustomerCarItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CarIdComboBox2.AccountTextBox, TextBox.TextProperty,
                new Binding
                {
                    Source = (DataContext as ShipOrderViewModel),
                    Path = new PropertyPath("Order.CarId"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = (DataContext as ShipOrderViewModel),
                    Path = new PropertyPath("CustomerItems"),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            BindingOperations.SetBinding(CustomerComboBox2.AccountListBox, Selector.SelectedItemProperty,
                new Binding
                {
                    Source = (DataContext as ShipOrderViewModel),
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

        private void CustomerComboBox2_TextInput(object sender, TextChangedEventArgs e)
        {
            if (_customerChangedEnabled)
            {
                szmCustomerTimer.Start();
            }
            _customerChangedEnabled = true;
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
                var item = (ShipCustomerCar)CarIdComboBox2.AccountListBox.SelectedItem;
                if (item == null) return;
                var dataContext = DataContext as ShipOrderViewModel;
                CarIdComboBox2.AccountTextBox.Text = item.CarId;
                //_selectedCarId = true;
                dataContext.Order.CustomerType = 1;
                /*
                if (!dataContext.CustomerItems.Contains(item.Customer))
                {
                    dataContext.CustomerItems.Add(item.Customer);
                    dataContext.Order.Customer = item.Customer;
                }
                else
                    dataContext.Order.Customer = dataContext.CustomerItems.FirstOrDefault(x => x.ID == item.Customer.ID);
                */
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

        private void CustomerComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CustomerComboBox2.AccountListBox.Tag == null)
                {
                    var dataContext = (DataContext as ShipOrderViewModel);
                    _customerChangedEnabled = false;
                    if (CustomerComboBox2.AccountListBox.SelectedItem != null)
                        CustomerComboBox2.AccountTextBox.Text = (CustomerComboBox2.AccountListBox.SelectedItem as ShipCustomer)?.Name;
                    dataContext.SetPriceByGoods(0);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as ShipOrderViewModel;
            datacontext.GetData();
            datacontext.Order.Reset();
            datacontext.CurrentCustomerPage.Page = 0;
            NTB_Gross.Text = "0";
            NTB_Tare.Text = "0";
            //datacontext.RunPond();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            /*
            var datacontext = DataContext as ShipOrderViewModel;
            datacontext.ClosePond();
            */
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
            var dataContext = DataContext as ShipOrderViewModel;
            dataContext.GetCustomerCarByCarId();
            CarIdComboBox2.AccountPopup.IsOpen = true;
            readDataTimer.Stop();
        }

        /// <summary>
        /// 车号下拉框选项变换 废弃
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        private void CarIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarIdComboBox.SelectedItem == null) return;
            var dataContext = DataContext as ShipOrderViewModel;
            var customer = (CarIdComboBox.SelectedItem as ShipCustomerCar).Customer;
            _selectedCarId = true;
            dataContext.Order.CustomerType = 1;
            dataContext.Order.Customer = dataContext.CustomerItems.FirstOrDefault(x => x.ID == customer.ID);
        }
        */

        /// <summary>
        /// 料品点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            var chip = sender as MaterialDesignThemes.Wpf.Chip;
            var dataContext = DataContext as ShipOrderViewModel;
            //重置所有chip颜色
            var sg = chip.Tag as ShipGoods;
            dataContext.Order.Goods = sg;
            dataContext.SetPriceByGoods();
            SelectedGoodsChips(sg);
        }

        /// <summary>
        /// 根据料品更新料品控件选中状态
        /// </summary>
        /// <param name="goods">goods为null时，无选中</param>
        private void SelectedGoodsChips(ShipGoods goods)
        {
            for (int i = 0; i < GoodsListView.Items.Count; i++)
            {
                var item = GoodsListView.ItemContainerGenerator.ContainerFromIndex(i);
                MaterialDesignThemes.Wpf.Chip c = Common.FindSingleVisualChildren<MaterialDesignThemes.Wpf.Chip>(item);
                if (goods != null && goods.ID == (GoodsListView.Items[i] as ShipGoods).ID)
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
            var datacontext = DataContext as ShipOrderViewModel;
            if (datacontext.Order.Status != 1) return;
            var textBox = sender as TextBox;
            var text = textBox.Text;
            // 匹配输入字符不为""， 末尾为. ，末尾为.00
            if (text == "" || text.Substring(text.Length - 1, 1).Equals(".")
                || Regex.IsMatch(text, Config.REGEXSTR_00)) return;
            switch (textBox.Name)
            {
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
            var dataContext = DataContext as ShipOrderViewModel;
            dataContext.ResetOrder(true); //重置单据
            dataContext.GetData();  //拉取数据
            SelectedGoodsChips(null);  //重置料品控件颜色
            EnterDataGrid.SelectedItem = null; //清空选择状态
            NTB_Tare.Text = "0";
            NTB_Gross.Text = "0";
            dataContext.CurrentCustomerPage.Page = 0;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
            CarIdComboBox2.AccountPopup.IsOpen = false;
        }

        /// <summary>
        /// 进厂单据表单SelectionChanged响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EnterDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                EnterDataGrid.IsEnabled = false;
                //_selectedCarId = true;
                var dataContext = DataContext as ShipOrderViewModel;
                return;
                if (EnterDataGrid.SelectedItem == null || dataContext.MenuIndex != 0) return;
                //dataContext.Order.Reset();
                //dataContext.ResetOrder(true);
                _customerChangedEnabled = false;
                CarIdComboBox2.AccountTextBox.Tag = new object();
                dataContext.CurrentPonderationDisplay = null;
                dataContext.CustomerCarItems = null;
                dataContext.Order = Common.DeepCopy(EnterDataGrid.SelectedItem as ShipOrder);
                dataContext.Order.Status = 1;
                NTB_Tare.Text = dataContext.Order.CarTare.ToString();
                NTB_Gross.Text = dataContext.Order.CarGrossWeight.ToString();
                SelectedGoodsChips(dataContext.Order.Goods);
                if (dataContext.Order.Customer != null)
                {
                    CustomerComboBox2.AccountListBox.Tag = new object();
                    //Task.Run(() =>
                    //{
                    var r = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetShipCustomerAsync, new { Page = 0, Size = 500 });
                    dataContext.CustomerItems = new ObservableCollection<ShipCustomer>(r.Data);
                    //dataContext.CustomerItems.FirstOrDefault(x => ( x.ID == dataContext.Order.Customer.ID));
                    //foreach (ShipCustomer item in CustomerComboBox2.AccountListBox.Items)

                    foreach (ShipCustomer item in CustomerComboBox2.AccountListBox.Items)
                    {
                        //if (item.ID == dataContext.Order.Customer.ID) CustomerComboBox2.AccountListBox.SelectedItem = item;
                        if (item.ID == (EnterDataGrid.SelectedItem as ShipOrder).Customer?.ID)
                        {
                            //Application.Current.Dispatcher.Invoke(new Action(() =>
                            //{
                            CustomerComboBox2.AccountListBox.SelectedItem = item;
                            CustomerComboBox2.AccountTextBox.Text = item.Name;
                            //}));
                        }
                    }
                    //});
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
                EnterDataGrid.IsEnabled = true;
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
            if (!(DataContext is ShipOrderViewModel datacontext)) return;
            var selectedIndex = MenuBtn.SelectedIndex;
            SearchOrderTextBox.Clear();
            if (selectedIndex < 0 || selectedIndex > 2)
                MenuBtn.SelectedIndex = datacontext.MenuIndex;
            else
                datacontext.MenuIndex = MenuBtn.SelectedIndex;
                */
            //SearchOrderTextBox.Clear();
        }

        /// <summary>
        /// 地磅点击
        /// </summary>
        /// <param name="sender"></param>)
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as ShipOrderViewModel).RefreshWeight();
            /*
            var grid = sender as Grid;
            grid.Width = 150;
            grid.Height = 105;
            grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
            */
        }

        /// <summary>
        /// 选择客户ComboBox SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = (DataContext as ShipOrderViewModel);
            dataContext.SetPriceByGoods(0);
            CustomerTogBtn.IsChecked = true;
        }

        /// <summary>
        /// 客户下拉框文本输入，用于查找客户 已废弃
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        private void CustomerComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomerComboBox.Text == null ||
                (!CustomerComboBox.Text.Equals((CustomerComboBox?.SelectedItem as ShipCustomer)?.Name ?? "")))
            {
                CustomerComboBox.IsDropDownOpen = true;
                szmCustomerTimer.Start();
            }
        }
        */

        /// <summary>
        /// 搜索客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchCus(object sender, EventArgs e)
        {
            try
            {
                var dataContext = DataContext as ShipOrderViewModel;
                //dataContext.GetShipCustomerItems(CustomerComboBox.Text); 修改中
                dataContext.GetShipCustomerItems(CustomerComboBox2.AccountTextBox.Text);
                szmCustomerTimer.Stop();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        /// <summary>
        /// 车牌号按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            SoftKeyBoardPopup.IsOpen = !SoftKeyBoardPopup.IsOpen;
        }

        private void CustomerComboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            /* 因为扩大获取客户数量，取消下滑分页
            var datacontext = DataContext as ShipOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as ComboBox);
                }
            }
            */
        }

        /// <summary>
        /// 客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as ShipOrderViewModel).SetPriceByGoods();
        }

        /// <summary>
        /// 零售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerTogBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ShipOrderViewModel;
            CustomerComboBox2.AccountListBox.SelectedItem = null;
            CustomerComboBox2.AccountTextBox.Text = "";
            CustomerComboBox2.AccountPopup.IsOpen = false;
            //szmCustomerTimer.Start();
            dataContext.Order.CustomerType = 0;
            dataContext.SetPriceByGoods(0);
        }

        private void EnterDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
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

        private void SearchOrderTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (e.Key == Key.Enter)
                SearchButton.Command.Execute(textBox.Text);
        }

        private void EnterDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var dataContext = DataContext as ShipOrderViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    dataContext.NextPageOrderItems(EnterDataGrid);
                }
            }
        }


        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock.Text.Length > 10 && textBlock.Tag == null)
            {
                var s = 25 - BalanceRun.Text.Length;
                BalanceRun.FontSize = (s < 12) ? 12 : s;
            }
            else
            {
                BalanceRun.FontSize = 22;
                textBlock.Tag = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            // 预装吨数显示切换为预装吨数显示
            AdvCubicTextBlock.Text = "预装吨数:";
            BindingOperations.SetBinding(AdvCubicTextBox, TextBox.TextProperty,
            new Binding
            {
                Source = (DataContext as ShipOrderViewModel),
                Path = new PropertyPath("Order.AdvanceWeight"),
            });
            */
        }

        private void NumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as ShipOrderViewModel;
            if (sender is TextBox txt) {
                if (string.IsNullOrEmpty(txt.Text)) txt.Text = "0";
                else
                {
                    double.TryParse(txt.Text, out double d);
                    txt.Text = d.ToString();
                    if (txt.Name.Equals("NTB_Tare")) datacontext.Order.CarTare = d;
                    if (txt.Name.Equals("NTB_Gross")) datacontext.Order.CarGrossWeight = d;
                }
            }
            //datacontext.Order.CarGrossWeight = NTB_Care.Text;
            datacontext.Order.CarNetWeight = Common.Double2DecimalCalculate(datacontext.Order.CarGrossWeight - datacontext.Order.CarTare);
            datacontext.Calculate(0);
        }

        private void NTB_Gross_GotFocus(object sender, RoutedEventArgs e)
        {
            NTB_Gross.SelectAll();
        }
    }
}
