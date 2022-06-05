using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.ship_order;
using biubiu.model.stock_order;
using biubiu.model.system;
using biubiu.model.user;
using Qiniu.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using wow;

namespace biubiu
{
    public class Config
    {
        //public static IApi ApiClient = HttpApiFactory.Create<IApi>();
        public static string ServerURL = "http://101.42.224.128:8080/weigh/";   // 远程服务器
        public const string UserURL = "user/";
        public const string ShipOrderURL = "orderSales/";
        public const string ShipCustomerURL = "customerSales/";
        public const string StockOrderURL = "orderStock/";
        public const string FinanceURL = "finance/";
        public const string FactoryURL = "stone/";
        public const string SystemSettingURL = "config/";
        public const string StockCustomerURL = "customerStock/";
        public const string DEVICE_CONFIG_PATH = "./device_config.ini";
        public const string BILL_CONFIG_PATH = "./bill_config.ini";
        public const string PONDS_PATH = "./ponds/";
        public const string REFER_SHIP_BILL = "./Reports/ReferShipBill.rdlc";
        public const string REFER_STOCK_BILL = "./Reports/ReferStockBill.rdlc";
        public const string SHIP_ORDER_REPORT = "./Reports/ShipOrderReport.rdlc";
        public const string STOCK_ORDER_REPORT = "./Reports/StockOrderReport.rdlc";
        public const string TEMP_PATH = "./temp/";
        public const string REGEXSTR_00 = @"^\d+(\.0{1,2})+?$";
        public const string ICON_PATH = "./logo.ico";
        public static string UPDATE_URL = "";
        public static string COMPANY_ID = "";
        public static string COMPANY_NAME = "";
        public static Window MAIN_WINDOWS;
        public const int PageSize = 15;
        public static User CURRENT_USER;
        public static SystemModel SYSTEM_SETTING;
        public static SQLiteConnection LiteConnection = SQLiteHelper.CreateConnection();
        public static System.DateTime StartTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        public static readonly int REQUEST_DATETIME = 800; // 多用于输入框文本变换发送请求的延迟毫秒数
        public static bool AdjustWeightEnabled = File.Exists("./AdjustWeightEnabled.awe"); 
        public static bool AdjustPrintEnabled = File.Exists("./AdjustPrintEnabled.awe"); 
        public static PonderationConfig P1 = new PonderationConfig("1号地磅");
        public static PonderationConfig P2 = new PonderationConfig("2号地磅");
        public static PonderationConfig P3 = new PonderationConfig("3号地磅");
        public static PonderationConfig P4 = new PonderationConfig("4号地磅");
        public static PondDataParameter PondDataP1;  // 地磅仪表参数
        public static PondDataParameter PondDataP2;
        public static PondDataParameter PondDataP3;
        public static PondDataParameter PondDataP4;
        public static BillConfig STOCK_ENTER_CONFIG = new BillConfig();
        public static BillConfig STOCK_EXIT_CONFIG = new BillConfig();
        public static BillConfig SHIP_ENTER_CONFIG = new BillConfig();
        public static BillConfig SHIP_EXIT_CONFIG = new BillConfig();
        //public static ModelHelper MODELHELPER = new ModelHelper();
        public static ShipOrderColumnsVisibility ShipColVisibility = new ShipOrderColumnsVisibility();
        public static StockOrderColumnsVisibility StockColVisibility = new StockOrderColumnsVisibility();
        public static Qiniu.Storage.Config QiniuUploadConfig = new Qiniu.Storage.Config //七牛云上传配置
        {
            Zone = Zone.ZONE_CN_North,
            UseHttps = true,
            UseCdnDomains = true,
            ChunkSize = ChunkUnit.U512K
        };
        public static Dictionary<string, string> _fileMutexPool = new Dictionary<string, string>();  // 配置文件读取

        public static string LOADING_EXE_PATH = "./";
        public static Snapshot SnapshotPicture = null;

        /*
        public static PonderSubject cs1 = new PonderSubject();
        public static PonderSubject cs2 = new PonderSubject();
        public static PonderSubject cs3 = new PonderSubject();
        public static PonderSubject cs4 = new PonderSubject();
        */
    }

    /// <summary>
    /// 地磅配置类
    /// 
    /// 各类型仪表的参数文件将 开始位 StartTag 提到第二行
    /// 
    /// </summary>
    public class PonderationConfig : IConfigModelInterface, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public bool Enable { get; set; }         // 是否启用
        private string _com;                        // 串口号
        public string COM
        {
            get { return _com; }
            set
            {
                _com = value;
                NotifyPropertyChanged("COM");
            }
        }
        private int _baudrate; // 波特率
        public int Baudrate
        {
            get
            {
                return _baudrate;
            }
            set
            {
                _baudrate = value;
                NotifyPropertyChanged("Baudrate");
            }
        }
        private int _dataBits;//数据位值 一般为8 SerialPort参数
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                _dataBits = value;
                NotifyPropertyChanged("DataBits");
            }
        }
        private string _parityValue; //SerialPort 参数
        public string ParityValue
        {
            get
            {
                return _parityValue;
            }
            set
            {
                _parityValue = value;
                NotifyPropertyChanged("ParityValue");
            }
        }
        private int _stopBitsValue;     //停止位
        public int StopBitsValue
        {
            get
            {
                return _stopBitsValue;
            }
            set
            {
                _stopBitsValue = value;
                NotifyPropertyChanged("StopBitsValue");
            }
        }
        private int _startBitsValue;    //开始位
        public int StartBitsValue
        {
            get
            {
                return _startBitsValue;
            }
            set
            {
                _startBitsValue = value;
                NotifyPropertyChanged("StartBitsValue");
            }
        }
        private string _pondTypeName; //表头类型名称
        public string PondTypeName
        {
            get { return _pondTypeName; }
            set
            {
                _pondTypeName = value;
                NotifyPropertyChanged("PondTypeName");
            }
        }
        public string RoadGateCOM { get; set; }   // 道闸COM口
        public string InRoadGateOpen { get; set; }  // 进厂道闸开命令
        public string InRoadGateClose { get; set; } // 进厂道闸关命令
        public string OutRoadGateOpen { get; set; }
        public string OutRoadGateClose { get; set; }
        public string InfraredCOM { get; set; }   // 进厂红外COM口
        public string InInfraredOrder { get; set; } // 进厂红外触发命令
        public string OutInfraredOrder { get; set; }
        public string CameraType { get; set; }    // 摄像机类型 
        public string CameraIP { get; set; }      // 硬盘刻录摄像头IP
        public string CameraIP1 { get; set; }      // 网络摄像头IP
        public string CameraIP2 { get; set; }      // 网络摄像头IP
        public string CameraIP3 { get; set; }      // 网络摄像头IP
        public bool WatchEnableIP1 { get; set; } // 是否启用录像功能(过磅界面查看录像)
        public bool WatchEnableIP2 { get; set; } // 是否启用录像功能(过磅界面查看录像)
        public bool WatchEnableIP3 { get; set; } // 是否启用录像功能(过磅界面查看录像)
        public int CameraPort { get; set; }    // 端口
        public string CameraUserName { get; set; } //用户名
        public string CameraPassWord { get; set; } //密码
        public int CameraChannel { get; set; } // 网络摄像头通道
        public int CameraChannel1 { get; set; } // 通道
        public int CameraChannel2 { get; set; } // 通道
        public int CameraChannel3 { get; set; } // 通道
        public bool CaptureEnable { get; set; } // 是否启用抓拍
        public bool WatchEnable { get; set; } // 是否启用录像功能(过磅界面查看录像)
        public double AdjustWeight { get; set; } // 地磅矫正数值


        public PonderationConfig(string name)
        {
            Name = name;
            Enable = false;
            COM = "COM1";
            Baudrate = 1200;
            DataBits = 8;
            ParityValue = "0";
            StopBitsValue = 1;
            StartBitsValue = 2;
            PondTypeName = "上海耀华XK3190系列1";
            RoadGateCOM = "COM1";
            InRoadGateOpen = "0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A";
            InRoadGateClose = "0x01,0x05,0x00,0x00,0x00,0x00,0xCD,0xCA";
            OutRoadGateOpen = "0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A";
            OutRoadGateClose = "0x01,0x05,0x00,0x00,0x00,0x00,0xCD,0xCA";
            InfraredCOM = "COM1";
            InInfraredOrder = "0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A";
            OutInfraredOrder = "0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A";
            CameraType = "海康威视硬盘刻录机";
            CameraIP = "127.0.0.1";
            CameraIP1 = "127.0.0.1";
            CameraIP2 = "127.0.0.1";
            CameraIP3 = "127.0.0.1";
            CameraPort = 8000;
            CameraUserName = "username";
            CameraPassWord = "password";
            CameraChannel = 1;
            CameraChannel1 = 33;
            CameraChannel2 = 33;
            CameraChannel3 = 33;
            CaptureEnable = false;
            WatchEnable = false;
            WatchEnableIP1 = false;
            WatchEnableIP2 = false;
            WatchEnableIP3 = false;
            AdjustWeight = 0.0;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="iniClass"></param>
        /// <param name="section"></param>
        public void WriteToFile(INIClass iniClass, string section)
        {
            iniClass.IniWriteValue(section, "Enable", Enable.ToString());
            iniClass.IniWriteValue(section, "COM", COM);
            iniClass.IniWriteValue(section, "Baudrate", Baudrate.ToString());
            iniClass.IniWriteValue(section, "DataBits", DataBits.ToString());
            iniClass.IniWriteValue(section, "ParityValue", ParityValue.ToString());
            iniClass.IniWriteValue(section, "StopBitsValue", StopBitsValue.ToString());
            iniClass.IniWriteValue(section, "StartBitsValue", StartBitsValue.ToString());
            iniClass.IniWriteValue(section, "PondTypeName", PondTypeName.ToString());
            iniClass.IniWriteValue(section, "RoadGateCOM", RoadGateCOM.ToString());
            iniClass.IniWriteValue(section, "InRoadGateOpen", InRoadGateOpen.ToString());
            iniClass.IniWriteValue(section, "InRoadGateClose", InRoadGateClose.ToString());
            iniClass.IniWriteValue(section, "OutRoadGateOpen", OutRoadGateOpen.ToString());
            iniClass.IniWriteValue(section, "OutRoadGateClose", OutRoadGateClose.ToString());
            iniClass.IniWriteValue(section, "InfraredCOM", InfraredCOM.ToString());
            iniClass.IniWriteValue(section, "InInfraredOrder", InInfraredOrder.ToString());
            iniClass.IniWriteValue(section, "OutInfraredOrder", OutInfraredOrder.ToString());
            iniClass.IniWriteValue(section, "CameraType", CameraType);
            iniClass.IniWriteValue(section, "CameraIP", CameraIP.ToString());
            iniClass.IniWriteValue(section, "CameraIP1", CameraIP1.ToString());
            iniClass.IniWriteValue(section, "CameraIP2", CameraIP2.ToString());
            iniClass.IniWriteValue(section, "CameraIP3", CameraIP3.ToString());
            iniClass.IniWriteValue(section, "CameraPort", CameraPort.ToString());
            iniClass.IniWriteValue(section, "CameraUserName", CameraUserName.ToString());
            iniClass.IniWriteValue(section, "CameraPassWord", CameraPassWord.ToString());
            iniClass.IniWriteValue(section, "CameraChannel", CameraChannel.ToString());
            iniClass.IniWriteValue(section, "CameraChannel1", CameraChannel1.ToString());
            iniClass.IniWriteValue(section, "CameraChannel2", CameraChannel2.ToString());
            iniClass.IniWriteValue(section, "CameraChannel3", CameraChannel3.ToString());
            iniClass.IniWriteValue(section, "CaptureEnable", CaptureEnable.ToString());
            iniClass.IniWriteValue(section, "WatchEnable", WatchEnable.ToString());
            iniClass.IniWriteValue(section, "WatchEnableIP1", WatchEnableIP1.ToString());
            iniClass.IniWriteValue(section, "WatchEnableIP2", WatchEnableIP2.ToString());
            iniClass.IniWriteValue(section, "WatchEnableIP3", WatchEnableIP3.ToString());
            iniClass.IniWriteValue(section, "AdjustWeight", AdjustWeight.ToString());
        }

        /// <summary>
        /// 从文件读取
        /// </summary>
        /// <param name="iniClass"></param>
        /// <param name="section"></param>
        public void ReadFromFile(INIClass iniClass, string section)
        {
            Enable = ("True" == iniClass.IniReadValue(section, "Enable"));
            COM = iniClass.IniReadValue(section, "COM");
            Baudrate = Int32.Parse(iniClass.IniReadValue(section, "Baudrate"));
            DataBits = Int32.Parse(iniClass.IniReadValue(section, "DataBits"));
            ParityValue = iniClass.IniReadValue(section, "ParityValue");
            StopBitsValue = Int32.Parse(iniClass.IniReadValue(section, "StopBitsValue"));
            StartBitsValue = Int32.Parse(iniClass.IniReadValue(section, "StartBitsValue"));
            PondTypeName = iniClass.IniReadValue(section, "PondTypeName");
            RoadGateCOM = iniClass.IniReadValue(section, "RoadGateCOM");
            InRoadGateOpen = iniClass.IniReadValue(section, "InRoadGateOpen");
            InRoadGateClose = iniClass.IniReadValue(section, "InRoadGateClose");
            OutRoadGateOpen = iniClass.IniReadValue(section, "OutRoadGateOpen");
            OutRoadGateClose = iniClass.IniReadValue(section, "OutRoadGateClose");
            InfraredCOM = iniClass.IniReadValue(section, "InfraredCOM");
            InInfraredOrder = iniClass.IniReadValue(section, "InInfraredOrder");
            OutInfraredOrder = iniClass.IniReadValue(section, "OutInfraredOrder");
            CameraType = iniClass.IniReadValue(section, "CameraType");
            CameraIP = iniClass.IniReadValue(section, "CameraIP");
            CameraIP1 = iniClass.IniReadValue(section, "CameraIP1");
            CameraIP2 = iniClass.IniReadValue(section, "CameraIP2");
            CameraIP3 = iniClass.IniReadValue(section, "CameraIP3");
            CameraPort = Int32.Parse(iniClass.IniReadValue(section, "CameraPort"));
            CameraUserName = iniClass.IniReadValue(section, "CameraUserName");
            CameraPassWord = iniClass.IniReadValue(section, "CameraPassWord");
            CameraChannel = Int32.Parse(iniClass.IniReadValue(section, "CameraChannel"));
            CameraChannel1 = Int32.Parse(iniClass.IniReadValue(section, "CameraChannel1"));
            CameraChannel2 = Int32.Parse(iniClass.IniReadValue(section, "CameraChannel2"));
            CameraChannel3 = Int32.Parse(iniClass.IniReadValue(section, "CameraChannel3"));
            CaptureEnable = ("True" == iniClass.IniReadValue(section, "CaptureEnable"));
            WatchEnable = ("True" == iniClass.IniReadValue(section, "WatchEnable"));
            WatchEnableIP1 = ("True" == iniClass.IniReadValue(section, "WatchEnableIP1"));
            WatchEnableIP2 = ("True" == iniClass.IniReadValue(section, "WatchEnableIP2"));
            WatchEnableIP3 = ("True" == iniClass.IniReadValue(section, "WatchEnableIP3"));
            var adjWeight = iniClass.IniReadValue(section, "AdjustWeight");
            AdjustWeight = Double.Parse(string.IsNullOrEmpty(adjWeight)? "0.0" : adjWeight);
        }

        /// <summary>
        /// 返回PondData参数
        /// </summary>
        /// <returns></returns>
        public PondDataParameter GetPondDataParameter()
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            var _fileName = Config.PONDS_PATH + PondTypeName + ".pp";
            var _waitSecond = 200;
            try
            {
                while (Config._fileMutexPool.ContainsKey(_fileName))
                { //检测占用信号量，如果占用，等待
                    if (_waitSecond > 10000) throw new Exception("参数文件被占用!");
                    Thread.Sleep(200);
                    _waitSecond += 200;
                }
                Config._fileMutexPool.Add(_fileName, _fileName);

                if (!File.Exists(_fileName)) throw new Exception("参数文件不存在!");
                fileStream = File.Open(Config.PONDS_PATH + PondTypeName + ".pp", FileMode.Open);
                streamReader = new StreamReader(fileStream);
                string[] strArr = streamReader.ReadLine().Trim().Split(new string[1] { "," }, StringSplitOptions.None); //读取 波特率，等
                //Baudrate = Int32.Parse(strArr[0]);  //波特率
                ParityValue = strArr[1];
                DataBits = Int32.Parse(strArr[2]);  //数据位
                StopBitsValue = Int32.Parse(strArr[3]); //停止位
                StartBitsValue = Int32.Parse(streamReader.ReadLine().Trim()); //开始位
                var _p = new PondDataParameter
                {
                    StartTag = StartBitsValue,
                    TimeOut = Int32.Parse(streamReader.ReadLine().Trim()),
                    ReceiveNumber = Int32.Parse(streamReader.ReadLine().Trim()),
                    Fushuwei = Int32.Parse(streamReader.ReadLine().Trim()),
                    Dujiwei = Int32.Parse(streamReader.ReadLine().Trim()),
                    Kaishidu = Int32.Parse(streamReader.ReadLine().Trim()),
                    Zhengfanxu = Int32.Parse(streamReader.ReadLine().Trim()),
                    Fu = Int32.Parse(streamReader.ReadLine().Trim())
                };
                fileStream.Close();
                streamReader.Close();
                Config._fileMutexPool.Remove(_fileName);
                return _p;
            }
            catch (Exception er)
            {
                fileStream.Close();
                streamReader.Close();
                Config._fileMutexPool.Remove(_fileName);
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
            return new PondDataParameter();
        }

        public SerialPort GetSerialPort()
        {
            var parity = Parity.None;
            var stopBits = StopBits.One;
            switch (ParityValue)
            {
                case "1":
                    parity = Parity.Odd;
                    break;
                case "2":
                    parity = Parity.Even;
                    break;
                case "3":
                    parity = Parity.Mark;
                    break;
                case "4":
                    parity = Parity.Space;
                    break;
            }
            switch (StopBitsValue)
            {
                case 2:
                    stopBits = StopBits.Two;
                    break;
                case 3:
                    stopBits = StopBits.OnePointFive;
                    break;
            }
            return new SerialPort
            {
                PortName = COM,
                BaudRate = Baudrate,
                Parity = parity,
                DataBits = DataBits,
                StopBits = stopBits
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 过磅界面的四个地磅对象
    /// </summary>
    public class PonderationDisplay : INotifyPropertyChanged
    {
        /// <summary>
        /// 地磅配置
        /// </summary>
        public PonderationConfig PondConfig;

        /// <summary>
        /// 地磅名称
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 重量
        /// </summary>
        private string _weight = "0.0";
        public string Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                NotifyPropertyChanged("Weight");
            }
        }

        private bool _isError = false;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                NotifyPropertyChanged("IsError");
            }
        }

        /// <summary>
        /// 数据是否稳定
        /// </summary>
        private bool _isStability = true;
        public bool IsStability
        {
            get { return _isStability; }
            set
            {
                _isStability = value;
                NotifyPropertyChanged("IsStability");
            }
        }

        /// <summary>
        /// 重量集合，数据稳定用
        /// </summary>
        public List<string> WeightList = new List<string>();

        #region 控件样式控制

        private int _width = 145;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyPropertyChanged("Width");
            }
        }

        private int _height = 100;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                NotifyPropertyChanged("Height");
            }
        }

        private SolidColorBrush _background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4e4c4c"));
        public SolidColorBrush Background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            }
        }

        #endregion

        public PonderationDisplay(string name)
        {
            Name = name;
        }

        public void Selected()
        {
            Width = 151;
            Height = 106;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
        }

        public void UnSelected()
        {
            Width = 145;
            Height = 100;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#404040"));
            //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4e4c4c"));
        }

        public void Reset()
        {
            //Weight = "禁用";
            Weight = "0.0";
            IsStability = true;
            WeightList.Clear();
        }

        public void Error()
        {
            //Weight = "出错";
            Weight = "0.0";
            IsError = true;
            //IsStability = false;
            WeightList.Clear();
        }

        public void Awake()
        {
            EventCenter.AddListener<PonderEventInfomation>(EventType.Ponder, Show);
        }

        public void OnDestory()
        {
            EventCenter.RemoveListener<PonderEventInfomation>(EventType.Ponder, Show);
            Reset();
        }

        public void Show(PonderEventInfomation pei)
        {
            if (pei.Name == Name)
            {
                if (pei.Reset)
                {
                    Reset();
                }
                else if (!string.IsNullOrWhiteSpace(pei.Error) && !IsError)
                {
                    BiuMessageBoxWindows.BiuShow(pei.Name + pei.Error, image: BiuMessageBoxImage.Error);
                    Error();
                    return;
                }
                else
                {
                    var isStab = true;
                    WeightList.ForEach(delegate (string str)
                    {
                        if (str != pei.Weight) isStab = false;
                    });
                    IsStability = isStab;
                    Weight = pei.Weight; // 非展示用
                    WeightList.Add(pei.Weight);
                    if (WeightList.Count > 5) WeightList.RemoveAt(0);
                    IsError = false;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
