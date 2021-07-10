using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.bill;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.ship_order
{
    /// <summary>
    /// ReferBillWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReferBillWindow : Window
    {
        // BillType 0 为出料交账 1为进料交账
        public int BillType;
        public bool IsClose = false;
        public string path;
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

        /// <summary>
        /// billType 0 为出料交账 1为进料交账
        /// </summary>
        /// <param name="billType"></param>
        public ReferBillWindow(int billType)
        {
            InitializeComponent();
            try
            {
                BillType = billType;
                if (BillType == 0)
                {
                    //出料
                    var r = ApiClient.GetReferShipBillDetails().GetAwaiter().GetResult();
                    path = Config.REFER_SHIP_BILL;
                    InitShipDisplay(r);
                }
                else
                {
                    //进料
                    var r = ApiClient.GetReferStockBillDetails().GetAwaiter().GetResult();
                    path = Config.REFER_STOCK_BILL;
                    InitShipDisplay(r);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        public void InitShipDisplay(DataInfo<BillDetailsModel> r)
        {
            m_streams = new List<Stream>();

            var customerDataSource = new ReportDataSource
            {
                Name = "CustomerDataSet",
                Value = r.Data.Customer
            };
            var retailDataSource = new ReportDataSource
            {
                Name = "RetailDataSet",
                Value = r.Data.Retail
            };
            var cusDataSource = new ReportDataSource
            {
                Name = "CusDataSet",
                Value = r.Data.CusTotal
            };
            LocalReport report = new LocalReport
            {
                ReportPath = path
            };
            var overallCount = 0;
            var overallWeight = 0.0;
            var overallMoney = 0.0;
            var overallPlatformMoney = 0.0;
            var overallPlatDiffMoney = 0.0;

            r.Data.Customer.ForEach(x =>
            {
                overallCount += x.Count;
                overallWeight += x.Weight;
                overallMoney += x.Money;
                overallPlatformMoney += x.PlatformMoney;
                overallPlatDiffMoney += x.PlatDiffMoney;
            });
            r.Data.Retail.ForEach(x =>
            {
                overallCount += x.Count;
                overallWeight += x.Weight;
                overallMoney += x.Money;
                overallPlatformMoney += x.PlatformMoney;
                overallPlatDiffMoney += x.PlatDiffMoney;
            });

            ReportParameter overallCountParameter = new ReportParameter("OverallCount", overallCount.ToString());
            ReportParameter overallWeightParameter = new ReportParameter("OverallWeight", overallWeight.ToString());
            ReportParameter overallMoneyParameter = new ReportParameter("OverallMoney", overallMoney.ToString());
            ReportParameter overallPlatformMoneyParameter = new ReportParameter("OverallPlatformMoney", overallPlatformMoney.ToString());
            ReportParameter overallPlatDiffMoneyParameter = new ReportParameter("OverallPlatDiffMoney", overallPlatDiffMoney.ToString());
            ReportParameter referNicknameParameter = new ReportParameter("ReferNickname", Config.CURRENT_USER.NickName);
            ReportParameter referDatetimeParameter = new ReportParameter("ReferDatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            report.DataSources.Add(retailDataSource);
            report.DataSources.Add(customerDataSource);
            report.DataSources.Add(cusDataSource);
            BillReportViewer.LocalReport.ReportPath = path;
            BillReportViewer.LocalReport.DataSources.Add(retailDataSource);
            BillReportViewer.LocalReport.DataSources.Add(customerDataSource);
            BillReportViewer.LocalReport.DataSources.Add(cusDataSource);
            BillReportViewer.LocalReport.SetParameters(new ReportParameter[] { overallCountParameter, overallWeightParameter, overallMoneyParameter, referNicknameParameter, referDatetimeParameter, overallPlatformMoneyParameter, overallPlatDiffMoneyParameter });
            BillReportViewer.RefreshReport();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IsClose = true;
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            DataInfo<Object> result;
            if (BiuMessageBoxResult.Cancel.Equals(BiuMessageBoxWindows.BiuShow("确认交账?", BiuMessageBoxButton.OKCancel, BiuMessageBoxImage.Question))) return;
            try
            {
                if (BillType == 0)
                {
                    result = ApiClient.ReferShipBill().GetAwaiter().GetResult();

                }
                else
                {
                    result = ApiClient.ReferStockBill().GetAwaiter().GetResult();
                }

                if (result.Code == 200)
                {
                    Close();
                }
                else
                {
                    throw new Exception(result.ToString());
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        public void IShow(Window owner = null)
        {
            Owner = owner ?? Application.Current.MainWindow;
            Owner.Opacity = 0.5;
            ShowDialog();
            Owner.Opacity = 1;
        }
    }
}
