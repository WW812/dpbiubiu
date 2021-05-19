using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.FactoryService;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.FactoryService.StoneProduce
{
    /// <summary>
    /// Interaction logic for CarOperateDialog.xaml
    /// </summary>
    public partial class CarOperateDialog : UserControl
    {
        private bool _rfidRead = false;
        private Process _rfidProc = new Process();
        private bool _isAdd = false;
        private StoneCarGroupModel scgm = null;
        private StoneCarModel scm = null;

        private readonly string[] _errorStr = { "未找到发卡器，请检查设备连接后重试!", "未知错误,请重试!" };

        private readonly ProcessStartInfo _rfidStartInfo = new ProcessStartInfo
        {
            FileName = "./RFID/RFIDReader.exe",
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        public CarOperateDialog(StoneCarGroupModel scgm)
        {
            InitializeComponent();
            if (scgm is null)
            {
                BiuMessageBoxWindows.BiuShow("参数传输错误!");
                DialogHost.CloseDialogCommand.Execute(true, this);
            }
            _isAdd = true;
            this.scgm = scgm;
            Btn_Submit.Content = "添加";
            Title.Text = "添加";
        }

        public CarOperateDialog(StoneCarGroupModel scgm, StoneCarModel scm)
        {
            InitializeComponent();
            if (scm is null || scgm is null)
            {
                BiuMessageBoxWindows.BiuShow("参数传输错误!");
                DialogHost.CloseDialogCommand.Execute(true, this);
            }
            this.scgm = scgm;
            this.scm = scm;
            _isAdd = false;
            Btn_Submit.Content = "修改";
            Title.Text = "修改";
            TB_CarID.Text = scm.CarID;
            TB_CarNumber.Text = scm.CarNumber;
            TB_UserName.Text = scm.User;
            TB_CarTare.Text = scm.CarTare.ToString();
            TB_RFIDCode.Text = scm.Rfid;
            TB_Contact.Text = scm.Contact;
            TB_CarType.Text = scm.CarType;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StartRFIDProc();
                ReadCode();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
                _rfidProc.Close();
            }
        }

        #region RFID相关

        /// <summary>
        /// 连接RFID，获取返回码
        /// </summary>
        /// <returns></returns>
        private void StartRFIDProc()
        {
            _rfidProc.StartInfo = _rfidStartInfo;
            _rfidProc.Start();
            _rfidRead = true;
        }

        /// <summary>
        /// 开始循环读码
        /// </summary>
        private void ReadCode()
        {
            Task.Run(() =>
            {
                while (_rfidRead)
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DecodeRFIDCode(_rfidProc.StandardOutput.ReadLine());
                        });
                        Thread.Sleep(80);
                    }
                    catch { }
                }
            });
        }

        /// <summary>
        /// 解析RFID返回码
        /// </summary>
        private void DecodeRFIDCode(string code)
        {
            if (code is null)
            {
            }
            // 得到卡内容
            else if (code.Contains("$$=="))
            {
                TB_RFIDCode.Text = code.Replace("$$==", "");
                Btn_Retry.Visibility = Visibility.Hidden;
            }
            // 未找到发卡器
            else if ("-1".Equals(code))
            {
                if(_isAdd) TB_RFIDCode.Text = _errorStr[0];
                Btn_Retry.Visibility = Visibility.Visible;
                CloseRFID();
            }
            // 未知异常
            else if (code.Contains("Error:"))
            {
                BiuMessageBoxWindows.BiuShow(code);
                if(_isAdd)TB_RFIDCode.Text = _errorStr[1];
                Btn_Retry.Visibility = Visibility.Visible;
                CloseRFID();
            }
            else
            {
                Btn_Retry.Visibility = Visibility.Hidden;
            }
        }

        private void Btn_Retry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseRFID();
                StartRFIDProc();
                ReadCode();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
                CloseRFID();
            }
        }

        private void CloseRFID()
        {
            _rfidRead = false;
            _rfidProc.Close();
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Process[] ps = Process.GetProcesses();//获取计算机上所有du进程zhi
            foreach (Process p in ps)
            {
                if ("RFIDReader".Equals(p.ProcessName))//判断进程名称dao
                {
                    p.Kill();//停止进程
                }
            }
        }

        private void Btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validate()) return;
                if (_isAdd)
                    AddCar();
                else
                    EditCar();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        private void AddCar()
        {
            var param = new StoneCarModel
            {
                TeamID = scgm.ID,
                User = TB_UserName.Text,
                CarID = TB_CarID.Text,
                CarNumber = TB_CarNumber.Text,
                CarTare = double.Parse(TB_CarTare.Text),
                Rfid = TB_RFIDCode.Text,
                CarType = TB_CarType.Text,
                Contact = TB_Contact.Text,
            };
            Task.Run(()=> {
                var rs = ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.AddStoneCarAsync,
                    param,
                    (DataInfo<StoneCarModel> success) => {
                        Application.Current.Dispatcher.Invoke(() => {
                            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                        });
                    }
                    ).Result;
            });
        }

        private void EditCar()
        {
            var param = new StoneCarModel
            {
                ID = scm.ID,
                TeamID = scgm.ID,
                User = TB_UserName.Text,
                CarID = TB_CarID.Text,
                CarNumber = TB_CarNumber.Text,
                CarTare = double.Parse(TB_CarTare.Text),
                Rfid = TB_RFIDCode.Text,
                CarType = TB_CarType.Text,
                Contact = TB_Contact.Text,
            };
            Task.Run(() => {
                var rs = ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.EditStoneCarAsync,
                    param,
                    (DataInfo<StoneCarModel> success) => {
                        Application.Current.Dispatcher.Invoke(() => {
                            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                        });
                    }
                    ).Result;
            });
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            if (string.IsNullOrEmpty(TB_CarID.Text))
            {
                BiuMessageBoxWindows.BiuShow("请填写车号!");
                return false;
            }

            if (string.IsNullOrEmpty(TB_CarTare.Text) || !RegexChecksum.IsNonnegativeReal(TB_CarTare.Text))
            {
                BiuMessageBoxWindows.BiuShow("请输入正确车辆皮重");
                return false;
            }

            if (string.IsNullOrEmpty(TB_RFIDCode.Text) || _errorStr.Contains(TB_RFIDCode.Text))
            {
                BiuMessageBoxWindows.BiuShow("请录入RFID码!");
                return false;
            }

            return true;
        }
    }
}
