using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wow;

namespace biubiu.view_model.device
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        private PonderationConfig _p1;
        public PonderationConfig P1
        {
            get
            {
                return _p1;
            }
            set
            {
                _p1 = value;
                NotifyPropertyChanged("P1");
            }
        }
        private PonderationConfig _p2;
        public PonderationConfig P2
        {
            get
            {
                return _p2;
            }
            set
            {
                _p2 = value;
                NotifyPropertyChanged("P2");
            }
        }
        private PonderationConfig _p3;
        public PonderationConfig P3
        {
            get
            {
                return _p3;
            }
            set
            {
                _p3 = value;
                NotifyPropertyChanged("P3");
            }
        }
        private PonderationConfig _p4;
        public PonderationConfig P4
        {
            get
            {
                return _p4;
            }
            set
            {
                _p4 = value;
                NotifyPropertyChanged("P4");
            }
        }

        /// <summary>
        /// 打开pp文件
        /// </summary>
        private FileStream fileStream; //打开pp文件
        private StreamReader streamReader;  //文件读取流

        /// <summary>
        /// 地磅集合
        /// </summary>
        private ObservableCollection<PonderationConfig> _pondItems;
        public ObservableCollection<PonderationConfig> PondItems
        {
            get
            {
                return _pondItems;
            }
            set
            {
                _pondItems = value;
                NotifyPropertyChanged("PondItems");
            }
        }

        /// <summary>
        /// 当前选择第几号地磅
        /// </summary>
        private PonderationConfig _currentPond;
        public PonderationConfig CurrentPond
        {
            get
            {
                return _currentPond;
            }
            set
            {
                _currentPond = value;
                NotifyPropertyChanged("CurrentPond");
            }
        }

        /// <summary>
        /// 数据展示
        /// </summary>
        private string _showData;
        public string ShowData
        {
            get
            {
                return _showData;
            }
            set
            {
                _showData = value;
                NotifyPropertyChanged("ShowData");
            }
        }

        /// <summary>
        /// 表头类型集合
        /// </summary>
        private ObservableCollection<string> _pondsTypeNameItems;
        public ObservableCollection<string> PondsTypeNameItems
        {
            get
            {
                return _pondsTypeNameItems;
            }
            set
            {
                _pondsTypeNameItems = value;
                NotifyPropertyChanged("PondsTypeNameItems");
            }
        }

        /// <summary>
        /// 串口集合
        /// </summary>
        private ObservableCollection<string> _serialPortItems = new ObservableCollection<string>();
        public ObservableCollection<string> SerialPortItems
        {
            get
            {
                return _serialPortItems;
            }
            set
            {
                _serialPortItems = value;
                NotifyPropertyChanged("SerialPortItems");
            }
        }

        /// <summary>
        /// 波特率集合
        /// </summary>
        private ObservableCollection<int> _baudrateItems = new ObservableCollection<int>
                {
                    300,600,900,1200,2400,4800,9600,14400,19200
                };
        public ObservableCollection<int> BaudrateItems
        {
            get
            {
                return _baudrateItems;
            }
            set
            {
                _baudrateItems = value;
                NotifyPropertyChanged("BaudrateItems");
            }
        }

        /// <summary>
        /// 仪表参数
        /// </summary>
        private PondDataParameter _pondParameter;
        public PondDataParameter PondParameter
        {
            get
            {
                return _pondParameter;
            }
            set
            {
                _pondParameter = value;
                NotifyPropertyChanged("PondParameter");
            }
        }

        /// <summary>
        /// 是否进行连接测试
        /// </summary>
        private Boolean _isConnectionTest = false;
        public Boolean IsConnectionTest
        {
            get
            {
                return _isConnectionTest;
            }
            set
            {
                _isConnectionTest = value;
                NotifyPropertyChanged("IsConnectionTest");
            }
        }

        private string _testBtnText = "接收数据";
        public string TestBtnText
        {
            get { return _testBtnText; }
            set
            {
                _testBtnText = value;
                NotifyPropertyChanged("TestBtnText");
            }
        }

        private string _saveWarningStr = "";
        public string SaveWarningStr
        {
            get { return _saveWarningStr; }
            set
            {
                _saveWarningStr = value;
                NotifyPropertyChanged("SaveWarningStr");
            }
        }

        public SerialPort sPort;
        public PonderationCommon pondCommon;

        private string _snackbarMessage = "";
        public string SnackbarMessage
        {
            get { return _snackbarMessage; }
            set
            {
                _snackbarMessage = value;
                NotifyPropertyChanged("SnackbarMessage");
            }
        }

        private Boolean _snackbarIsActive = false;
        public Boolean SnackbarIsActive
        {
            get { return _snackbarIsActive; }
            set
            {
                _snackbarIsActive = value;
                NotifyPropertyChanged("SnackbarIsActive");
            }
        }

        /*
        #region 摄像头属性

        /// <summary>
        /// 是否登陆成功
        /// </summary>
        public bool _isLogin = false;
        public bool IsLogin
        {
            get { return _isLogin; }
            set
            {
                _isLogin = value;
                NotifyPropertyChanged("IsLogin");
            }
        }

        /// <summary>
        /// 登陆按钮字体内容
        /// </summary>
        public string _cameraLogBtnText = "登录";
        public string CameraLogBtnText
        {
            get { return _cameraLogBtnText; }
            set
            {
                _cameraLogBtnText = value;
                NotifyPropertyChanged("CameraLogBtnText");
            }
        }

        /// <summary>
        /// 登录成功后，当前可用通道列表
        /// </summary>
        public ObservableCollection<int> _channelItems;
        public ObservableCollection<int> ChannelItems
        {
            get { return _channelItems; }
            set
            {
                _channelItems = value;
                NotifyPropertyChanged("ChannelItems");
            }
        }

        #endregion
    */

        /// <summary>
        /// 底部弹窗
        /// </summary>
        /*
        private SnackbarViewModel _messageBar = new SnackbarViewModel();
        public SnackbarViewModel MessageBar
        {
            get { return _messageBar; }
            set
            {
                _messageBar = value;
                NotifyPropertyChanged("MessageBar");
            }
        }
        */

        /// <summary>
        /// 当前选中的摄像机类型Tab
        /// </summary>
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        /// <summary>
        /// 摄像头类型名称集合, 可配合TabIndex使用
        /// </summary>
        private readonly List<string> CameraTypeList = new List<string> { "海康威视硬盘刻录机", "海康威视网络摄像头" };

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceViewModel()
        {
            SetItemsSource();
            P1 = Config.P1;
            P2 = Config.P2;
            P3 = Config.P3;
            P4 = Config.P4;
            CurrentPond = P1;
            PondItems = new ObservableCollection<PonderationConfig> { P1, P2, P3, P4 };
            SetPondsTypeName();
        }

        /// <summary>
        /// 设置各项ItemsSource
        /// </summary>
        private void SetItemsSource()
        {
            SerialPortItems.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                SerialPortItems.Add(item);
            }
        }

        /// <summary>
        /// 写入到文件
        /// </summary>
        /// <param name="p"></param>
        /// <param name="section"></param>
        public void WriteChangeToFile(PonderationConfig p, string section)
        {
            p.WriteToFile(new INIClass(Config.DEVICE_CONFIG_PATH), section);
        }

        public ICommand SaveCommand => new AnotherCommandImplementation(ExecuteSave);
        public ICommand SaveCameraTypeCommand => new AnotherCommandImplementation(ExecuteSaveCameraType);
        public ICommand SaveWatchEnabledCommand => new AnotherCommandImplementation(ExecuteSaveWatchEnabledCommand);
        public ICommand ConnectionTest => new AnotherCommandImplementation(ExecuteConnectionTest);
        public ICommand SaveWatchEnabledIP1Command => new AnotherCommandImplementation(ExecuteSaveWatchEnabledIP1Command);
        public ICommand SaveWatchEnabledIP2Command => new AnotherCommandImplementation(ExecuteSaveWatchEnabledIP2Command);
        public ICommand SaveWatchEnabledIP3Command => new AnotherCommandImplementation(ExecuteSaveWatchEnabledIP3Command);

        /// <summary>
        ///  执行存储
        /// </summary>
        /// <param name="o">弹出提示为true 不提示为false</param>
        public void ExecuteSave(object o)
        {
            try
            {
                Task.Run(() =>
                {
                    switch (CurrentPond.Name.ToString())
                    {
                        case "1号地磅":
                            WriteChangeToFile(Config.P1, "1Ponderation");
                            break;
                        case "2号地磅":
                            WriteChangeToFile(Config.P2, "2Ponderation");
                            break;
                        case "3号地磅":
                            WriteChangeToFile(Config.P3, "3Ponderation");
                            break;
                        case "4号地磅":
                            WriteChangeToFile(Config.P4, "4Ponderation");
                            break;
                    }

                    if ("true".Equals(o.ToString().ToLower()))
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
                    }
                });
            }
            catch (Exception er)
            {
                SaveWarningStr = "保存失败，原因：" + er.Message;
            }
        }
        private void ExecuteConnectionTest(object o)
        {
            IsConnectionTest = !IsConnectionTest;
            if (IsConnectionTest)
            {

                sPort = CurrentPond.GetSerialPort();
                pondCommon = new PonderationCommon(sPort, CurrentPond.PondTypeName, PondParameter);
                Task.Run(() =>
                {
                    try
                    {
                        if (!sPort.IsOpen) sPort.Open();
                        while (IsConnectionTest)
                        {
                            var data = pondCommon.Run();
                            if (data != "") ShowData = data;
                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(er.Message);
                        IsConnectionTest = false;
                    }
                    finally
                    {
                        sPort.Close();
                        ShowData = "";
                    }
                });
            }
            else
            {
                ShowData = "";
            }

        }
        /// <summary>
        /// 存储录像机类型,与是否启用抓拍
        /// </summary>
        /// <param name="o">录像机类型</param>
        private void ExecuteSaveCameraType(object o)
        {
            CurrentPond.CaptureEnable = (bool)o;
            CurrentPond.CameraType = CameraTypeList[SelectedTabIndex];
            ExecuteSave(false);
        }
        private void ExecuteSaveWatchEnabledCommand(object o)
        {
            CurrentPond.WatchEnable = (bool)o;
            CurrentPond.CameraType = CameraTypeList[SelectedTabIndex];
            ExecuteSave(false);
        }
        private void ExecuteSaveWatchEnabledIP1Command(object o)
        {
            CurrentPond.WatchEnableIP1 = (bool)o;
            CurrentPond.CameraType = CameraTypeList[SelectedTabIndex];
            ExecuteSave(false);
        }
        private void ExecuteSaveWatchEnabledIP2Command(object o)
        {
            CurrentPond.WatchEnableIP2 = (bool)o;
            CurrentPond.CameraType = CameraTypeList[SelectedTabIndex];
            ExecuteSave(false);
        }
        private void ExecuteSaveWatchEnabledIP3Command(object o)
        {
            CurrentPond.WatchEnableIP3 = (bool)o;
            CurrentPond.CameraType = CameraTypeList[SelectedTabIndex];
            ExecuteSave(false);
        }

        /// <summary>
        /// 设置
        /// </summary>
        private void SetPondsTypeName()
        {
            PondsTypeNameItems = new ObservableCollection<string>();
            Task.Run(() =>
            {
                string[] files = Directory.GetFiles(Config.PONDS_PATH, "*.pp");

                foreach (var file in files)
                {
                    PondsTypeNameItems.Add(Path.GetFileName(file).Replace(".pp", ""));
                }
            });
        }

        /// <summary>
        /// 读取指定参数文件
        /// </summary>
        private void SetCurrentPondByFile()
        {
            var _fileName = Config.PONDS_PATH + CurrentPond.PondTypeName + ".pp";
            try
            {
                var _waitSecond = 200;
                while (Config._fileMutexPool.ContainsKey(_fileName))
                { //检测占用信号量，如果占用，等待
                    if (_waitSecond > 10000) throw new Exception("参数文件被占用!");
                    Thread.Sleep(200);
                    _waitSecond += 200;
                }
                Config._fileMutexPool.Add(_fileName, _fileName);
                if (!File.Exists(_fileName)) throw new Exception("参数文件不存在!");
                fileStream = File.Open(_fileName, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                string[] strArr = streamReader.ReadLine().Trim().Split(new string[1] { "," }, StringSplitOptions.None); //读取 波特率，等
                CurrentPond.Baudrate = Int32.Parse(strArr[0]);  //波特率
                CurrentPond.ParityValue = strArr[1];
                CurrentPond.DataBits = Int32.Parse(strArr[2]);  //数据位
                CurrentPond.StopBitsValue = Int32.Parse(strArr[3]); //停止位
                CurrentPond.StartBitsValue = Int32.Parse(streamReader.ReadLine().Trim()); //开始位
                PondParameter = new PondDataParameter
                {
                    StartTag = CurrentPond.StartBitsValue,
                    TimeOut = Int32.Parse(streamReader.ReadLine().Trim()),
                    ReceiveNumber = Int32.Parse(streamReader.ReadLine().Trim()),
                    Fushuwei = Int32.Parse(streamReader.ReadLine().Trim()),
                    Dujiwei = Int32.Parse(streamReader.ReadLine().Trim()),
                    Kaishidu = Int32.Parse(streamReader.ReadLine().Trim()),
                    Zhengfanxu = Int32.Parse(streamReader.ReadLine().Trim()),
                    Fu = Int32.Parse(streamReader.ReadLine().Trim())
                };
                switch (CurrentPond.Name.ToString())
                {
                    case "1号地磅":
                        Config.PondDataP1 = PondParameter;
                        break;
                    case "2号地磅":
                        Config.PondDataP2 = PondParameter;
                        break;
                    case "3号地磅":
                        Config.PondDataP3 = PondParameter;
                        break;
                    case "4号地磅":
                        Config.PondDataP4 = PondParameter;
                        break;
                }
                fileStream.Close();
                streamReader.Close();
                Config._fileMutexPool.Remove(_fileName);
            }
            catch (Exception er)
            {
                fileStream.Close();
                streamReader.Close();
                Config._fileMutexPool.Remove(_fileName);
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }

        }

        /// <summary>
        /// NotifyPropertyChanged事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "IsConnectionTest":
                    TestBtnText = IsConnectionTest ? "停止接收" : "接收数据";
                    break;
                case "CurrentPondsTypeName":
                    //仪表名变换 拿取配置
                    SetCurrentPondByFile();
                    break;
                case "CurrentPond":
                    IsConnectionTest = false;
                    SaveWarningStr = "";
                    SetItemsSource();
                    break;
                    /*
                case "IsLogin":
                    CameraLogBtnText = IsLogin ? "退出" : "登录";
                    break;
                    */
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
