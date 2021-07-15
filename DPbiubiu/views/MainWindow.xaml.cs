using AutoUpdaterDotNET;
using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.FactoryService.StoneProduce;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.view_model;
using biubiu.view_model.ship_order;
using biubiu.views.finance.accept;
using biubiu.views.finance.paytype;
using biubiu.views.finance.report;
using biubiu.views.login;
using biubiu.views.marketing.customer.stock_customer;
using biubiu.views.marketing.goods;
using biubiu.views.marketing.ship_bill;
using biubiu.views.marketing.ship_customer;
using biubiu.views.marketing.ship_order;
using biubiu.views.marketing.ship_order_manage;
using biubiu.views.marketing.stock_bill;
using biubiu.views.marketing.stock_goods;
using biubiu.views.marketing.stock_order;
using biubiu.views.marketing.stock_order_manage;
using biubiu.views.setting.adjust_weight;
using biubiu.views.setting.device;
using biubiu.views.setting.print;
using biubiu.views.setting.role;
using biubiu.views.setting.system;
using MaterialDesignThemes.Wpf;
using RuningInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WebApiClient;
using static biubiu.model.ModelHelper;

namespace biubiu.views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginWindow loginWindow;

        public static Dictionary<string, object> LeftMenuPool = new Dictionary<string, Object>();
        //readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromHours(3) };
        private readonly List<string> _roles = new List<string>(); //权限名列表
        private string _currentPageName = "default";


        public MainWindow(LoginWindow window)
        {
            _roles.Clear();
            InitializeComponent();
            InitializeConfigINI();
            InitPermissionMenu();
            InitializeBillConfig();
            DataContext = new MainWindowViewModel();
            SnackbarViewModel.GetInstance().SetSanckbar(SnackbarOne);
            UserName.Text = Config.CURRENT_USER.NickName;
            loginWindow = window;
            TitleText.Text = Config.COMPANY_NAME;
            Btn_AdjustPondEnabled.Background = Config.AdjustWeightEnabled ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#668B8B")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            Btn_AdjustPrintEnabled.Background = Config.AdjustPrintEnabled ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#668B8B")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            GetSystemSetting();
            /*
            timer.Tick += delegate
            {
                AutoUpdater.Start(Config.UPDATE_URL);
                AutoUpdater.UpdateMode = Mode.Forced;  //更新类型
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600); //更新窗口大小
            };
            timer.Start();
            PutInfo();
            */
        }

        private void PutInfo()
        {
            RuningInfoModel rim = new RuningInfoModel();
            try
            {
                rim.UserName = Config.CURRENT_USER.NickName;
                rim.UserAccount = Config.CURRENT_USER.UserName;
                rim.CompanyName = Config.COMPANY_NAME;
                rim.ComputerName = Environment.MachineName;
                rim.SystemName = Environment.OSVersion.Platform.ToString();
                rim.SystemVersion = Environment.OSVersion.VersionString;
                rim.SystemBit = Environment.Is64BitOperatingSystem ? "64bit" : "32bit";
                RuningInfoHelper.PutInfo(rim);
            }
            catch (Exception er)
            {
                rim.ProvinceName = er.Message;
                RuningInfoHelper.PutInfo(rim);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnTag = (sender as Button).Tag.ToString();
                SwitchPage(btnTag);
                //MainView.UnregisterName("");  注销对应控件
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 接口跳转处理
        /// </summary>
        /// <param name="tag"></param>
        private void SwitchPage(string tag)
        {
            if (!LeftMenuPool.ContainsKey(tag))
            {
                LeftMenuPool.Add(tag, GetUserControlByTag(tag));
            }
            switch (tag)
            {
                //需要初始化硬件
                case "采购进料过磅":
                case "销售出料过磅":
                case "设置设备管理":
                    //InitializeConfigINI();
                    InitializeBillConfig();
                    GetSystemSetting();
                    break;
                //不需要初始化硬件
                case "销售单据管理":
                case "采购单据管理":
                    GetSystemSetting();
                    InitializeBillConfig();
                    break;
                case "设置票据设置":
                case "销售客户管理":
                case "采购客户管理":
                case "销售交账记录":
                case "采购交账记录":
                    InitializeBillConfig();
                    break;
            }
            //Task.Run(() =>
            //{
                //Thread.Sleep(340);
                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                    MainFrame.Content = LeftMenuPool[tag];
                    _currentPageName = tag;
            /*
            try
            {
                ShipOrderPage sop = LeftMenuPool[_currentPageName] as ShipOrderPage;
                ShipOrderViewModel sovm = sop.DataContext as ShipOrderViewModel;
                if ("销售出料过磅".Equals(_currentPageName))
                    sovm.Run_RFID_LJYZN();
                else
                    sovm.CloseRFID();
            }
            catch { }
            */
                //}));
            //});
        }

        async void InitPermissionMenu()
        {
            try
            {
                LeftMenuPool.Clear();
                // 首页
                LeftMenuPool.Add("default", GetUserControlByTag(""));
                MainFrame.Content = LeftMenuPool["default"];
                Button defaultBtn = new Button
                {
                    Content = "首页",
                    Command = MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand,
                    CommandParameter = Dock.Left,
                    Margin = new Thickness(4, 20, 4, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 18,
                    Tag = ""
                };
                defaultBtn.SetValue(Button.StyleProperty, Application.Current.Resources["MaterialDesignFlatButton"]);
                defaultBtn.Click += new RoutedEventHandler(MenuItem_Click);
                LeftMenuStackPanel.Children.Add(defaultBtn);

                //左侧剩余列表
                var Result = await ModelHelper.ApiClient.GetPermissionMenuAsync();
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    var groupInfo = Result.Data.GroupBy(m => m.Type).ToList();
                    foreach (var item in groupInfo)
                    {
                        Expander expander = new Expander
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Header = new TextBlock
                            {
                                Text = item.Key,
                                FontSize = 20,
                                HorizontalAlignment = HorizontalAlignment.Center
                            },
                            IsExpanded = true
                        };
                        StackPanel sp = new StackPanel();
                        expander.Content = sp;
                        var perList = item.OrderBy(m => m.Weight).ToList();
                        foreach (var per in perList)
                        {
                            Button btn = new Button
                            {
                                Content = per.Name,
                                Command = MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand,
                                CommandParameter = Dock.Left,
                                Margin = new Thickness(4),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                FontSize = 18,
                                Tag = item.Key + per.Name
                            };
                            btn.SetValue(Button.StyleProperty, Application.Current.Resources["MaterialDesignFlatButton"]);
                            btn.Click += new RoutedEventHandler(MenuItem_Click);
                            sp.Children.Add(btn);
                            _roles.Add(item.Key + per.Name);
                        }
                        LeftMenuStackPanel.Children.Add(expander);
                    }
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        public void InitializeBillConfig()
        {
            INIClass billConfig = new INIClass(Config.BILL_CONFIG_PATH);
            try
            {
                if (!billConfig.ExistINIFile())
                {
                    var rs = BiuMessageBoxWindows.BiuShow("票据打印配置文件出错，是否重置配置文件？", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question);
                    if (!((rs == BiuMessageBoxResult.Yes) ? ResetConfigINI(billConfig) : true))
                    {
                        BiuMessageBoxWindows.BiuShow("重置失败!", image: BiuMessageBoxImage.Error);
                        File.Delete(Config.BILL_CONFIG_PATH);
                    }
                    return;
                }
                Config.STOCK_ENTER_CONFIG.ReadFromFile(billConfig, "StockEnter");
                Config.STOCK_EXIT_CONFIG.ReadFromFile(billConfig, "StockExit");
                Config.SHIP_ENTER_CONFIG.ReadFromFile(billConfig, "ShipEnter");
                Config.SHIP_EXIT_CONFIG.ReadFromFile(billConfig, "ShipExit");
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        public void InitializeConfigINI()
        {
            INIClass iniClass = new INIClass(Config.DEVICE_CONFIG_PATH);
            try
            {
                if (!iniClass.ExistINIFile())
                {
                    var rs = BiuMessageBoxWindows.BiuShow("配置文件出错，是否重置配置文件？", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question);
                    if (!((rs == BiuMessageBoxResult.Yes) ? ResetConfigINI(iniClass) : true))
                    {
                        BiuMessageBoxWindows.BiuShow("重置失败!", image: BiuMessageBoxImage.Error);
                        File.Delete(Config.DEVICE_CONFIG_PATH);
                    }
                    return;
                }
                Config.P1.ReadFromFile(iniClass, "1Ponderation");
                Config.P2.ReadFromFile(iniClass, "2Ponderation");
                Config.P3.ReadFromFile(iniClass, "3Ponderation");
                Config.P4.ReadFromFile(iniClass, "4Ponderation");
                Config.PondDataP1 = Config.P1.GetPondDataParameter();
                Config.PondDataP2 = Config.P2.GetPondDataParameter();
                Config.PondDataP3 = Config.P3.GetPondDataParameter();
                Config.PondDataP4 = Config.P4.GetPondDataParameter();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 重置配置文件
        /// </summary>
        /// <param name="iniClass"></param>
        /// <returns></returns>
        private bool ResetConfigINI(INIClass iniClass)
        {
            try
            {
                switch (iniClass.inipath)
                {
                    case Config.DEVICE_CONFIG_PATH:
                        // 进行重置
                        Config.P1.WriteToFile(iniClass, "1Ponderation");
                        Config.P2.WriteToFile(iniClass, "2Ponderation");
                        Config.P3.WriteToFile(iniClass, "3Ponderation");
                        Config.P4.WriteToFile(iniClass, "4Ponderation");
                        break;
                    case Config.BILL_CONFIG_PATH:
                        Config.STOCK_ENTER_CONFIG.WriteToFile(iniClass, "StockEnter");
                        Config.STOCK_EXIT_CONFIG.WriteToFile(iniClass, "StockExit");
                        Config.SHIP_ENTER_CONFIG.WriteToFile(iniClass, "ShipEnter");
                        Config.SHIP_EXIT_CONFIG.WriteToFile(iniClass, "ShipExit");
                        break;
                }
                return true;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// 设置系统设置
        /// </summary>
        private void GetSystemSetting()
        {
            Task.Run(() =>
            {
                Config.SYSTEM_SETTING = ModelHelper.GetInstance().GetApiData(
                    ApiClient.GetSystemSetting).Result.Data;
            });
        }

        private object GetUserControlByTag(string tag)
        {
            switch (tag)
            {
                case "设置用户管理":
                    return new RolePage();
                case "销售料品管理":
                    return new GoodsPage();
                case "采购料品管理":
                    return new StockGoodsPage();
                case "设置设备管理":
                    return new DevicePage();
                case "销售单据管理":
                    return new ShipOrderManage();
                case "采购单据管理":
                    return new StockOrderManage();
                case "销售客户管理":
                    return new ShipCustomerPage();
                case "采购客户管理":
                    return new StockCustomerPage();
                case "销售交账记录":
                    return new ShipBillPage();
                case "采购交账记录":
                    return new StockBillPage();
                case "销售出料过磅":
                    return new ShipOrderPage();
                case "采购进料过磅":
                    return new StockOrderPage();
                case "设置票据设置":
                    return new PrintPage();
                case "设置系统设置":
                    return new SystemPage();
                case "财务承兑管理":
                    return new AcceptPage();
                case "财务账户管理":
                    return new PayTypePage();
                case "财务报表管理":
                    return new ReportPage();
                case "厂务石料生产":
                    return new StoneProducePage();
                case "设置数据矫正":
                    return new AdjustWeightPage();
                default:
                    return new DefaultPage();
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            MenuListBox.SelectedItem = null;
            UserPopup.IsOpen = !UserPopup.IsOpen;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lougout()
        {
            ModelHelper.GetInstance().GetApiData(ApiClient.LogoutAsync);
            var loginWin = new LoginWindow();
            Application.Current.MainWindow = loginWin;
            Close();
            loginWin.Show();
            loginWin.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChangePassWord()
        {
            UserPopup.IsOpen = false;
            var view = new ChangePasswordDialog();
            await DialogHost.Show(view, "RootDialog");
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MenuListBox.SelectedIndex)
            {
                case 0:
                    ChangePassWord();
                    break;
                case 1:
                    UserPopup.IsOpen = false;
                    PonderationHelper.GetInstance().ReRunPond();
                    break;
                case 2:
                    Lougout();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 跳转接口
        /// </summary>
        /// <param name="name"></param>
        public void JumpPage(string name)
        {
            if (!_roles.Contains(name))
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("没有权限!");
                return;
            }
            SwitchPage(name);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (!_roles.Contains(btn.Tag.ToString()))
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("没有权限!");
                    return;
                }
                SwitchPage(btn.Tag.ToString());
            }
        }

        private void Btn_AdjustPondEnabled_Click(object sender, RoutedEventArgs e)
        {
            Config.AdjustWeightEnabled = !Config.AdjustWeightEnabled;
            if (Config.AdjustWeightEnabled)
            {
                Btn_AdjustPondEnabled.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#668B8B"));
                if (!File.Exists("./AdjustWeightEnabled.awe"))
                {
                   var fs =  File.Create("./AdjustWeightEnabled.awe");
                    fs.Close();
                }
            }
            else
            {
                Btn_AdjustPondEnabled.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
                if (File.Exists("./AdjustWeightEnabled.awe"))
                    File.Delete("./AdjustWeightEnabled.awe");
            }
            Task.Run(()=> {
                var r = ModelHelper.GetInstance().GetApiDataArg(ApiClient.SetAdjustEnabled,
                    new { enable = Config.AdjustWeightEnabled ? 1 : 0 }).Result;
            });
            
        }

        private void Btn_AdjustPrintEnabled_Click(object sender, RoutedEventArgs e)
        {
            Config.AdjustPrintEnabled = !Config.AdjustPrintEnabled;
            if (Config.AdjustPrintEnabled)
            {
                Btn_AdjustPrintEnabled.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#668B8B"));
                if (!File.Exists("./AdjustPrintEnabled.awe"))
                {
                    var fs = File.Create("./AdjustPrintEnabled.awe");
                    fs.Close();
                }
            }
            else
            {
                Btn_AdjustPrintEnabled.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
                if (File.Exists("./AdjustPrintEnabled.awe"))
                    File.Delete("./AdjustPrintEnabled.awe");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // 关闭RFID进程
                //Common.GetProcByName("RFIDReader_LJYZN")?.Kill();
                RFIDHelper.GetInstance().CloseRFID();
            }
            catch { }
        }
    }
}
