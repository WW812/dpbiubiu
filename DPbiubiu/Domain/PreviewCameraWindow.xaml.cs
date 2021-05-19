using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace biubiu.Domain
{
    /// <summary>
    /// PreviewCameraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewCameraWindow : Window
    {
        #region 老代码
        public bool IsClose = false;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
        public CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode;
        public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
        public bool IsLogin = false;

        public List<HikvisionConfigModel> hikConfig = new List<HikvisionConfigModel> {
            new HikvisionConfigModel(),
            new HikvisionConfigModel(),
            new HikvisionConfigModel(),
            new HikvisionConfigModel()
        };


        public PreviewCameraWindow()
        {
            InitializeComponent();
        }

        private void CameraInit(PonderationConfig config, int index)
        {
            switch (config.CameraType)
            {
                case "海康威视硬盘刻录机":
                    if (!config.WatchEnable) return;
                    var hikHelper = new HikvisionHelper();
                    if (-1 != hikHelper.Login(config.CameraIP, (Int16)config.CameraPort, config.CameraUserName, config.CameraPassWord, ref hikConfig[index].UserID))
                    {
                        hikHelper.Play(hikConfig[index].UserID, hikConfig[index].PlayWnd, config.CameraChannel1, ref hikConfig[index].RealHandle);
                        hikHelper.Play(hikConfig[index].UserID, hikConfig[index].PlayWnd2, config.CameraChannel2, ref hikConfig[index].RealHandle2);
                        hikHelper.Play(hikConfig[index].UserID, hikConfig[index].PlayWnd3, config.CameraChannel3, ref hikConfig[index].RealHandle3);
                    }
                    break;
                case "海康威视网络摄像头":
                    var hikHelper2 = new HikvisionHelper();
                    if (config.WatchEnableIP1 && -1 != hikHelper2.LoginBySingleCamera(config.CameraIP1, (Int16)config.CameraPort, config.CameraUserName, config.CameraPassWord, ref hikConfig[index].UserID))
                        hikHelper2.PlayBySingleCamera(hikConfig[index].UserID, hikConfig[index].PlayWnd, ref hikConfig[index].RealHandle);
                    if (config.WatchEnableIP2 && -1 != hikHelper2.LoginBySingleCamera(config.CameraIP2, (Int16)config.CameraPort, config.CameraUserName, config.CameraPassWord, ref hikConfig[index].UserID2))
                        hikHelper2.PlayBySingleCamera(hikConfig[index].UserID2, hikConfig[index].PlayWnd2, ref hikConfig[index].RealHandle2);
                    if (config.WatchEnableIP3 && -1 != hikHelper2.LoginBySingleCamera(config.CameraIP3, (Int16)config.CameraPort, config.CameraUserName, config.CameraPassWord, ref hikConfig[index].UserID3))
                        hikHelper2.PlayBySingleCamera(hikConfig[index].UserID3, hikConfig[index].PlayWnd3, ref hikConfig[index].RealHandle3);
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IsClose = true;
            Logout();
        }

        public void Logout()
        {
            hikConfig.ForEach(hc =>
            {
                CHCNetSDK.NET_DVR_StopRealPlay(hc.RealHandle);
                hc.RealHandle = -1;
                CHCNetSDK.NET_DVR_StopRealPlay(hc.RealHandle2);
                hc.RealHandle2 = -1;
                CHCNetSDK.NET_DVR_StopRealPlay(hc.RealHandle3);
                hc.RealHandle2 = -1;
                if (hc.UserID >= 0)
                {
                    CHCNetSDK.NET_DVR_Logout(hc.UserID);
                    CHCNetSDK.NET_DVR_Cleanup();
                    hc.UserID = -1;
                }
            });
            RealPlayWnd1_1.Refresh();
            RealPlayWnd1_2.Refresh();
            RealPlayWnd1_3.Refresh();
            RealPlayWnd2_1.Refresh();
            RealPlayWnd2_2.Refresh();
            RealPlayWnd2_3.Refresh();
            RealPlayWnd3_1.Refresh();
            RealPlayWnd3_2.Refresh();
            RealPlayWnd3_3.Refresh();
            RealPlayWnd4_1.Refresh();
            RealPlayWnd4_2.Refresh();
            RealPlayWnd4_3.Refresh();
        }

        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                //TextBoxInfo.AppendText(str);
            }
        }


        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 播放控件赋值 
            hikConfig[0].PlayWnd = RealPlayWnd1_1;
            hikConfig[0].PlayWnd2 = RealPlayWnd1_2;
            hikConfig[0].PlayWnd3 = RealPlayWnd1_3;
            hikConfig[1].PlayWnd = RealPlayWnd2_1;
            hikConfig[1].PlayWnd2 = RealPlayWnd2_2;
            hikConfig[1].PlayWnd3 = RealPlayWnd2_3;
            hikConfig[2].PlayWnd = RealPlayWnd3_1;
            hikConfig[2].PlayWnd2 = RealPlayWnd3_2;
            hikConfig[2].PlayWnd3 = RealPlayWnd3_3;
            hikConfig[3].PlayWnd = RealPlayWnd4_1;
            hikConfig[3].PlayWnd2 = RealPlayWnd4_2;
            hikConfig[3].PlayWnd3 = RealPlayWnd4_3;

            CameraInit(Config.P1, 0);
            CameraInit(Config.P2, 1);
            CameraInit(Config.P3, 2);
            CameraInit(Config.P4, 3);
        }
    }

}
