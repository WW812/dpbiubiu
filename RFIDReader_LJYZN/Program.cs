using ReaderB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFIDReader_LJYZN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = 0;
                byte fComAdr = 255;
                int frmcomportindex = 0;
                //int fCmdRet = 30; //所有执行指令的返回值
                int openresult = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, 5, ref frmcomportindex);
                if (openresult == 0)
                {
                    /*
                    if ((fCmdRet == 0x35) | (fCmdRet == 0x30))
                    {
                        StaticClassReaderB.CloseSpecComPort(frmcomportindex);
                        if(fCmdRet == 0x30) Console.WriteLine("-1");
                    }
                    else
                    {
                    */
                        while (true)
                        {
                            Thread.Sleep(600);
                            Inventory(frmcomportindex);
                        }
                    //}
                }
                else
                {
                    Console.WriteLine("-1");
                }
            }catch(Exception er)
            {
                Console.WriteLine("Error:" + er.Message);
                return;
            }
        }

        static private void Inventory(int frmcomportindex)
        {
            int CardNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[5000];
            int CardIndex;
            string temps;
            string s, sEPC;
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;
            byte fComAdr = 255;
            var fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//代表已查找结束，
            {
                byte[] daw = new byte[Totallen];
                Array.Copy(EPC, daw, Totallen);
                temps = ByteArrayToHexString(daw);
                m = 0;

                if (CardNum == 0)
                {
                    Console.WriteLine(0);
                    return;
                }
                for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                    EPClen = daw[m];
                    sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                    m = m + EPClen + 1;
                    if (sEPC.Length != EPClen * 2)
                    {
                        Console.WriteLine(0);
                        return;
                    }
                    s = sEPC;
                    s = (sEPC.Length / 2).ToString().PadLeft(2, '0');
                    Console.WriteLine($"$$=={sEPC}");
                }
                Console.WriteLine(0);
            }
            else if(fCmdRet == 0x30)
            {
                Console.WriteLine("-1");
            }
            else
            {
                Console.WriteLine(ErrorStr(GetReturnCodeDesc(fCmdRet)));
            }
        }

        static private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        private static string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "操作成功";
                case 0x01:
                    return "询查时间结束前返回";
                case 0x02:
                    return "指定的询查时间溢出";
                case 0x03:
                    return "本条消息之后，还有消息";
                case 0x04:
                    return "读写模块存储空间已满";
                case 0x05:
                    return "访问密码错误";
                case 0x09:
                    return "销毁密码错误";
                case 0x0a:
                    return "销毁密码不能为全0";
                case 0x0b:
                    return "电子标签不支持该命令";
                case 0x0c:
                    return "对该命令，访问密码不能为全0";
                case 0x0d:
                    return "电子标签已经被设置了读保护，不能再次设置";
                case 0x0e:
                    return "电子标签没有被设置读保护，不需要解锁";
                case 0x10:
                    return "有字节空间被锁定，写入失败";
                case 0x11:
                    return "不能锁定";
                case 0x12:
                    return "已经锁定，不能再次锁定";
                case 0x13:
                    return "参数保存失败,但设置的值在读写模块断电前有效";
                case 0x14:
                    return "无法调整";
                case 0x15:
                    return "询查时间结束前返回";
                case 0x16:
                    return "指定的询查时间溢出";
                case 0x17:
                    return "本条消息之后，还有消息";
                case 0x18:
                    return "读写模块存储空间已满";
                case 0x19:
                    return "电子不支持该命令或者访问密码不能为0";
                case 0xFA:
                    return "有电子标签，但通信不畅，无法操作";
                case 0xFB:
                    return "无电子标签可操作";
                case 0xFC:
                    return "电子标签返回错误代码";
                case 0xFD:
                    return "命令长度错误";
                case 0xFE:
                    return "不合法的命令";
                case 0xFF:
                    return "参数错误";
                case 0x30:
                    return "通讯错误";
                case 0x31:
                    return "CRC校验错误";
                case 0x32:
                    return "返回数据长度有错误";
                case 0x33:
                    return "通讯繁忙，设备正在执行其他指令";
                case 0x34:
                    return "繁忙，指令正在执行";
                case 0x35:
                    return "端口已打开";
                case 0x36:
                    return "端口已关闭";
                case 0x37:
                    return "无效句柄";
                case 0x38:
                    return "无效端口";
                case 0xEE:
                    return "返回指令错误";
                default:
                    return "";
            }
        }
        /*
        private string GetErrorCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "其它错误";
                case 0x03:
                    return "存储器超限或不被支持的PC值";
                case 0x04:
                    return "存储器锁定";
                case 0x0b:
                    return "电源不足";
                case 0x0f:
                    return "非特定错误";
                default:
                    return "";
            }
        }
        */

        private static string ErrorStr(string err)
        {
            return $"Error:{err}";
        }
    }
}
