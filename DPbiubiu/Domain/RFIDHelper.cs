using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    public class RFIDHelper
    {
        private static RFIDHelper _uniqueInstance;
        private static readonly object locker = new object();
        private bool _rfidRead = false;
        private Process _rfidProc = new Process();
        private readonly string[] _errorStr = { "未找到发卡器，请检查设备连接后重试!", "未知错误,请重试!" };
        private readonly ProcessStartInfo _rfidStartInfo = new ProcessStartInfo
        {
            FileName = "./RFID/RFID_LJYZN/RFIDReader_LJYZN.exe",
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        private RFIDHelper()
        {

        }

        public static RFIDHelper GetInstance()
        {
            if (_uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (_uniqueInstance == null)
                    {
                        _uniqueInstance = new RFIDHelper();
                    }
                }
            }
            return _uniqueInstance;
        }

        public void Run_RFID_LJYZN()
        {
            try
            {
                if (_rfidRead) return;
                var proc = Common.GetProcByName("RFIDReader_LJYZN");
                proc?.Kill();
                _rfidProc.StartInfo = _rfidStartInfo;
                _rfidProc.Start();
                Thread.Sleep(100);
                _rfidRead = true;
                Task.Run(() =>
                {
                    while (_rfidRead)
                    {
                        EventCenter.Broadcast(EventType.LJYZN_RFID,DecodeRFIDCode(_rfidProc.StandardOutput.ReadLine()));
                        Thread.Sleep(200);
                    }
                });
            }
            catch (Exception er)
            {
                EventCenter.Broadcast(EventType.LJYZN_RFID, new LJYZN_RFIDEventInfomation { Code = -2, Error = "报错, 原因: " + er.Message + "\n检查读卡器数据线是否松动，可尝试重启软件。" }) ;
                _rfidProc.Close();
            }
        }

        /// <summary>
        /// 解析RFID返回码
        /// </summary>
        private LJYZN_RFIDEventInfomation DecodeRFIDCode(string code)
        {
            LJYZN_RFIDEventInfomation rfidInfo = new LJYZN_RFIDEventInfomation();
            // 代表通讯正常
            if ("0".Equals(code))
            {
                rfidInfo.Code = 0;
            }
            // 得到卡内容
            else if (code.Contains("$$=="))
            {
                rfidInfo.Data = code.Replace("$$==", "");
                rfidInfo.Code = 1;
                //BtnRFIDShow = Visibility.Hidden;
            }
            // 未找到发卡器
            else if ("-1".Equals(code))
            {
                rfidInfo.Error = _errorStr[0];
                rfidInfo.Code = -1;
                CloseRFID();
            }
            // 未知异常
            else if (code.Contains("Error:"))
            {
                rfidInfo.Error = code;
                rfidInfo.Code = -2;
                CloseRFID();
            }
            else
            {
                rfidInfo.Data = "";
                rfidInfo.Code = 2;
            }
            return rfidInfo;
        }
        
        public void RetryRFID()
        {
            try
            {
                CloseRFID();
                Thread.Sleep(200);
                Run_RFID_LJYZN();
            }catch(Exception er)
            {
                EventCenter.Broadcast(EventType.LJYZN_RFID, new LJYZN_RFIDEventInfomation { Code = -2, Error = "报错, 原因: " + er.Message + "\n检查读卡器数据线是否松动，可尝试重启软件。" }) ;
                CloseRFID();
            }
        }

        public void CloseRFID()
        {
            _rfidRead = false;
            _rfidProc.Close();
        }
    }
}
