using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.view_model.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

namespace biubiu.views.setting.device.camera
{
    /// <summary>
    /// Interaction logic for Hikvision2Control.xaml
    /// </summary>
    public partial class Hikvision2Control : UserControl
    {
        private bool m_bInitSDK = false;
        private List<Int32> m_lUserIDList = new List<Int32> { -1, -1, -1 };
        private uint iLastErr = 0;
        private string str;
        private Int32 m_lRealHandle1 = -1;
        private Int32 m_lRealHandle2 = -1;
        private Int32 m_lRealHandle3 = -1;
        //private CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
        public CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode;
        public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
        /*
        private string str1;
        private string str2;
        */
        private Int32 i = 0;
        //private Int32 m_lTree = 0;
        private uint dwAChanTotalNum = 0;
        private uint dwDChanTotalNum = 0;
        private int[] iIPDevID = new int[96];
        private int[] iChannelNum = new int[96];
        //private long iSelIndex = 0;

        public PonderationConfig CurrentPond2
        {
            get { return (PonderationConfig)GetValue(CurrentPond2Property); }
            set { SetValue(CurrentPond2Property, value); }
        }
        public static readonly DependencyProperty CurrentPond2Property =
           DependencyProperty.Register("CurrentPond2", typeof(PonderationConfig), typeof(UserControl), new PropertyMetadata());

        public Hikvision2Control()
        {
            InitializeComponent();
        }

        public void Logout()
        {
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle1);
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle2);
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle3);
            m_lRealHandle1 = -1;
            m_lRealHandle2 = -1;
            m_lRealHandle3 = -1;
            RealPlayWnd1.Refresh();
            RealPlayWnd2.Refresh();
            RealPlayWnd3.Refresh();
            //注销登录
            for (int i = 0; i < 3; i++)
            {
                if (m_lUserIDList[i] >= 0)
                {
                    CHCNetSDK.NET_DVR_Logout(m_lUserIDList[i]);
                    m_lUserIDList[i] = -1;
                }
            }
            CHCNetSDK.NET_DVR_Cleanup();
        }

        public void Logout(ref Int32 m_lRealHandle, System.Windows.Forms.PictureBox playWnd, int UserIDIndex)
        {
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            m_lRealHandle = -1;
            playWnd.Refresh();
            CHCNetSDK.NET_DVR_Logout(m_lUserIDList[UserIDIndex]);
            CHCNetSDK.NET_DVR_Cleanup();
        }

        private void NET_DVR_Init()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                BiuMessageBoxWindows.BiuShow("摄像头初始化失败!",image:BiuMessageBoxImage.Error);
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        /// <summary>
        /// 网络摄像头登录
        /// </summary>
        /// <param name="ip">摄像头IP</param>
        /// <param name="playWnd">视频句柄</param>
        /// <param name="m_lRealHandle"></param>
        private void Login(string ip, int userID, System.Windows.Forms.PictureBox playWnd, ref Int32 m_lRealHandle)
        {
            NET_DVR_Init(); //初始化

            if (m_lUserIDList[userID] >= 0)
            {
                Logout(ref m_lRealHandle, playWnd, userID);
            }
            //if (m_lUserIDList[userID] < 0)
            {
                string DVRIPAddress = ip;//IPTextBox.Text; //设备IP地址或者域名
                Int16 DVRPortNumber = System.Convert.ToInt16(PortTextBox.Text);//设备服务端口号
                string DVRUserName = UserNameTextBox.Text;//设备登录用户名
                string DVRPassword = PasswordTextBox.Text;//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserIDList[userID] = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserIDList[userID] < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "登录失败, 错误码= " + iLastErr; //登录失败，输出错误号
                    BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                }
                else
                {
                    //登录成功
                    DebugInfo("NET_DVR_Login_V30 succ!");
                    // 保存

                    dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                    dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
                    if (dwDChanTotalNum > 0)
                    {
                        InfoIPChannel(m_lUserIDList[userID]);
                    }
                    else
                    {
                        for (i = 0; i < dwAChanTotalNum; i++)
                        {
                            iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                        }
                    }

                    if (m_lRealHandle < 0)
                    {
                        CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                        lpPreviewInfo.hPlayWnd = playWnd.Handle;//预览窗口
                        lpPreviewInfo.lChannel = 1;//预te览的设备通道
                        lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                        lpPreviewInfo.dwLinkMode = 1;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                        lpPreviewInfo.bBlocked = false; //0- 非阻塞取流，1- 阻塞取流
                        lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数

                        CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                                                                                                               //IntPtr pUser = new IntPtr();//用户数据
                        IntPtr pUser = IntPtr.Zero;

                        //打开预览 Start live view 
                        m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserIDList[userID], ref lpPreviewInfo, null, pUser);
                        if (m_lRealHandle < 0)
                        {
                            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                            str = "预览失败, 错误码= " + iLastErr; //预览失败，输出错误号
                            BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            //预览成功 保存
                            (DataContext as DeviceViewModel).ExecuteSave(false);
                        }
                    }
                }
                (DataContext as DeviceViewModel).ExecuteSave(false);
            }
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }

        public void InfoIPChannel(Int32 m_lUserID)
        {
            uint dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40);

            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
            Marshal.StructureToPtr(m_struIpParaCfgV40, ptrIpParaCfgV40, false);

            uint dwReturn = 0;
            int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_GET_IPPARACFG_V40 failed, error code= " + iLastErr;
                //获取IP资源配置信息失败，输出错误号 Failed to get configuration of IP channels and output the error code
                DebugInfo(str);
            }
            else
            {
                DebugInfo("NET_DVR_GET_IPPARACFG_V40 succ!");

                m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));

                for (i = 0; i < dwAChanTotalNum; i++)
                {
                    iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                }

                byte byStreamType = 0;
                uint iDChanNum = 64;

                if (dwDChanTotalNum < 64)
                {
                    iDChanNum = dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                }

                for (i = 0; i < iDChanNum; i++)
                {
                    iChannelNum[i + dwAChanTotalNum] = i + (int)m_struIpParaCfgV40.dwStartDChan;
                    byStreamType = m_struIpParaCfgV40.struStreamMode[i].byGetStreamType;

                    dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40.struStreamMode[i].uGetStream);
                    switch (byStreamType)
                    {
                        //目前NVR仅支持直接从设备取流 NVR supports only the mode: get stream from device directly
                        case 0:
                            IntPtr ptrChanInfo = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfo, false);
                            m_struChanInfo = (CHCNetSDK.NET_DVR_IPCHANINFO)Marshal.PtrToStructure(ptrChanInfo, typeof(CHCNetSDK.NET_DVR_IPCHANINFO));

                            iIPDevID[i] = m_struChanInfo.byIPID + m_struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            iIPDevID[i] = m_struChanInfoV40.wIPID - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfoV40);
                            break;
                        default:
                            break;
                    }
                }
            }
            Marshal.FreeHGlobal(ptrIpParaCfgV40);
        }

        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                //TextBoxInfo.AppendText(str);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        /// <summary>
        /// 启用抓拍CheckBox状态确认
        /// </summary>
        public void CheckBoxStatus()
        {
            var currentPond = (DataContext as DeviceViewModel).CurrentPond;
            CaptureEnableCheckBox.IsChecked = (currentPond.CaptureEnable && currentPond.CameraType == "海康威视网络摄像头");
            WatchEnableCheckBox1.IsChecked = (currentPond.WatchEnableIP1 && currentPond.CameraType == "海康威视网络摄像头");
            WatchEnableCheckBox2.IsChecked = (currentPond.WatchEnableIP2 && currentPond.CameraType == "海康威视网络摄像头");
            WatchEnableCheckBox3.IsChecked = (currentPond.WatchEnableIP3 && currentPond.CameraType == "海康威视网络摄像头");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBoxStatus();
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "Camera1":
                    Login(CurrentPond2.CameraIP1, 0, RealPlayWnd1, ref m_lRealHandle1);
                    break;
                case "Camera2":
                    Login(CurrentPond2.CameraIP2, 1, RealPlayWnd2, ref m_lRealHandle2);
                    break;
                case "Camera3":
                    Login(CurrentPond2.CameraIP3, 2, RealPlayWnd3, ref m_lRealHandle3);
                    break;
            }
        }
    }
}
