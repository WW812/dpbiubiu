using ADSDK.Bases;
using ADSDK.Device;
using ADSDK.Device.Reader.Adpmm;
using ADSDK.Initializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFIDReader
{
    /// <summary>
    /// RFID读取
    /// </summary>
    public class Program
    {
        public static void RxRspEventReceived(object sender, ProtocolEventArg e)
        {
            ParseRsp(e.Protocol);
        }

        public static void Main(string[] args)
        {
            SystemPub.ADRcp = new AdpmmRcp();
            IniSettings.Communication = CommType.USB;
            SystemPub.ADRcp.RxRspParsed += RxRspEventReceived;
            try
            {
                if (LoadUsbDevice())
                {
                    while (SystemPub.ADRcp.bConnected)
                    {
                        AdpmmCommand.Identify6C(SystemPub.ADRcp);
                        Thread.Sleep(600);
                    }
                }
            }
            catch(Exception er)
            {
                Console.Write("Error: " + er.Message);
                return;
            }
        }

        /// <summary>
        /// 加载连接设备
        /// </summary>
        private static bool LoadUsbDevice()
        {
            LoadCommunication();
            List<DictionaryEntry> deviceList = new List<DictionaryEntry>();
            if (!ADUsb.GetHidDeviceList(ref deviceList))
            {// 未找到发卡器，返回-1;
                Console.Write("-1");
                return false;
            }
            else
            {
                foreach (var item in deviceList)
                {
                    //string usbkey = deviceList[0].Key.ToString();
                    string usbkey = item.Key.ToString();
                    // 开始连接 
                    if (usbkey.Contains("HID_110"))
                        IniSettings.USBType = 1;
                    else if (usbkey.Contains("HID_300"))
                        IniSettings.USBType = 2;
                    else
                        IniSettings.USBType = 0;

                    if (!SystemPub.ADRcp.bConnected)
                    {
                        IniSettings.USBDevPath = item.Value.ToString();
                        SystemPub.ADRcp.Connect(IniSettings.HostName, IniSettings.HostPort, (int)IniSettings.Communication);
                        if (SystemPub.ADRcp.bConnected)
                        {
                            break;
                        }
                        else
                        {
                        }
                    }
                }

                if(!SystemPub.ADRcp.bConnected)
                {
                    throw new Exception("连接异常,请检查发卡器连接!");
                }
                else
                {
                    return true;
                }
            }
        }


        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="Data"></param>
        public static void ParseRsp(ProtocolPacket Data)
        {
            switch (Data.Code)
            {
                case AdpmmRcp.RCP_CMD_READ_C_UII:
                    if (Data.Type == 2 || Data.Type == 5)
                    {
                        int pcepclen = GetCodelen(Data.Payload[1]);
                        int datalen = Data.Length - 2;//去掉天线号去掉rssi
                        string epc = "";
                        if ((datalen - pcepclen) > 0)
                            epc = ConvertData.ByteArrayToHexString(Data.Payload, 3, datalen - 2);
                        else
                            epc = ConvertData.ByteArrayToHexString(Data.Payload, 3, pcepclen - 2);

                        Console.WriteLine("$$=="+epc);

                    }
                    else if (Data.Type == 0 || Data.Type == 1)
                    {
                        //throw new Exception("Data.Type = " + Data.Type);
                    }
                    else
                    {
                        //throw new Exception("Data.Type = " + Data.Type);
                    }
                    break;
            }
        }

        /// <summary>
        /// 加载通讯默认值
        /// </summary>
        private static void LoadCommunication()
        {
            IniSettings.LoadCommunication();

            foreach (string st in SerialPort.GetPortNames())
            {
                for (int i = st.Length - 1; i > 3; i--)
                {
                    if (RegexBase.IsUint(st.Substring(i, 1)))
                    {
                        break;
                    }
                }
            }
        }


        private static int GetCodelen(byte iData)
        {
            return (((iData >> 3) + 1) * 2);
        }
    }
}
