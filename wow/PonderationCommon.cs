using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wow
{
    /// <summary>
    /// 委托声明
    /// </summary>
    /// <returns></returns>
    public delegate string RunPort();

    /// <summary>
    /// 不同型号地磅仪表参数类
    /// </summary>
    public class PondDataParameter
    {
        public int StartTag { get; set; } //开始位
        public int TimeOut { get; set; }
        public int ReceiveNumber { get; set; }
        public int Fushuwei { get; set; }
        public int Dujiwei { get; set; }
        public int Kaishidu { get; set; }
        public int Zhengfanxu { get; set; }
        public int Fu { get; set; }
    }

    public class PonderationCommon
    {
        /// <summary>
        /// 地磅仪表参数类
        /// </summary>
        private readonly PondDataParameter pondDataParameter;
        /// <summary>
        /// 仪表型号名
        /// </summary>
        private readonly string PondTypeName;
        /// <summary>
        /// SerialPort对象
        /// </summary>
        private readonly SerialPort SerialPortObj;
        /// <summary>
        /// 委托对象
        /// </summary>
        private readonly RunPort runPort;
        /// <summary>
        /// 回调委托
        /// </summary>
        /// <returns></returns>
        public delegate string GetData(string data);
        /// <summary>
        /// 数据
        /// </summary>
        private string FinalData = "";


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serialPort">SerialPort对象</param>
        /// <param name="pondTypeName">仪表型号名</param>
        public PonderationCommon(SerialPort serialPort, string pondTypeName, PondDataParameter dataParameter)
        {
            SerialPortObj = serialPort;
            PondTypeName = pondTypeName;
            pondDataParameter = dataParameter;

            //根据名字设置解析方法
            switch (PondTypeName)
            {
                case "聊城华科XK3199系列":
                    runPort = new RunPort(XK3199);
                    break;
                case "耀华XK3190A12E A27E系列":
                    runPort = new RunPort(XK3190A12E_A27E);
                    break;
                case "杭州科利系列XK3110-E XK3138":
                    runPort = new RunPort(XK3110E_XK3138);
                    break;
                case "上海秋毫QDI系列1":
                    runPort = new RunPort(QDI1);
                    break;
                case "上海秋毫QDI系列2":
                    runPort = new RunPort(QDI2);
                    break;
                case "XK3101系列":
                    runPort = new RunPort(XK3101);
                    break;
                case "杭州顶松系列DS822":
                    runPort = new RunPort(DS822);
                    break;
                case "志美PT650F":
                    runPort = new RunPort(PT650F);
                    break;
                case "上海唐衡T9901":
                case "上海三积分系列":
                case "福州科杰XK3198 ORMT-J2000系列":
                    runPort = new RunPort(T9901_XK3198);
                    break;
                case "上海友声XK3100B2 ACS天平 TCS台秤全系列":
                case "上海友声XK3100-D1 D2+ D2+P":
                    runPort = new RunPort(XK3100B2_XK3100_D1);
                    break;
                case "宁波柯力D2008 D2009 XK3118 D11 D12 D13系列":
                    runPort = new RunPort(D2008_D2009);
                    break;
                case "上海耀华XK3190系列2 D2+专用":
                case "福建科达KD-8137QD2+":
                case "大华XK3199D2+系列":
                case "上海彩信XK315A系列2反序":
                    runPort = new RunPort(XK3190_KD_8137QD2_XK3199D2_XK315A);
                    break;
                case "宁波博达-北京瑞迪宇衡XK3180系列":
                    runPort = new RunPort(XK3180);
                    break;
                case "上海东南衡器XK3188-A9系列":
                case "上海东南衡器XK3188-A30系列":
                case "上海耀华XK3190系列1":
                case "重庆大唐DT3101":
                case "杭州万博WS-822":
                    runPort = new RunPort(XK3188_A9_XK3188_A30);
                    break;
                case "北京能克XK319A系列":
                    runPort = new RunPort(XK319A);
                    break;
                case "四方XK3196系列":
                    runPort = new RunPort(XK3196);
                    break;
                case "上海彩信XK315 A6 系列":
                    runPort = new RunPort(XK315_A6);
                    break;
                case "天津世铨Vishay威世VT300":
                    runPort = new RunPort(VT300);
                    break;
                case "美国传力T1-1520系列":
                case "UMC555江苏赛摩系列":
                    runPort = new RunPort(UMC555_T1_1520);
                    break;
                case "上海彩信XK315A系列1正序":
                    runPort = new RunPort(XK315A);
                    break;
                case "常州托利多8142系列(1)XK3127系列T800系列":
                case "广东南方衡器8142-07":
                    runPort = new RunPort(XK3127_8142_07);
                    break;
                case "常州托利多XK3130系列":
                    runPort = new RunPort(XK3130);
                    break;
                case "常州托利多8142系列T800(2)":
                    runPort = new RunPort(T800);
                    break;
                case "赛多利斯":
                    runPort = new RunPort(SDLS);
                    break;
                case "宁波柯力D2002系列":
                case "上海东南衡器XK3188-T3系列":
                    runPort = new RunPort(D2002_XK3188_T3);
                    break;
                case "济南金钟3102系列":
                    runPort = new RunPort(JZ3102);
                    break;
                case "沈阳鲁尔XK3106系列":
                    runPort = new RunPort(XK3106);
                    break;
                case "杭州衡天HT9800特殊方式":
                    runPort = new RunPort(HT9800);
                    break;
                case "杭州科利系列XK3110":
                case "常州托利多XK3124 ind245 ind880":
                case "常州托利多XK3124 ind245":
                    runPort = new RunPort(XK3110_XK3124_ind245);
                    break;
                case "宁波柯力D2002EC D2002ED D2002EF D2002EH":
                    runPort = new RunPort(D2002EC_D2002ED);
                    break;
                case "正鼎联邦DK3230系列":
                    runPort = new RunPort(DK3230);
                    break;
                case "德国富林泰克(flintec)系列FT-11D":
                    runPort = new RunPort(FT_11D);
                    break;
                case "杭州衡天 HT9800系列1 BCD码":
                case "聊城昌信XK3168系列":
                default:
                    runPort = new RunPort(DefaultParsing);
                    break;
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public string Run()
        {
            return runPort.Invoke();
        }
        public void Run(GetData getData)
        {
            runPort.Invoke();
            getData(FinalData);
        }

        /// <summary>
        /// 聊城华科XK3199
        /// </summary>
        /// <returns></returns>
        private string XK3199()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == byte.MaxValue)
            {
                for (int index = 0; index < 4; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("FF", "00").ToCharArray();
                string str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + charArray[5].ToString() + charArray[2].ToString() + charArray[3].ToString();
                string str3 = !(charArray[1].ToString() == "1") ? int.Parse(str2.Trim()).ToString() : (!(charArray[0].ToString() == "3") ? int.Parse(str2.Trim()).ToString() : "-" + int.Parse(str2.Trim()).ToString());
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 耀华XK3190A12E A27E系列
        /// </summary>
        /// <returns></returns>
        private string XK3190A12E_A27E()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 10)
            {
                for (int index = 0; index < 11; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                //this.richTextBox2.AppendText(string.Format("{0:X2} ", (object)str1) + "\n");
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                char[] charArray = str2.ToCharArray();
                string str3 = charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString() + charArray[19].ToString();
                string str4 = !(charArray[5].ToString() == "D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString();
                if (str4 == "0" && str2.Contains("."))
                    str4 = "0.00";
                return FinalData = str4;
            }
            return "";
        }

        /// <summary>
        /// 杭州科利系列XK3110-E XK3138
        /// </summary>
        /// <returns></returns>
        private string XK3110E_XK3138()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 130)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("B", "3").ToCharArray();
                string str2 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                string str3 = !(charArray[3].ToString() == "0") ? "-" + double.Parse(str2.Trim()).ToString() : double.Parse(str2.Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 上海秋毫QDI系列1
        /// </summary>
        /// <returns></returns>
        private string QDI1()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "");
                char[] charArray = str2.Replace("D", "0").ToCharArray();
                string str3 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 上海秋毫QDI系列2
        /// </summary>
        /// <returns></returns>
        private string QDI2()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 61)
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "");
                char[] charArray = str2.Replace("2D", "").ToCharArray();
                string str3 = charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// XK3101系列
        /// </summary>
        /// <returns></returns>
        private string XK3101()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 13)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2;
                if (charArray[5].ToString() == "D")
                    str2 = "-" + double.Parse((charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString()).Trim()).ToString();
                else
                    str2 = double.Parse((charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString()).Trim()).ToString();
                return FinalData = str2;
            }
            return "";
        }

        /// <summary>
        /// 杭州顶松系列DS822
        /// </summary>
        /// <returns></returns>
        private string DS822()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 65)
            {
                for (int index = 0; index < 10; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString();
                string s = !(charArray[3].ToString() == "B") ? "-" + double.Parse(str2.Trim()).ToString() : double.Parse(str2.Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(s) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 志美PT650F
        /// </summary>
        /// <returns></returns>
        private string PT650F()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 44)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                char[] charArray = str2.ToCharArray();
                string str3 = charArray[15].ToString() + charArray[13].ToString() + charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' '); ;
            }
            return "";
        }

        /// <summary>
        /// 上海唐衡T9901
        /// 上海三积分系列
        /// 福州科杰XK3198 ORMT-J2000系列
        /// </summary>
        /// <returns></returns>
        private string T9901_XK3198()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 61)
            {
                for (int index = 0; index < 7; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                double num;
                string str3;
                if (str2.Contains("2D"))
                {
                    char[] charArray = str2.Replace("2D", "").ToCharArray();
                    string str4 = charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString();
                    string str5 = "-";
                    num = double.Parse(str4.Trim());
                    string str6 = num.ToString();
                    str3 = str5 + str6;
                }
                else if (str2.Contains("20"))
                {
                    char[] charArray = str2.Replace("20", "").ToCharArray();
                    num = double.Parse((charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString()).Trim());
                    str3 = num.ToString();
                }
                else
                {
                    char[] charArray = str2.Replace("20", "").ToCharArray();
                    num = double.Parse((charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString()).Trim());
                    str3 = num.ToString();
                }
                return FinalData = str3;
            }
            return "";
        }

        /// <summary>
        /// 上海友声XK3100A2 A3 B3 系列
        /// </summary>
        /// <returns></returns>
        private string XK3100A2_A3_B3()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 61)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                char[] charArray = str2.ToCharArray();
                string str3 = charArray[15].ToString() + charArray[13].ToString() + charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 上海友声XK3100B2 ACS天平 TCS台秤全系列
        /// 上海友声XK3100-D1 D2+ D2+P
        /// </summary>
        /// <returns></returns>
        private string XK3100B2_XK3100_D1()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == (int)byte.MaxValue)
            {
                for (int index = 0; index < 4; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2;
                if (charArray[1].ToString() == "3")
                    str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + charArray[5].ToString() + "." + charArray[2].ToString() + charArray[3].ToString();
                else if (charArray[1].ToString() == "2")
                    str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + charArray[5].ToString() + charArray[2].ToString() + "." + charArray[3].ToString();
                else if (charArray[1].ToString() == "4")
                    str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + "." + charArray[5].ToString() + charArray[2].ToString() + charArray[3].ToString();
                else
                    str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + charArray[5].ToString() + charArray[2].ToString() + charArray[3].ToString();
                return FinalData = double.Parse(str2.Trim()).ToString().PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 宁波柯力D2008 D2009 XK3118 D11 D12 D13系列
        /// </summary>
        /// <returns></returns>
        private string D2008_D2009()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                if (str1.Contains("2E"))
                {
                    string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                    char[] charArray = str2.ToCharArray();
                    string str3 = charArray[13].ToString() + charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString();
                    return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
                }
                else
                {
                    char[] charArray = str1.Replace(" ", "").Replace("2E", "3.").ToCharArray();
                    string str2 = charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString();
                    string str3 = charArray[1] != 'B' ? "-" + double.Parse(str2.Trim()).ToString() : double.Parse(str2.Trim()).ToString();
                    return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 1000.0));
                }
            }
            return "";
        }

        /// <summary>
        /// 上海耀华XK3190系列2 D2+专用
        /// 福建科达KD-8137QD2+
        /// 大华XK3199D2+系列
        /// 上海彩信XK315A系列2反序
        /// </summary>
        /// <returns></returns>
        private string XK3190_KD_8137QD2_XK3199D2_XK315A()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 61)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                char[] charArray = str2.ToCharArray();
                string str3 = charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 宁波博达-北京瑞迪宇衡XK3180系列
        /// </summary>
        /// <returns></returns>
        private string XK3180()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                return FinalData = (!(charArray[3].ToString() == "2") ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 上海东南衡器XK3188-A9系列
        /// 上海东南衡器XK3188-A30系列
        /// 上海耀华XK3190系列1
        /// 重庆大唐DT3101
        /// 杭州万博WS-822
        /// </summary>
        /// <returns></returns>
        private string XK3188_A9_XK3188_A30()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("2E", "3.").ToCharArray();
                string str2 = charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString();
                string str3 = charArray[1] != 'B' ? "-" + double.Parse(str2.Trim()).ToString() : double.Parse(str2.Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 1000.0));
            }
            return "";
        }

        /// <summary>
        /// 北京能克XK319A系列
        /// </summary>
        /// <returns></returns>
        private string XK319A()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                return FinalData = (!(charArray[3].ToString() == "A") ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 四方XK3196系列
        /// </summary>
        /// <returns></returns>
        private string XK3196()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 205)
            {
                for (int index = 0; index < 7; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "");
                char[] charArray = str2.Replace("A", "0").Replace("B", "0").ToCharArray();
                string str3 = charArray[1].ToString() + charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString();
                return FinalData = (!str2.Contains("B") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 上海彩信XK315 A6 系列
        /// </summary>
        /// <returns></returns>
        private string XK315_A6()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                string str3 = !(charArray[3].ToString() == "2") ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 天津世铨Vishay威世VT300
        /// </summary>
        /// <returns></returns>
        private string VT300()
        {
            string str1 = "";
            switch (this.SerialPortObj.ReadByte())
            {
                case 13:
                case 141:
                    for (int index = 0; index < 8; ++index)
                        str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                    Thread.Sleep(10);
                    char[] charArray = str1.Replace(" ", "").ToCharArray();
                    string str2 = charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString();
                    string str3 = charArray[3] != 'B' ? "-" + double.Parse(str2.Trim()).ToString() : double.Parse(str2.Trim()).ToString();
                    return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 美国传力T1-1520系列
        /// UMC555江苏赛摩系列
        /// </summary>
        /// <returns></returns>
        private string UMC555_T1_1520()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString();
                return FinalData = (charArray[1] != 'D' ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 上海彩信XK315A系列1正序
        /// </summary>
        /// <returns></returns>
        private string XK315A()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == pondDataParameter.StartTag)
            {
                for (int index = 0; index < pondDataParameter.Dujiwei; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("B", "3").ToCharArray();
                int index1 = pondDataParameter.Kaishidu;
                string str2;
                if (pondDataParameter.Zhengfanxu == 0)
                    str2 = charArray[index1].ToString() + charArray[index1 + 2].ToString() + charArray[index1 + 4].ToString() + charArray[index1 + 6].ToString() + charArray[index1 + 8].ToString() + charArray[index1 + 10].ToString();
                else
                    str2 = charArray[index1 + 10].ToString() + charArray[index1 + 8].ToString() + charArray[index1 + 6].ToString() + charArray[index1 + 4].ToString() + charArray[index1 + 2].ToString() + charArray[index1].ToString();
                string str3 = !(charArray[pondDataParameter.Fushuwei] == pondDataParameter.Fu) ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 常州托利多8142系列(1)XK3127系列T800系列
        /// 广东南方衡器8142-07
        /// </summary>
        /// <returns></returns>
        private string XK3127_8142_07()
        {
            string str1 = "";
            if (string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) == "82")
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString()).Trim()).ToString();
                if (charArray[3].ToString() == "3")
                    str2 = "-" + str2;
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str2.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 常州托利多XK3130系列
        /// </summary>
        /// <returns></returns>
        private string XK3130()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 58)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString()).Trim()).ToString();
                if (charArray[1].ToString() == "3")
                    str2 = "-" + str2;
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str2.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 常州托利多8142系列T800(2)
        /// </summary>
        /// <returns></returns>
        private string T800()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString()).Trim()).ToString();
                if (charArray[3].ToString() == "2")
                    str2 = "-" + str2;
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str2.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 赛多利斯
        /// </summary>
        /// <returns></returns>
        private string SDLS()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 72)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[1].ToString() + charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString()).Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str2.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 宁波柯力D2002系列
        /// 上海东南衡器XK3188-T3系列
        /// </summary>
        /// <returns></returns>
        private string D2002_XK3188_T3()
        {
            string str1 = "";
            string news2 = "";
            if (this.SerialPortObj.ReadByte() == 46)
            {
                for (int index = 0; index < 7; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Trim().Substring(0, 13).ToCharArray();
                double num;
                if (charArray[11].ToString() == "D")
                {
                    news2 = charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString();
                    string str2 = "-";
                    num = double.Parse(news2.Trim());
                    string str3 = num.ToString();
                    news2 = str2 + str3;
                }
                else
                {
                    news2 = charArray[11].ToString() + charArray[9].ToString() + charArray[7].ToString() + charArray[5].ToString() + charArray[3].ToString() + charArray[1].ToString();
                    num = double.Parse(news2.Trim());
                    news2 = num.ToString();
                }
                news2 = news2.PadLeft(6, ' ');
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(news2) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 济南金钟3102系列
        /// </summary>
        /// <returns></returns>
        private string JZ3102()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 10)
            {
                for (int index = 0; index < 15; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace(" ", "").Replace("2E", "3.");
                double num;
                string str3;
                if (str2.Contains("2D"))
                {
                    char[] charArray = str2.ToCharArray();
                    string str4 = charArray[19].ToString() + charArray[21].ToString() + charArray[23].ToString() + charArray[25].ToString() + charArray[27].ToString();
                    string str5 = "-";
                    num = double.Parse(str4.Trim());
                    string str6 = num.ToString();
                    str3 = str5 + str6;
                }
                else
                {
                    char[] charArray = str2.ToCharArray();
                    num = double.Parse((charArray[17].ToString() + charArray[19].ToString() + charArray[21].ToString() + charArray[23].ToString() + charArray[25].ToString() + charArray[27].ToString()).Trim());
                    str3 = num.ToString();
                }
                return FinalData = str3;
            }
            return "";
        }

        /// <summary>
        /// 沈阳鲁尔XK3106系列
        /// </summary>
        /// <returns></returns>
        private string XK3106()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 8; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("2E", "3.").ToCharArray();
                string str2 = charArray[3].ToString() + charArray[5].ToString() + charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString();
                string str3 = charArray[1] != 'B' ? (double.Parse(str2.Trim()) <= 0.0 ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString()) : double.Parse(str2.Trim()).ToString();
                if (charArray[15].ToString() == "0")
                {
                    FinalData = str3.PadLeft(6, ' ');
                }
                else if (charArray[15].ToString() == "2")
                {
                    FinalData = string.Format("{0:f2}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 100.0));
                }
                else if (charArray[15].ToString() == "1")
                {
                    FinalData = string.Format("{0:f1}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 10.0));
                }
                else if (charArray[15].ToString() == "3")
                {
                    FinalData = string.Format("{0:f3}", (object)(double.Parse(str3.PadLeft(6, ' ')) / 1000.0));
                }
                else
                {
                    FinalData = str3.PadLeft(6, ' ');
                }

                return FinalData;
            }
            return "";
        }

        /// <summary>
        /// 杭州衡天HT9800特殊方式
        /// </summary>
        /// <returns></returns>
        private string HT9800()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                string str2 = str1.Replace("2E", "3.").Replace(" ", "");
                char[] charArray = str2.ToCharArray();
                string str3 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                return FinalData = (!str2.Contains("2D") ? double.Parse(str3.Trim()).ToString() : "-" + double.Parse(str3.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 杭州科利系列XK3110
        /// 常州托利多XK3124 ind245 ind880
        /// 常州托利多XK3124 ind245
        /// </summary>
        /// <returns></returns>
        private string XK3110_XK3124_ind245()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString();
                return FinalData = (!(charArray[3].ToString() == "2") ? double.Parse(str2.Trim()).ToString() : "-" + double.Parse(str2.Trim()).ToString()).PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 宁波柯力D2002EC D2002ED D2002EF D2002EH
        /// </summary>
        /// <returns></returns>
        private string D2002EC_D2002ED()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 2)
            {
                for (int index = 0; index < 9; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("E", ".").ToCharArray();
                string str2 = double.Parse((charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString()).Trim()).ToString();
                if (charArray[3].ToString() == "2")
                    str2 = "-" + str2;
                return FinalData = str2.PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 正鼎联邦DK3230系列
        /// </summary>
        /// <returns></returns>
        private string DK3230()
        {
            string str1 = "";
            if (string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) == "82")
            {
                for (int index = 0; index < 16; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[7].ToString() + charArray[9].ToString() + charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString()).Trim()).ToString();
                if (charArray[3].ToString() == "2")
                    str2 = "-" + str2;
                return FinalData = str2.PadLeft(6, ' ');
            }
            return "";
        }

        /// <summary>
        /// 德国富林泰克(flintec)系列FT-11D
        /// </summary>
        /// <returns></returns>
        private string FT_11D()
        {
            string str1 = "";
            if (this.SerialPortObj.ReadByte() == 13)
            {
                for (int index = 0; index < 12; ++index)
                    str1 = str1 + string.Format("{0:X2}", (object)this.SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").ToCharArray();
                string str2 = double.Parse((charArray[11].ToString() + charArray[13].ToString() + charArray[15].ToString() + charArray[17].ToString() + charArray[19].ToString() + charArray[21].ToString()).Trim()).ToString();
                return FinalData = string.Format("{0:f2}", (object)(double.Parse(str2.PadLeft(6, ' ')) / 100.0));
            }
            return "";
        }

        /// <summary>
        /// 默认解析
        /// 杭州衡天 HT9800系列1 BCD码
        /// 聊城昌信XK3168系列 
        /// </summary>
        /// <returns></returns>
        private string DefaultParsing()
        {
            string str1 = "";
            if (SerialPortObj.ReadByte() == byte.MaxValue)
            {
                for (int i = 0; i < 4; i++)
                    str1 = str1 + string.Format("{0:X2}", SerialPortObj.ReadByte()) + " ";
                Thread.Sleep(10);
                char[] charArray = str1.Replace(" ", "").Replace("FF", "00").ToCharArray();
                string str2 = charArray[6].ToString() + charArray[7].ToString() + charArray[4].ToString() + charArray[5].ToString() + charArray[2].ToString() + charArray[3].ToString();
                string str3 = !(charArray[1].ToString() == "1") ? int.Parse(str2.Trim()).ToString() : (!(charArray[0].ToString() == "3") ? int.Parse(str2.Trim()).ToString() : "-" + int.Parse(str2.Trim()).ToString());
                return FinalData = string.Format("{0:f2}", (double.Parse(str3.PadLeft(6, ' ')) / 1000.0));
            }
            return "";
        }
    }
}
