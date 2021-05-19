using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.view_model.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.views.setting.device.camera
{
    /// <summary>
    /// Hikvision1Control.xaml 的交互逻辑
    /// </summary>
    public partial class Hikvision1Control : UserControl
    {
        private bool m_bInitSDK = false;
        private Int32 m_lUserID = -1;
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
        private string str1;
        private string str2;
        private Int32 i = 0;
        private Int32 m_lTree = 0;
        private uint dwAChanTotalNum = 0;
        private uint dwDChanTotalNum = 0;
        private int[] iIPDevID = new int[96];
        private int[] iChannelNum = new int[96];
        //private long iSelIndex = 0;
        public bool IsLogin = false;
        public PonderationConfig CurrentPond
        {
            get { return (PonderationConfig)GetValue(CurrentPondProperty); }
            set { SetValue(CurrentPondProperty, value); }
        }

        public static readonly DependencyProperty CurrentPondProperty =
           DependencyProperty.Register("CurrentPond", typeof(PonderationConfig), typeof(UserControl), new PropertyMetadata());

        public Hikvision1Control()
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
            ComboBox1.Items.Clear();
            ComboBox2.Items.Clear();
            ComboBox3.Items.Clear();
            IPTextBox.IsEnabled = true;
            PortTextBox.IsEnabled = true;
            UserNameTextBox.IsEnabled = true;
            PasswordTextBox.IsEnabled = true;
            RefreshBtn.IsEnabled = false;

            //注销登录
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                m_lUserID = -1;
            }

            CHCNetSDK.NET_DVR_Cleanup();
            IsLogin = false;
            LoginBtn.Content = "登录";
        }

        private void NET_DVR_Init()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                BiuMessageBoxWindows.BiuShow("摄像头初始化失败!", image: BiuMessageBoxImage.Error);
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        private void Login()
        {

            NET_DVR_Init(); //初始化

            if (m_lUserID < 0)
            {
                string DVRIPAddress = IPTextBox.Text; //设备IP地址或者域名
                Int16 DVRPortNumber = System.Convert.ToInt16(PortTextBox.Text);//设备服务端口号
                string DVRUserName = UserNameTextBox.Text;//设备登录用户名
                string DVRPassword = PasswordTextBox.Text;//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "登录失败, 错误码= " + iLastErr; //登录失败，输出错误号
                    BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                    return;
                }
                else
                {
                    //登录成功
                    DebugInfo("NET_DVR_Login_V30 succ!");
                    IsLogin = true;
                    LoginBtn.Content = "退出";
                    IPTextBox.IsEnabled = false;
                    PortTextBox.IsEnabled = false;
                    UserNameTextBox.IsEnabled = false;
                    PasswordTextBox.IsEnabled = false;
                    RefreshBtn.IsEnabled = true;
                    ChannelRun1.Text = CurrentPond.CameraChannel1.ToString();
                    ChannelRun2.Text = CurrentPond.CameraChannel2.ToString();
                    ChannelRun3.Text = CurrentPond.CameraChannel3.ToString();
                    // 保存
                    (DataContext as DeviceViewModel).ExecuteSave(false);

                    dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                    dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
                    if (dwDChanTotalNum > 0)
                    {
                        InfoIPChannel();
                    }
                    else
                    {
                        for (i = 0; i < dwAChanTotalNum; i++)
                        {
                            ListAnalogChannel(i + 1, 1);
                            iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                        }

                        // MessageBox.Show("This device has no IP channel!");
                    }

                    ShowVideo(RealPlayWnd1, CurrentPond.CameraChannel1, ref m_lRealHandle1);
                    ShowVideo(RealPlayWnd2, CurrentPond.CameraChannel2, ref m_lRealHandle2);
                    ShowVideo(RealPlayWnd3, CurrentPond.CameraChannel3, ref m_lRealHandle3);
                }
            }
        }

        private void ShowVideo(System.Windows.Forms.PictureBox playWnd, int channelNum, ref Int32 m_lRealHandle)
        {
            if (m_lUserID < 0)
            {
                BiuMessageBoxWindows.BiuShow("设备未登录!",image:BiuMessageBoxImage.Error);
                return;
            }

            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = playWnd.Handle;//预览窗口
                lpPreviewInfo.lChannel = channelNum;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 1;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = false; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数

                CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                //IntPtr pUser = new IntPtr();//用户数据
                IntPtr pUser = IntPtr.Zero;

                //打开预览 Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "预览失败, 错误码= " + iLastErr; //预览失败，输出错误号
                    BiuMessageBoxWindows.BiuShow(str,image:BiuMessageBoxImage.Error);
                    return;
                }
                else
                {
                    //预览成功
                }
            }
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }

        public void InfoIPChannel()
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
                    ListAnalogChannel(i + 1, m_struIpParaCfgV40.byAnalogChanEnable[i]);
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

                            //列出IP通道 List the IP channel
                            ListIPChannel(i + 1, m_struChanInfo.byEnable, m_struChanInfo.byIPID);
                            iIPDevID[i] = m_struChanInfo.byIPID + m_struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            //列出IP通道 List the IP channel
                            ListIPChannel(i + 1, m_struChanInfoV40.byEnable, m_struChanInfoV40.wIPID);
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

        public void ListIPChannel(Int32 iChanNo, byte byOnline, int byIPID)
        {
            str1 = String.Format("IPCamera {0}", iChanNo);
            m_lTree++;

            if (byIPID == 0)
            {
                str2 = "X"; //通道空闲，没有添加前端设备 the channel is idle                  
            }
            else
            {
                if (byOnline == 0)
                {
                    str2 = "离线"; //通道不在线 the channel is off-line
                }
                else
                    str2 = "在线"; //通道在线 The channel is on-line
            }

            ComboBox1.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
            ComboBox2.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
            ComboBox3.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
        }

        public void ListAnalogChannel(Int32 iChanNo, byte byEnable)
        {
            str1 = String.Format("Camera {0}", iChanNo);
            m_lTree++;

            if (byEnable == 0)
            {
                str2 = "Disabled"; //通道已被禁用 This channel has been disabled               
            }
            else
            {
                str2 = "Enabled"; //通道处于启用状态 This channel has been enabled
            }

            ComboBox1.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
            ComboBox2.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
            ComboBox3.Items.Add(str1 + " -- " + str2);//将通道添加到列表中 add the channel to the list
        }

        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                //TextBoxInfo.AppendText(str);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsLogin)
                Logout();
            else
                Login();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //刷新通道列表
            ComboBox1.Items.Clear();
            ComboBox2.Items.Clear();
            ComboBox3.Items.Clear();
            for (i = 0; i < dwAChanTotalNum; i++)
            {
                ListAnalogChannel(i + 1, 1);
                iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
            }
            InfoIPChannel();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var datacontext = DataContext as DeviceViewModel;
            if (cb.Items.Count <= 0) return;
            switch (cb.Name)
            {
                case "ComboBox1":
                    CurrentPond.CameraChannel1 = iChannelNum[cb.SelectedIndex];
                    ChannelRun1.Text = iChannelNum[cb.SelectedIndex].ToString();
                    ChangedChannel(ref RealPlayWnd1, iChannelNum[cb.SelectedIndex], ref m_lRealHandle1);
                    break;
                case "ComboBox2":
                    CurrentPond.CameraChannel2 = iChannelNum[cb.SelectedIndex];
                    ChannelRun2.Text = iChannelNum[cb.SelectedIndex].ToString();
                    ChangedChannel(ref RealPlayWnd2, iChannelNum[cb.SelectedIndex], ref m_lRealHandle2);
                    break;
                case "ComboBox3":
                    CurrentPond.CameraChannel3 = iChannelNum[cb.SelectedIndex];
                    ChannelRun3.Text = iChannelNum[cb.SelectedIndex].ToString();
                    ChangedChannel(ref RealPlayWnd3, iChannelNum[cb.SelectedIndex], ref m_lRealHandle3);
                    break;
            }
            datacontext.ExecuteSave(false);
        }

        private void ChangedChannel(ref System.Windows.Forms.PictureBox realPlayWnd, int Channel, ref Int32 realHandle)
        {
            //停止预览 Stop live view 
            if (realHandle > -1)
            {
                if (!CHCNetSDK.NET_DVR_StopRealPlay(realHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    //DebugInfo(str);
                    //return;
                }
                realHandle = -1;
                realPlayWnd.Refresh();
            }
            ShowVideo(realPlayWnd, Channel, ref realHandle);
        }

        /// <summary>
        /// 启用抓拍CheckBox状态确认
        /// </summary>
        public void CheckBoxStatus()
        {
            var currentPond = (DataContext as DeviceViewModel).CurrentPond;
            CaptureEnableCheckBox.IsChecked = (currentPond.CaptureEnable && currentPond.CameraType == "海康威视硬盘刻录机");
            WatchEnableCheckBox.IsChecked = (currentPond.WatchEnable && currentPond.CameraType == "海康威视硬盘刻录机");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBoxStatus();
        }

    }
}
