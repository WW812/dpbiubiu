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
            int port = 0;
            byte fComAdr = 255;
            int frmcomportindex = 0;
            int fCmdRet = 30; //所有执行指令的返回值
            int openresult = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, 5, ref frmcomportindex);
            if (openresult == 0)
            {
                if ((fCmdRet == 0x35) | (fCmdRet == 0x30))
                {
                    StaticClassReaderB.CloseSpecComPort(frmcomportindex);
                    throw new Exception("串口通讯错误");
                }
                while (true)
                {
                    Thread.Sleep(300);
                    Inventory(frmcomportindex);
                }
            }
        }
        static private void Inventory(int frmcomportindex)
        {
            int i;
            int CardNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[5000];
            int CardIndex;
            string temps;
            string s, sEPC;
            bool isonlistview;
            bool fIsInventoryScan = true;
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;
            {
                AdrTID = 0;
                LenTID = 0;
                TIDFlag = 0;
            }
            byte fComAdr = 255;
            var fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//代表已查找结束，
            {
                byte[] daw = new byte[Totallen];
                Array.Copy(EPC, daw, Totallen);
                temps = ByteArrayToHexString(daw);
                m = 0;

                /*   while (ListView1_EPC.Items.Count < CardNum)
                  {
                      aListItem = ListView1_EPC.Items.Add((ListView1_EPC.Items.Count + 1).ToString());
                      aListItem.SubItems.Add("");
                      aListItem.SubItems.Add("");
                      aListItem.SubItems.Add("");
                 * 
                  }*/
                if (CardNum == 0)
                {
                    fIsInventoryScan = false;
                    return;
                }
                for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                    EPClen = daw[m];
                    sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                    m = m + EPClen + 1;
                    if (sEPC.Length != EPClen * 2)
                        return;
                    s = sEPC;
                    s = (sEPC.Length / 2).ToString().PadLeft(2, '0');
                    Console.WriteLine(sEPC);
                }
            }
            fIsInventoryScan = false;
        }

        static private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
    }
}
