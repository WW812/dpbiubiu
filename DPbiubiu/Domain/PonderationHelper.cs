using biubiu.Domain.biuMessageBox;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using wow;

namespace biubiu.Domain
{
    public class PonderationHelper
    {
        // 定义一个静态变量来保存类的实例
        private static PonderationHelper _uniqueInstance;
        private static readonly object locker = new object();
        public bool Running = false;

        /// <summary>
        /// 是否展示版本
        /// 调试用，随机数地磅
        /// </summary>
        private readonly bool _isDisplay = true;

        private PonderationHelper()
        {
        }

        /// <summary>
        /// 重新运行地磅
        /// </summary>
        public void ReRunPond()
        {
            Running = false;
            Thread.Sleep(400);
            Running = true;
            RunPond();
        }

        public void StopPond()
        {
            Running = false;
            Thread.Sleep(400);
        }

        public void RunPond()
        {
            Running = true;
            // 1磅
            Task.Run(() =>
            {
                if (Config.P1.Enable)
                {
                    if (_isDisplay)
                    {
                        while (Running)
                        {
                            var r = new Random();
                            EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation
                            {
                                Name = Config.P1.Name,
                                Weight = (r.Next(2000, 8000) / 100.0).ToString(),
                                Error = ""
                            });
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    else
                    {
                        RunPortByPond(Config.P1, Config.PondDataP1);
                    }
                }
            });

            // 2磅
            Task.Run(() =>
            {
                if (Config.P2.Enable)
                {
                    if (_isDisplay)
                    {
                        while (Running)
                        {
                            var r = new Random();
                            EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation
                            {
                                Name = Config.P2.Name,
                                Weight = (r.Next(800, 10000) / 100.0).ToString(),
                                Error = ""
                            });
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    else
                    {
                        RunPortByPond(Config.P2, Config.PondDataP2);
                    }
                }
            });

            // 3磅
            Task.Run(() =>
            {
                if (Config.P3.Enable)
                {
                    if (_isDisplay)
                    {
                        while (Running)
                        {
                            var r = new Random();
                            EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation
                            {
                                Name = Config.P3.Name,
                                Weight = (r.Next(1000, 7000) / 100.0).ToString(),
                                Error = ""
                            });
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    else
                    {
                        RunPortByPond(Config.P3, Config.PondDataP3);
                    }
                }
            });

            // 4磅
            Task.Run(() =>
            {
                if (Config.P4.Enable)
                {
                    if (_isDisplay)
                    {
                        while (Running)
                        {
                            var r = new Random();
                            EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation
                            {
                                Name = Config.P4.Name,
                                Weight = (r.Next(200, 9000) / 100.0).ToString(),
                                Error = ""
                            });
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    else
                    {
                        RunPortByPond(Config.P4, Config.PondDataP4);
                    }
                }
            });
        }

        private void RunPortByPond(PonderationConfig pConfig, PondDataParameter pData)
        {
            try
            {
                SerialPort sPort = pConfig.GetSerialPort();
                PonderationCommon pondCommon = new PonderationCommon(sPort, pConfig.PondTypeName, pData);
                if (sPort.IsOpen) return;
                sPort.Open();
                while (pConfig.Enable && Running)
                {
                    string data = pondCommon.Run();
                    if (data != "")
                    {
                        var pei = Config.AdjustWeightEnabled ? new PonderEventInfomation { Name = pConfig.Name, Weight = (double.Parse(data) + pConfig.AdjustWeight).ToString(), Error = "" } : new PonderEventInfomation { Name = pConfig.Name, Weight = double.Parse(data).ToString(), Error = "" };
                        EventCenter.Broadcast(EventType.Ponder, pei);
                    }
                }
                sPort.Close();
                sPort.Dispose();
                EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation { Name = pConfig.Name, Reset = true }); ;
            }
            catch (Exception er)
            {
                EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation { Name = pConfig.Name, Weight = "0", Error = "报错, 原因: " + er.Message + "\n检查仪表数据线是否松动，可尝试重启软件和地磅仪表。" });
            }
        }

        public static PonderationHelper GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (_uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (_uniqueInstance == null)
                    {
                        _uniqueInstance = new PonderationHelper();
                    }
                }
            }
            return _uniqueInstance;
        }
    }
}
