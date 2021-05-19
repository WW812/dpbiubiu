using biubiu.Domain.biuMessageBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.Domain
{
    /// <summary>
    /// 抓拍
    /// </summary>
    public class Snapshot
    {
        private bool m_bInitSDK = false;
        private uint iLastErr = 0;
        private string str;
        //private CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
        public CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode;
        public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
        public CHCNetSDK.NET_DVR_JPEGPARA m_jpegpara = new CHCNetSDK.NET_DVR_JPEGPARA
        {
            wPicQuality = 2,
            wPicSize = 1
        };
        CHCNetSDK.NET_DVR_WORKSTATE m_status;
        private string str1;
        private string str2;
        private Int32 i = 0;
        private uint dwAChanTotalNum = 0;
        private uint dwDChanTotalNum = 0;
        private int[] iIPDevID = new int[96];
        private HikvisionHelper hikvisionConfig = new HikvisionHelper();
        private int _loginHikNums = 0; // 当前登录的摄像头数量

        private Dictionary<String, HikvisionConfigModel> PonderationPool = new Dictionary<string, HikvisionConfigModel>();

        public Snapshot()
        {
            NET_DVR_Init(); //初始化
        }

        private void NET_DVR_Init()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                BiuMessageBoxWindows.BiuShow("摄像头初始化失败，抓拍功能无法使用!", image: BiuMessageBoxImage.Error);
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        public List<String> GetSnapshotByPonderation(PonderationConfig pond)
        {
            switch (pond.CameraType)
            {
                case "海康威视硬盘刻录机":
                    return Snapshot_Hikvision(pond);
                case "海康威视网络摄像头":
                    return Snapshot_Hikvision_Single(pond);
                default:
                    return new List<string> { "", "", "" };
            }
        }

        /// <summary>
        /// 海康威视硬盘刻录机抓拍
        /// </summary>
        /// <param name="pond"></param>
        /// <returns></returns>
        private List<String> Snapshot_Hikvision(PonderationConfig pond)
        {
            NET_DVR_Init();
            List<String> FilesName = new List<string> { "", "", "" };
            if (pond == null)
            {
                BiuMessageBoxWindows.BiuShow("抓拍参数错误，抓拍失败!", image: BiuMessageBoxImage.Error);
                return FilesName;
            }

            if (PonderationPool.ContainsKey(pond.Name + pond.CameraType))
            {
                //直接截屏
                FilesName[0] = ShowVideo(PonderationPool[pond.Name + pond.CameraType].UserID, PonderationPool[pond.Name + pond.CameraType].PlayWnd, PonderationPool[pond.Name + pond.CameraType].Pond.CameraChannel1, ref PonderationPool[pond.Name + pond.CameraType].RealHandle);
                FilesName[1] = ShowVideo(PonderationPool[pond.Name + pond.CameraType].UserID, PonderationPool[pond.Name + pond.CameraType].PlayWnd2, PonderationPool[pond.Name + pond.CameraType].Pond.CameraChannel2, ref PonderationPool[pond.Name + pond.CameraType].RealHandle2);
                FilesName[2] = ShowVideo(PonderationPool[pond.Name + pond.CameraType].UserID, PonderationPool[pond.Name + pond.CameraType].PlayWnd3, PonderationPool[pond.Name + pond.CameraType].Pond.CameraChannel3, ref PonderationPool[pond.Name + pond.CameraType].RealHandle3);
            }
            else
            {
                CheckCameraNums();
                //登陆截屏
                var cameraData = new HikvisionConfigModel
                {
                    Pond = pond,
                    PlayWnd = new System.Windows.Forms.PictureBox(),
                    PlayWnd2 = new System.Windows.Forms.PictureBox(),
                    PlayWnd3 = new System.Windows.Forms.PictureBox()
                };
                if (cameraData.UserID < 0)
                {
                    string DVRIPAddress = pond.CameraIP; //设备IP地址或者域名
                    Int16 DVRPortNumber = (Int16)pond.CameraPort;//设备服务端口号
                    string DVRUserName = pond.CameraUserName;//设备登录用户名
                    string DVRPassword = pond.CameraPassWord;//设备登录密码

                    CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                    //登录设备 Login the device
                    cameraData.UserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                    if (cameraData.UserID < 0)
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        str = "登录失败，错误码=" + iLastErr; //登录失败，输出错误号
                        BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                    }
                    else
                    {
                        //登录成功 ///////////////////可以先进行校验再预览////////////////
                        DebugInfo("NET_DVR_Login_V30 succ!");

                        PonderationPool.Add(pond.Name + pond.CameraType, cameraData);
                        FilesName[0] = ShowVideo(cameraData.UserID, cameraData.PlayWnd, cameraData.Pond.CameraChannel1, ref cameraData.RealHandle);
                        FilesName[1] = ShowVideo(cameraData.UserID, cameraData.PlayWnd2, cameraData.Pond.CameraChannel2, ref cameraData.RealHandle2);
                        FilesName[2] = ShowVideo(cameraData.UserID, cameraData.PlayWnd3, cameraData.Pond.CameraChannel3, ref cameraData.RealHandle3);
                        _loginHikNums += 3;
                    }
                }
            }


            return FilesName;
        }

        /// <summary>
        /// 海康威视网络摄像头
        /// </summary>
        /// <param name="pond"></param>
        /// <returns></returns>
        private List<String> Snapshot_Hikvision_Single(PonderationConfig pond)
        {
            NET_DVR_Init();
            List<String> FilesName = new List<string> {"", "", "" };
            if (pond == null)
            {
                BiuMessageBoxWindows.BiuShow("抓拍参数错误，抓拍失败!", image: BiuMessageBoxImage.Error);
                return FilesName;
            }
            if (PonderationPool.ContainsKey(pond.Name + pond.CameraType))
            {
                //var hikvisionConfig = new HikvisionHelper();
                FilesName[0] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref PonderationPool[pond.Name + pond.CameraType].UserID, ref PonderationPool[pond.Name + pond.CameraType].RealHandle);
                FilesName[1] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref PonderationPool[pond.Name + pond.CameraType].UserID2, ref PonderationPool[pond.Name + pond.CameraType].RealHandle2);
                FilesName[2] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref PonderationPool[pond.Name + pond.CameraType].UserID3, ref PonderationPool[pond.Name + pond.CameraType].RealHandle3);
            }
            else
            {
                CheckCameraNums();
                //登陆截屏
                var cameraData = new HikvisionConfigModel
                {
                    Pond = pond,
                    PlayWnd = new System.Windows.Forms.PictureBox(),
                    PlayWnd2 = new System.Windows.Forms.PictureBox(),
                    PlayWnd3 = new System.Windows.Forms.PictureBox()
                };

                //var hikvisionConfig = new HikvisionHelper();
                PonderationPool.Add(pond.Name + pond.CameraType, cameraData);

                if (hikvisionConfig.LoginBySingleCamera(pond.CameraIP1, (Int16)pond.CameraPort, pond.CameraUserName, pond.CameraPassWord, ref cameraData.UserID) > -1)
                {
                    FilesName[0] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref cameraData.UserID, ref cameraData.RealHandle);
                    _loginHikNums++;
                }
                if (hikvisionConfig.LoginBySingleCamera(pond.CameraIP2, (Int16)pond.CameraPort, pond.CameraUserName, pond.CameraPassWord, ref cameraData.UserID2) > -1)
                {
                    FilesName[1] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref cameraData.UserID2, ref cameraData.RealHandle2);
                    _loginHikNums++;
                }
                if (hikvisionConfig.LoginBySingleCamera(pond.CameraIP3, (Int16)pond.CameraPort, pond.CameraUserName, pond.CameraPassWord, ref cameraData.UserID3) > -1)
                {
                    FilesName[2] = hikvisionConfig.CaptureImage(Config.TEMP_PATH, ref cameraData.UserID3, ref cameraData.RealHandle3);
                    _loginHikNums++;
                }
            }


            return FilesName;
        }

        /// <summary>
        /// 检查摄像头数量，摄像头数量限制在两个地磅
        /// </summary>
        public void CheckCameraNums()
        {
            if (_loginHikNums > 3)
            {
                foreach (var item in PonderationPool)
                {
                    Logout(item.Value);
                    PonderationPool.Remove(item.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 退出摄像头登录，并清空资源
        /// </summary>
        private void Logout(HikvisionConfigModel hcm)
        {
            hikvisionConfig.Logout(ref hcm.RealHandle, hcm.PlayWnd, ref hcm.UserID);
            hikvisionConfig.Logout(ref hcm.RealHandle2, hcm.PlayWnd2, ref hcm.UserID2);
            hikvisionConfig.Logout(ref hcm.RealHandle3, hcm.PlayWnd3, ref hcm.UserID3);
            if (_loginHikNums < 3) _loginHikNums = 0; else _loginHikNums -= 3;


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
                    ListAnalogChannel(i + 1, m_struIpParaCfgV40.byAnalogChanEnable[i]);
                    //iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                }

                byte byStreamType = 0;
                uint iDChanNum = 64;

                if (dwDChanTotalNum < 64)
                {
                    iDChanNum = dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                }


                for (i = 0; i < iDChanNum; i++)
                {
                    //iChannelNum[i + dwAChanTotalNum] = i + (int)m_struIpParaCfgV40.dwStartDChan;
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
                            IsOnline(m_struChanInfo.byEnable, m_struChanInfo.byIPID);
                            iIPDevID[i] = m_struChanInfo.byIPID + m_struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            //列出IP通道 List the IP channel
                            IsOnline(m_struChanInfoV40.byEnable, m_struChanInfoV40.wIPID);
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

        public void ListAnalogChannel(Int32 iChanNo, byte byEnable)
        {
            str1 = String.Format("Camera {0}", iChanNo);

            if (byEnable == 0)
            {
                str2 = "Disabled"; //通道已被禁用 This channel has been disabled               
            }
            else
            {
                str2 = "Enabled"; //通道处于启用状态 This channel has been enabled
            }

        }

        public bool IsOnline(byte byOnline, int byIPID)
        {

            if (byIPID == 0)
            {
                return false; //通道空闲，没有添加前端设备 the channel is idle                  
            }
            else
            {
                if (byOnline == 0)
                {
                    return false; //通道不在线 the channel is off-line
                }
                else
                    return true; //通道在线 The channel is on-line
            }

        }

        private String ShowVideo(Int32 m_lUserID, System.Windows.Forms.PictureBox playWnd, int channelNum, ref Int32 m_lRealHandle)
        {
            //检测temp文件夹,没有则创建
            try
            {
                if (!Directory.Exists(Config.TEMP_PATH))
                    Directory.CreateDirectory(Config.TEMP_PATH);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("抓拍失败,路径权限错误: " + er, image: BiuMessageBoxImage.Error);
                return "";
            }

            if (m_lUserID < 0)
            {
                BiuMessageBoxWindows.BiuShow("设备未登录!", image: BiuMessageBoxImage.Warning);
                return "";
            }

            //InfoIPChannel(m_lUserID);

            string sBmpPicFileName = "";
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO
                {
                    //hPlayWnd = playWnd.Handle,//预览窗口
                    hPlayWnd = new System.Windows.Forms.PictureBox().Handle,//预览窗口
                    lChannel = channelNum,//预te览的设备通道
                    dwStreamType = 0,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 1,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = true, //0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = 1 //播放库播放缓冲区最大缓冲帧数
                };

                CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                //IntPtr pUser = new IntPtr();//用户数据
                IntPtr pUser = IntPtr.Zero;

                //打开预览 Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);


                CHCNetSDK.NET_DVR_GetDVRWorkState(m_lUserID, ref m_status);

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "预览失败, 错误码= " + iLastErr; //预览失败，输出错误号
                    //MessageBox.Show(str);
                    return "";
                }
            }

            //图片保存路径和文件名 the path and file name to save
            sBmpPicFileName = "./temp/" + DateTime.Now.ToString("yyyyMMddhhmmss") + channelNum + ".jpg";

            //BMP抓图 Capture a BMP picture
            if (!CHCNetSDK.NET_DVR_CapturePicture(m_lRealHandle, sBmpPicFileName))
            {
                //失败
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "抓拍失败, 错误码= " + iLastErr;
                BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                return "";
            }
            else
            {
                //图片压缩
                var nFileName = "./temp/" + DateTime.Now.ToString("yyyyMMddhhss") + sBmpPicFileName.GetHashCode() + ".jpg";
                /*
                var uploadFileName = nFileName;
                if (!File.Exists(sBmpPicFileName))
                    Thread.Sleep(500);
                if (!Common.GetPicThumbnail(sBmpPicFileName, nFileName, 1080, 1920, 80))
                {
                    //压缩失败
                    uploadFileName = sBmpPicFileName;
                }
                else
                {
                    //压缩成功
                    if (File.Exists(sBmpPicFileName))
                    {
                        File.Delete(sBmpPicFileName);
                    }
                    uploadFileName = nFileName;
                }

                return uploadFileName;
                */
                return hikvisionConfig.CutPicture(nFileName, sBmpPicFileName);
            }
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
    }

}
