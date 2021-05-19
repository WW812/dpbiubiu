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
    public class HikvisionHelper
    {
        private bool m_bInitSDK = false;
        private uint iLastErr = 0;
        //private uint dwAChanTotalNum = 0;
        //private uint dwDChanTotalNum = 0;
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
        //private Int32 i = 0;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
        private int[] iIPDevID = new int[96];

        private CHCNetSDK.NET_DVR_WORKSTATE m_status;

        private void NET_DVR_Init()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                //MessageBox.Show("NET_DVR_Init error!");
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        #region 海康威视网络摄像头

        /// <summary>
        /// 返回 UserID 登录失败返回-1
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="userID"></param>
        /// <returns>返回 UserID 登录失败返回-1</returns>
        public int LoginBySingleCamera(string ip, Int16 port, string username, string password, ref int userID)
        {
            NET_DVR_Init(); //初始化

            /*
            if (userID >= 0)
            {
                Logout(ref m_lRealHandle, playWnd, ref userID);
            }
            */
            string DVRIPAddress = ip;//IPTextBox.Text; //设备IP地址或者域名
            Int16 DVRPortNumber = port;//设备服务端口号
            string DVRUserName = username;//设备登录用户名
            string DVRPassword = password;//设备登录密码

            DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            userID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (userID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                var str = "登录失败, 错误码= " + iLastErr; //登录失败，输出错误号
                //MessageBox.Show(str);
                return -1;
            }
            else
            {
                //登录成功

                /*
                dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
                if (dwDChanTotalNum > 0)
                {
                    //InfoIPChannel(ref userID);
                }
                else
                {
                    for (i = 0; i < dwAChanTotalNum; i++)
                    {
                        iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                    }
                }
            */
                return userID;
            }
        }

        public void PlayBySingleCamera(int userID, System.Windows.Forms.PictureBox playWnd, ref Int32 m_lRealHandle)
        {
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO
                {
                    hPlayWnd = playWnd.Handle,//预览窗口
                    lChannel = 1,//预te览的设备通道
                    dwStreamType = 0,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 1,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = false, //0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = 1 //播放库播放缓冲区最大缓冲帧数
                };

                CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                                                                                                       //IntPtr pUser = new IntPtr();//用户数据
                IntPtr pUser = IntPtr.Zero;

                //打开预览 Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userID, ref lpPreviewInfo, null, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "预览失败, 错误码= " + iLastErr; //预览失败，输出错误号
                    //MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                }
            }
        }

        /*
        public void InfoIPChannel(ref Int32 m_lUserID)
        {
            uint dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40);

            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
            Marshal.StructureToPtr(m_struIpParaCfgV40, ptrIpParaCfgV40, false);

            uint dwReturn = 0;
            int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                var str = "NET_DVR_GET_IPPARACFG_V40 failed, error code= " + iLastErr;
                //获取IP资源配置信息失败，输出错误号 Failed to get configuration of IP channels and output the error code
                DebugInfo(str);
            }
            else
            {
                DebugInfo("NET_DVR_GET_IPPARACFG_V40 succ!");

                m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));


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
        */

        public void Logout(ref Int32 m_lRealHandle, System.Windows.Forms.PictureBox playWnd, ref int UserID)
        {
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            m_lRealHandle = -1;
            playWnd.Refresh();
            CHCNetSDK.NET_DVR_Logout(UserID);
            CHCNetSDK.NET_DVR_Cleanup();
        }
        #endregion

        #region 海康威视硬盘刻录机
        public int Login(string ip, Int16 port, string username, string password, ref int userID)
        {
            NET_DVR_Init(); //初始化
            string DVRIPAddress = ip; //设备IP地址或者域名
            Int16 DVRPortNumber = port; //设备服务端口号
            string DVRUserName = username;//设备登录用户名
            string DVRPassword = password;//设备登录密码

            DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            userID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (userID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                var str = "登录失败，错误码= " + iLastErr; //登录失败，输出错误号
                //MessageBox.Show(str);
                return -1;
            }
            else
            {
                //登录成功
                DebugInfo("NET_DVR_Login_V30 succ!");
                return userID;
            }
        }

        public void Play(int userID, System.Windows.Forms.PictureBox playWnd, int channelNum, ref Int32 m_lRealHandle)
        {
            if (userID < 0)
            {
                //MessageBox.Show("Please login the device firstly");
                return;
            }
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO
                {
                    hPlayWnd = playWnd.Handle,//预览窗口
                    lChannel = channelNum,//预te览的设备通道
                    dwStreamType = 0,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 1,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = false, //0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = 1 //播放库播放缓冲区最大缓冲帧数
                };

                CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = IntPtr.Zero;

                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "摄像头预览失败，错误码= " + iLastErr; //预览失败，输出错误号
                    //MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                }
            }
        }
        #endregion

        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="userID"></param>
        /// <param name="ReadlHandle"></param>
        /// <param name="channel"></param>
        /// <returns>返回文件路径</returns>
        public string CaptureImage(string path, ref Int32 userID, ref Int32 ReadlHandle, int channel = 1)
        {
            //检测temp文件夹,没有则创建
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("抓拍失败,路径权限错误: " + er, image: BiuMessageBoxImage.Error);
                return "";
            }

            if (userID < 0)
            {
                //MessageBox.Show("设备未登录!");
                return "";
            }
            string sBmpPicFileName = "";

            if (ReadlHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO
                {
                    hPlayWnd = new System.Windows.Forms.PictureBox().Handle,//预览窗口
                    lChannel = channel, //预te览的设备通道
                    dwStreamType = 0,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 1,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = true, //0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = 1 //播放库播放缓冲区最大缓冲帧数
                };

                CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = IntPtr.Zero;

                //打开预览 Start live view 
                ReadlHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userID, ref lpPreviewInfo, null/*RealData*/, pUser);


                CHCNetSDK.NET_DVR_GetDVRWorkState(userID, ref m_status);

                if (ReadlHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "摄像头预览失败，错误码= " + iLastErr; //预览失败，输出错误号
                    return "";
                }
            }

            //图片保存路径和文件名 the path and file name to save
            sBmpPicFileName = path + DateTime.Now.ToString("yyyyMMddhhmmss") + Common.GetUUID() + ".jpg";
            Console.WriteLine(sBmpPicFileName);
            //BMP抓图 Capture a BMP picture
            if (!CHCNetSDK.NET_DVR_CapturePicture(ReadlHandle, sBmpPicFileName))
            {
                //失败
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                var str = "图片抓拍失败,错误码= " + iLastErr;
                BiuMessageBoxWindows.BiuShow(str, image: BiuMessageBoxImage.Error);
                return "";
            }
            else
            {
                var nFileName = path + DateTime.Now.ToString("yyyyMMddhhmmss") + Common.GetUUID() + ".jpg";
                //图片压缩
                return CutPicture(nFileName, sBmpPicFileName);
            }
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <returns></returns>
        public string CutPicture(string nFileName, string sBmpPicFileName)
        {
            var t = 0;
            while (!File.Exists(sBmpPicFileName) && t <= 75) { 
                Thread.Sleep(500);
                t++;
            }
            string uploadFileName;
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

                var i = 0;
                while (!File.Exists(uploadFileName) && i <= 75) {
                    Thread.Sleep(200);
                    i++;
                }
            }

            return uploadFileName;
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }

        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                //TextBoxInfo.AppendText(str);
            }
        }
    }

    public class HikvisionConfigModel
    {
        public int UserID = -1;
        public int UserID2 = -1;
        public int UserID3 = -1;
        public Int32 RealHandle = -1;
        public Int32 RealHandle2 = -1;
        public Int32 RealHandle3 = -1;
        public System.Windows.Forms.PictureBox PlayWnd;
        public System.Windows.Forms.PictureBox PlayWnd2;
        public System.Windows.Forms.PictureBox PlayWnd3;
        public int Channel = 1;
        public int Channel2 = 1;
        public int Channel3 = 1;
        public PonderationConfig Pond = null;
    }
}
