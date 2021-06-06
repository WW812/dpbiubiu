using AutoUpdaterDotNET;
using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.model.user;
using biubiu.view_model.user;
using RuningInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WebApiClient;
using static biubiu.model.ModelHelper;

namespace biubiu.views.login
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _logining = false; //在登录中
        private readonly string _tableName = "account_history";
        private static readonly DispatcherTimer accountTimer = new DispatcherTimer();

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new UserViewModel();
            SQLiteHelper.CreateConnection();  //初始化SqlLite 连接
            accountTimer.Tick += new EventHandler(SearchAccountTimer);
            accountTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.REQUEST_DATETIME);
            var sql = "select account from " + _tableName + " ORDER by login_time DESC LIMIT 500";
            DataTable a = SQLiteHelper.ExecuteDataTable(sql);
            for (int i = 0; i < a.Rows.Count; i++)
            {
                AccountListBox.Items.Add(a.Rows[i]["account"]);
            }
        }

        /// <summary>
        /// 搜索账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchAccountTimer(object sender, EventArgs e)
        {
            AccountListBox.Items.Clear();
            var sql = "select account from " + _tableName + " WHERE account LIKE '%" + AccountTextBox.Text + "%' ORDER by login_time DESC LIMIT 500";
            DataTable a = SQLiteHelper.ExecuteDataTable(sql);
            for (int i = 0; i < a.Rows.Count; i++)
            {
                AccountListBox.Items.Add(a.Rows[i]["account"]);
            }
            accountTimer.Stop();
        }

        /// <summary>
        /// 自动更新
        /// </summary>
        public async void AutoUpdaterFunc()
        {
            try
            {
                DataInfo<String> r = await ApiClient.GetUpdateURL();
                Config.UPDATE_URL = r?.Data ?? "http://39.104.200.66/Version.xml";
                AutoUpdater.Start(Config.UPDATE_URL); //配置文件地址
                AutoUpdater.UpdateMode = Mode.Forced;  //更新类型
               //AutoUpdater.ReportErrors = true;
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600); //更新窗口大小
                //AutoUpdater.DownloadPath = "C:\\Users\\diu\\Desktop\\sdssf\\";
            }
            catch (Exception )
            {
                //BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        public string GetEdition()
        {
            try
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch
            {
                return "-.-.-";
            }
        }

        public async void Login()
        {
            // Sung 调试用
            //AccountTextBox.Text = "15612345678";
            //PassWordTextBox.Password = "123456";
            if (_logining) return;
            _logining = true;
            if (AccountTextBox.Text.Equals("") || PassWordTextBox.Password.Equals(""))
            {
                BiuMessageBoxWindows.BiuShow("请输入账号密码!"); 
                _logining = false;
                return;
            }
            LoginBtn.IsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.LoginAsync,
                new { UserName = AccountTextBox.Text, PassWord = PassWordTextBox.Password },
                delegate (DataInfo<User> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Config.COMPANY_ID = result.Data.CompanyId;
                        Config.COMPANY_NAME = result.Data.CompanyName;
                        Config.CURRENT_USER = result.Data;
                        UpdataLoginAccount(AccountTextBox.Text);
                        var mainWin = new MainWindow(this);
                        Application.Current.MainWindow = mainWin;
                        Close();
                        mainWin.Show();
                        mainWin.WindowState = WindowState.Maximized;
                    }));
                });
            LoginBtn.IsEnabled = true;
            _logining = false;
        }


        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void LoginBtn_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Login();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 更新用户登录
        /// </summary>
        /// <param name="username"></param>
        private void UpdataLoginAccount(string username)
        {
            var sql = "replace into account_history(account, login_time) VALUES('" + username + "', " + Common.DateTime2TimeStamp(DateTime.Now) + ")";
            SQLiteHelper.ExecuteNonQuery(sql);
        }

        private void SearchListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PassWordTextBox.Focus();
            }
        }

        /*
        private void SearchListBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            accountTimer.Start();
        }
        */

        private void AccountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            accountTimer.Start();
            AccountPopup.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = true;
        }

        private void AccountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = false;
        }

        private void AccountListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AccountListBox.SelectedItem == null) return;
                AccountTextBox.Text = AccountListBox.SelectedItem.ToString();
                AccountPopup.IsOpen = false;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void AccountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Run_Version.Text = GetEdition();
            /*
            if (Environment.Is64BitOperatingSystem)
            {
                Lab_Tip.Visibility = Visibility.Collapsed;
                // 自动更新配置
                AutoUpdaterFunc();
            }
            else
            {
                Lab_Tip.Visibility = Visibility.Visible;
            }
            */
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var sql = "delete from " + _tableName + " where account = "+ button.Tag.ToString();
            SQLiteHelper.ExecuteNonQuery(sql);
            AccountListBox.Items.Remove(button.Tag.ToString());
        }
    }
}
