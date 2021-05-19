using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.print;
using biubiu.model.ship_goods;
using biubiu.model.user;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows;


namespace biubiu.views.setting.print
{
    /// <summary>
    /// PreviewBillTemplateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewBillTemplateWindow : Window
    {
        private BillConfig BConfig;
        private double width = 0;
        private double height = 0;
        //private int m_currentPageIndex;

        public PreviewBillTemplateWindow(BillConfig bill)
        {
            InitializeComponent();
            if (!File.Exists(bill.TemplatePath))
            {
                BiuMessageBoxWindows.BiuShow("模板文件不存在,加载失败!", image: BiuMessageBoxImage.Error);
                return;
            }
            BConfig = bill;
            try
            {
                Print();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        //声明一个Stream对象的列表用来保存报表的输出数据
        //LocalReport对象的Render方法会将报表按页输出为多个Stream对象。
        private List<Stream> m_streams;

        //用来提供Stream对象的函数，用于LocalReport对象的Render方法的第三个参数。
        private Stream CreateStream(string name, string fileNameExtension,
          Encoding encoding, string mimeType, bool willSeek)
        {
            //如果需要将报表输出的数据保存为文件，请使用FileStream对象。
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void Print()
        {
            m_streams = new List<Stream>();

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
            DataRow dr = dt.NewRow();
            var dateSet = new ShipOrderDataSet();
            dr["OrderNo"] = dateSet.OrderNo;
            dr["EnterOrderNo"] = dateSet.EnterOrderNo;
            dr["CarTare"] = dateSet.CarTare;
            dr["CarId"] = dateSet.CarId;
            dr["GoodsPrice"] = dateSet.GoodsPrice;
            dr["GoodsRealPrice"] = dateSet.GoodsRealPrice;
            dr["GoodsName"] = dateSet.GoodsName;
            dr["CarGrossWeight"] = dateSet.CarGrossWeight;
            dr["CarNetWeight"] = dateSet.CarNetWeight;
            dr["CustomerName"] = dateSet.CustomerName;
            dr["CustomerBalance"] = dateSet.CustomerBalance;
            dr["OrderMoney"] = dateSet.OrderMoney;
            dr["DiscountMoney"] = dateSet.DiscountMoney;
            dr["RealMoney"] = dateSet.RealMoney;
            dr["Status"] = dateSet.Status;
            dr["EmptyCar"] = dateSet.EmptyCar;
            dr["Hedge"] = dateSet.Hedge;
            dr["Type"] = dateSet.Type;
            dr["CustomerType"] = dateSet.CustomerType;
            dr["AdvanceWeight"] = dateSet.AdvanceWeight;
            dr["EnterPonderation"] = dateSet.EnterPonderation;
            dr["ExitPonderation"] = dateSet.ExitPonderation;
            dr["EditTime"] = dateSet.EditTime;
            dr["EditUser"] = dateSet.EditUser;
            dr["EditReason"] = dateSet.EditReason;
            dr["EditNote"] = dateSet.EditNote;
            dr["EnterTime"] = dateSet.EnterTime;
            dr["ExitTime"] = dateSet.ExitTime;
            dr["EnterUser"] = dateSet.EnterUser;
            dr["ExitUser"] = dateSet.ExitUser;
            dr["Note"] = dateSet.Note;
            dt.Rows.Add(dr);

            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = "DataSet1",
                Value = dt
            };

            LocalReport report = new LocalReport
            {
                ReportPath = BConfig.TemplatePath
            };
            report.DataSources.Add(reportDataSource);
            /*
            try
            {
                var a = report.GetParameters();
            Console.WriteLine(a);
            }catch(Exception er)
            {
            }
            */
            width = System.Convert.ToDouble(report.GetParameters()["Width"].Values[0]);
            height = System.Convert.ToDouble(report.GetParameters()["Height"].Values[0]);
            PreviewReportViewer.LocalReport.ReportPath = BConfig.TemplatePath;
            PreviewReportViewer.LocalReport.DataSources.Add(reportDataSource);

            ReportParameter title = new ReportParameter("Title", Config.COMPANY_NAME + BConfig.TitleSuffix);
            ReportParameter footer = new ReportParameter("Footer", BConfig.FooterWord);
            PreviewReportViewer.LocalReport.SetParameters(new ReportParameter[] { title, footer });
            PreviewReportViewer.RefreshReport();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
