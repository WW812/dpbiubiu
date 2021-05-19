using biubiu.model;
using biubiu.model.print;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Threading;
using biubiu.model.ship_order;
using biubiu.model.stock_order;
using biubiu.Domain.biuMessageBox;
using System.Windows.Controls;

namespace biubiu.Domain
{
    public class BillPrinter
    {
        private BillConfig BConfig;
        private double Width = 0;
        private double Height = 0;
        private double MarginTop = 0;
        private double MarginLeft = 0;
        private double MarginBottom = 0;
        private double MarginRight = 0;
        private string Scale = null; // 打算用来记录长度单位例如（cm、in）, 报表中存在此值代表新票据文件，没有为旧票据文件
        private int m_currentPageIndex;
        //声明一个Stream对象的列表用来保存报表的输出数据
        //LocalReport对象的Render方法会将报表按页输出为多个Stream对象。
        private List<Stream> m_streams;
        private string _hashCode = ""; //信息组成
        private static BillPrinter _billPrinter;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        private BillPrinter()
        {
        }

        public static BillPrinter GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (_billPrinter == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (_billPrinter == null)
                    {
                        _billPrinter = new BillPrinter();
                    }
                }
            }
            return _billPrinter;
        }

        //用来提供Stream对象的函数，用于LocalReport对象的Render方法的第三个参数。
        private Stream CreateStream(string name, string fileNameExtension,
          Encoding encoding, string mimeType, bool willSeek)
        {
            //如果需要将报表输出的数据保存为文件，请使用FileStream对象。
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void PrintShip(ShipOrder so, string mendStr = "(补)")
        {
            ShipOrderDataSet dataSet = new ShipOrderDataSet();
            dataSet.ByShipOrder(Common.DeepCopy<ShipOrder>(so));
            BConfig = dataSet.Status.Equals("未结账") ? Config.SHIP_ENTER_CONFIG : Config.SHIP_EXIT_CONFIG;
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderNo", typeof(string));
            dt.Columns.Add("EnterOrderNo", typeof(string));
            dt.Columns.Add("CarTare", typeof(double));
            dt.Columns.Add("CarID", typeof(string));
            dt.Columns.Add("GoodsPrice", typeof(double));
            dt.Columns.Add("GoodsRealPrice", typeof(double));
            dt.Columns.Add("GoodsName", typeof(string));
            dt.Columns.Add("CarGrossWeight", typeof(double));
            dt.Columns.Add("CarNetWeight", typeof(double));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("CustomerBalance", typeof(double));
            dt.Columns.Add("OrderMoney", typeof(double));
            dt.Columns.Add("DiscountMoney", typeof(double));
            dt.Columns.Add("RealMoney", typeof(double));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("EmptyCar", typeof(string));
            dt.Columns.Add("Hedge", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CustomerType", typeof(string));
            dt.Columns.Add("AdvanceWeight", typeof(double));
            dt.Columns.Add("EnterPonderation", typeof(string));
            dt.Columns.Add("ExitPonderation", typeof(string));
            dt.Columns.Add("EditTime", typeof(string));
            dt.Columns.Add("EditUser", typeof(string));
            dt.Columns.Add("EditReason", typeof(string));
            dt.Columns.Add("EditNote", typeof(string));
            dt.Columns.Add("EnterTime", typeof(string));
            dt.Columns.Add("ExitTime", typeof(string));
            dt.Columns.Add("EnterUser", typeof(string));
            dt.Columns.Add("ExitUser", typeof(string));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("CustomerShowMoney", typeof(bool));
            DataRow dr = dt.NewRow();
            dr["OrderNo"] = dataSet.OrderNo;
            dr["EnterOrderNo"] = dataSet.EnterOrderNo;
            dr["CarTare"] = dataSet.CarTare;
            dr["CarId"] = dataSet.CarId;
            dr["GoodsPrice"] = dataSet.GoodsPrice;
            dr["GoodsRealPrice"] = dataSet.GoodsRealPrice;
            dr["GoodsName"] = dataSet.GoodsName;
            dr["CarGrossWeight"] = dataSet.CarGrossWeight;
            dr["CarNetWeight"] = dataSet.CarNetWeight;
            dr["CustomerName"] = dataSet.CustomerName;
            dr["CustomerBalance"] = dataSet.CustomerBalance;
            dr["OrderMoney"] = dataSet.OrderMoney;
            dr["DiscountMoney"] = dataSet.DiscountMoney;
            dr["RealMoney"] = dataSet.RealMoney;
            dr["Status"] = dataSet.Status;
            dr["EmptyCar"] = dataSet.EmptyCar;
            dr["Hedge"] = dataSet.Hedge;
            dr["Type"] = dataSet.Type;
            dr["CustomerType"] = dataSet.CustomerType;
            dr["AdvanceWeight"] = dataSet.AdvanceWeight;
            dr["EnterPonderation"] = dataSet.EnterPonderation;
            dr["ExitPonderation"] = dataSet.ExitPonderation;
            dr["EditTime"] = dataSet.EditTime;
            dr["EditUser"] = dataSet.EditUser;
            dr["EditReason"] = dataSet.EditReason;
            dr["EditNote"] = dataSet.EditNote;
            dr["EnterTime"] = dataSet.EnterTime;
            dr["ExitTime"] = dataSet.ExitTime;
            dr["EnterUser"] = dataSet.EnterUser;
            dr["ExitUser"] = dataSet.ExitUser;
            dr["Note"] = dataSet.Note;
            dr["CustomerShowMoney"] = dataSet.CustomerShowMoney;
            dt.Rows.Add(dr);
            _hashCode = "";
            _hashCode += so.ID;
            _hashCode += (so.EnterTime == 0) ? "" : so.EnterTime.ToString();
            _hashCode += (so.ExitTime == 0) ? "" : so.ExitTime.ToString();
            _hashCode += so.Goods.Name;
            _hashCode += so.CarNetWeight.ToString();
            _hashCode += so.RealMoney.ToString();
            _hashCode += (so.CustomerType == 0) ? "" : so.Customer.Name;
            _hashCode += so.CarId;
            var printTimes = so.Status == 0 ? Config.SHIP_ENTER_CONFIG.PrintTimes : Config.SHIP_EXIT_CONFIG.PrintTimes; //打印次数
            for (int i = 0; i < printTimes; i++)
            {
                Print(dt, mendStr);
            }
        }

        public void PrintStock(StockOrder so , string mendStr = "(补)")
        {
            StockOrderDataSet dataSet = new StockOrderDataSet();
            dataSet.ByStockOrder(Common.DeepCopy<StockOrder>(so));
            BConfig = dataSet.Status.Equals("出厂") ? Config.STOCK_EXIT_CONFIG : Config.STOCK_ENTER_CONFIG;
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderNo", typeof(string));
            dt.Columns.Add("EnterOrderNo", typeof(string));
            dt.Columns.Add("CarTare", typeof(double));
            dt.Columns.Add("CarId", typeof(string));
            dt.Columns.Add("GoodsRealPrice", typeof(double));
            dt.Columns.Add("GoodsName", typeof(string));
            dt.Columns.Add("CarGrossWeight", typeof(double));
            dt.Columns.Add("CarNetWeight", typeof(double));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("CustomerNonpaymentAmount", typeof(double));
            dt.Columns.Add("RealMoney", typeof(double));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CustomerType", typeof(string));
            dt.Columns.Add("DeductWeight", typeof(double));
            dt.Columns.Add("DeductWeightType", typeof(int));
            dt.Columns.Add("DeductWeightTypeText", typeof(string));
            dt.Columns.Add("FreightOfTon", typeof(double));
            dt.Columns.Add("Paid", typeof(int));
            dt.Columns.Add("PaidText", typeof(string));
            dt.Columns.Add("EnterPonderation", typeof(string));
            dt.Columns.Add("ExitPonderation", typeof(string));
            dt.Columns.Add("EnterTime", typeof(string));
            dt.Columns.Add("ExitTime", typeof(string));
            dt.Columns.Add("EditTime", typeof(string));
            dt.Columns.Add("EnterUser", typeof(string));
            dt.Columns.Add("ExitUser", typeof(string));
            dt.Columns.Add("DeductWeightText", typeof(string));
            DataRow dr = dt.NewRow();
            dr["OrderNo"] = dataSet.OrderNo;
            dr["EnterOrderNo"] = dataSet.EnterOrderNo;
            dr["CarTare"] = dataSet.CarTare;
            dr["CarId"] = dataSet.CarId;
            dr["GoodsRealPrice"] = dataSet.GoodsRealPrice;
            dr["GoodsName"] = dataSet.GoodsName;
            dr["CarGrossWeight"] = dataSet.CarGrossWeight;
            dr["CarNetWeight"] = dataSet.CarNetWeight;
            dr["CustomerName"] = dataSet.CustomerName;
            dr["CustomerNonpaymentAmount"] = dataSet.CustomerNonpaymentAmount;
            dr["RealMoney"] = dataSet.RealMoney;
            dr["Status"] = dataSet.Status;
            dr["Type"] = dataSet.Type;
            dr["CustomerType"] = dataSet.CustomerType;
            dr["DeductWeight"] = dataSet.DeductWeight;
            dr["DeductWeightType"] = dataSet.DeductWeightType;
            dr["DeductWeightTypeText"] = dataSet.DeductWeightTypeText;
            dr["FreightOfTon"] = dataSet.FreightOfTon;
            dr["Paid"] = dataSet.Paid;
            dr["PaidText"] = dataSet.PaidText;
            dr["EnterPonderation"] = dataSet.EnterPonderation;
            dr["ExitPonderation"] = dataSet.ExitPonderation;
            dr["EnterTime"] = dataSet.EnterTime;
            dr["ExitTime"] = dataSet.ExitTime;
            dr["EditTime"] = dataSet.EditTime;
            dr["EnterUser"] = dataSet.EnterUser;
            dr["ExitUser"] = dataSet.ExitUser;
            dr["DeductWeightText"] = dataSet.DeductWeightText;
            dt.Rows.Add(dr);
            _hashCode = "";
            _hashCode += so.ID;
            _hashCode += (so.EnterTime == 0) ? "" : so.EnterTime.ToString();
            _hashCode += (so.ExitTime == 0) ? "" : so.ExitTime.ToString();
            _hashCode += so.Goods.Name;
            _hashCode += so.CarNetWeight.ToString();
            _hashCode += so.RealMoney.ToString();
            _hashCode += (so.CustomerType == 0) ? "" : so.Customer.Name;
            _hashCode += so.CarId;
            var printTimes = so.Status == 0 ? Config.STOCK_ENTER_CONFIG.PrintTimes : Config.STOCK_EXIT_CONFIG.PrintTimes; //打印次数
            for (int i = 0; i < printTimes; i++)
            {
                Print(dt, mendStr);
            }
        }

        private void Print(DataTable dt, string mendStr = "(补)")
        {
            m_streams = new List<Stream>();

            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = "DataSet1",
                Value = dt
            };

            if (!File.Exists(BConfig.TemplatePath))
            {
                BiuMessageBoxWindows.BiuShow("票据模板不存在，打印失败!", image: BiuMessageBoxImage.Error);
                return;
            }

            LocalReport report = new LocalReport
            {
                ReportPath = BConfig.TemplatePath
            };

            var code = dt.Rows[0]["OrderNo"].ToString() + "+" + Common.GetMD5ByString(_hashCode);

            #region 二维码
            if (report.GetParameters()["QRCode"] != null)  // 含有此变量则进行二维码填充
            {
                Bitmap bmp;
                if (File.Exists(Config.ICON_PATH))
                {
                    bmp = Encoder.CodeIcon(code, 5, 7, Config.ICON_PATH, 20, 5, false);
                }
                else
                {
                    bmp = Encoder.Code(code, 5, 7, false);
                }
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Bmp);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                string strB64 = Convert.ToBase64String(byteImage);
                report.SetParameters(new ReportParameter[] {
                    new ReportParameter("QRCode",strB64)
                    });
                ms.Close();
                bmp.Dispose();
            }
            #endregion

            report.DataSources.Add(reportDataSource);
            Width = System.Convert.ToDouble(report.GetParameters()["Width"].Values[0]);
            Height = System.Convert.ToDouble(report.GetParameters()["Height"].Values[0]);
            MarginTop = System.Convert.ToDouble(report.GetParameters()["MarginTop"].Values[0]);
            MarginLeft = System.Convert.ToDouble(report.GetParameters()["MarginLeft"].Values[0]);
            MarginRight = System.Convert.ToDouble(report.GetParameters()["MarginRight"].Values[0]);
            MarginBottom = System.Convert.ToDouble(report.GetParameters()["MarginBottom"].Values[0]);
            Scale = report.GetParameters()["Scale"]?.Values[0];

            ReportParameter title = new ReportParameter("Title", Config.COMPANY_NAME + BConfig.TitleSuffix + mendStr);
            ReportParameter footer = new ReportParameter("Footer", BConfig.FooterWord);
            report.SetParameters(new ReportParameter[] { title, footer });

            string deviceInfo =
          "<DeviceInfo>" +
          "  <OutputFormat>EMF</OutputFormat>" +
          "  <PageWidth>" + Width + "cm</PageWidth>" +
          "  <PageHeight>" + Height + "cm</PageHeight>" +
          "  <MarginTop>" + MarginTop + "cm</MarginTop>" +
          "  <MarginLeft>" + MarginLeft + "cm</MarginLeft>" +
          "  <MarginRight>" + MarginRight + "cm</MarginRight>" +
          "  <MarginBottom>" + MarginBottom + "cm</MarginBottom>" +
          "</DeviceInfo>";
            Warning[] warnings;
            report.Render("Image", deviceInfo, CreateStream, out warnings);

            m_currentPageIndex = 0;

            if (m_streams == null || m_streams.Count == 0) return;
            //声明PrintDocument对象用于数据的打印
            PrintDocument printDoc = new PrintDocument();
            //指定需要使用的打印机的名称，使用空字符串""来指定默认打印机
            printDoc.PrinterSettings.PrinterName = BConfig.IsAppoint ? BConfig.ApponitPrinterName : new PrinterSettings().PrinterName;
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)Common.CentimeterConvertToInch(Width * 100), (int)Common.CentimeterConvertToInch(Height * 100));
            // >>>>>>>>>>>>>>>> 新票据设置边距, 所有票据改为新的之后可以删除 if判断
            /*
            if (!string.IsNullOrEmpty(Scale))
            {
                //printDoc.OriginAtMargins = true;
                printDoc.DefaultPageSettings.Margins = new Margins
                {
                    Left = (int)Common.CentimeterConvertToInch(MarginLeft * 100),
                    Top = (int)Common.CentimeterConvertToInch(MarginTop * 100),
                    Right = (int)Common.CentimeterConvertToInch(MarginRight * 100),
                    Bottom = (int)Common.CentimeterConvertToInch(MarginBottom * 100)
                };
            }
            */

            //printDoc.DefaultPageSettings.PaperSize = new PaperSize("Custom", 472, 367);
            //判断指定的打印机是否可用
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("没有发现打印机:" + BConfig.ApponitPrinterName);
            }
            //声明PrintDocument对象的PrintPage事件，具体的打印操作需要在这个事件中处理。
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            //执行打印操作，Print方法将触发PrintPage事件。

            printDoc.Print();
            report.Dispose();
            printDoc.Dispose();
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            //Metafile对象用来保存EMF或WMF格式的图形，
            //我们在前面将报表的内容输出为EMF图形格式的数据流。
            m_streams[m_currentPageIndex].Position = 0;

            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

            //指定是否横向打印
            ev.PageSettings.Landscape = false;
            //这里的Graphics对象实际指向了打印机
            #region 兼容代码
            /// >>>>>>>>>>> 如果票据全部改成新票据，可以删除旧代码
            if (string.IsNullOrEmpty(Scale)) //旧票据
            {
                ev.Graphics.DrawImage(pageImage, 0, 0, pageImage.Width, pageImage.Height);
            }
            else
            {
                // 新票据，根据分辨率重新计算宽高
                var w = pageImage.HorizontalResolution * Common.CentimeterConvertToInch(Width);
                var h = pageImage.VerticalResolution * Common.CentimeterConvertToInch(Height);
                ev.Graphics.DrawImage(pageImage, 0, 0, (int)w, (int)h);
            }
            #endregion
            m_streams[m_currentPageIndex].Close();
            m_currentPageIndex++;

            //设置是否需要继续打印
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            pageImage.Dispose();
        }
    }
}
