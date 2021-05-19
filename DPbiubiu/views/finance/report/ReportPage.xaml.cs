using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.bill;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.views.finance.report
{
    /// <summary>
    /// ReportPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReportPage : Page
    {
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

        public ReportPage()
        {
            InitializeComponent();
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if(StartDate.SelectedDate == null || StartTime.SelectedTime == null)
            {
                BiuMessageBoxWindows.BiuShow("开始日期或时间不可为空！");
                return;
            }

            if(EndDate.SelectedDate != null && EndTime.SelectedTime == null)
            {
                BiuMessageBoxWindows.BiuShow("结束时间不可为空!");
                return;
            }

            long startStamp = Common.DateTime2TimeStamp(StartDate.SelectedDate.Value.Date + StartTime.SelectedTime.Value.TimeOfDay) ?? 0;
            long? endStamp = null;
            if(EndDate.SelectedDate !=null)
                endStamp = Common.DateTime2TimeStamp(EndDate.SelectedDate.Value.Date + EndTime.SelectedTime.Value.TimeOfDay) ?? 0;

            try
            {
                if (TypeComboBox.SelectedIndex == 0)
                {
                    // 出料
                    var r = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetShipReportAsync,
                        new { StartTime = startStamp, EndTime = endStamp});
                    if (r.Code == 200)
                    {
                        path = Config.SHIP_ORDER_REPORT;
                        InitShipDisplay(r);
                    }
                }
                else
                {
                    // 进料
                    var r = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetStockReprotAsync,
                        new { StartTime = startStamp, EndTime = endStamp });
                    if (r.Code == 200)
                    {
                        path = Config.STOCK_ORDER_REPORT;
                        InitShipDisplay(r);
                    }
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
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

            report.DataSources.Clear();
            BillReportViewer.LocalReport.DataSources.Clear();

            var overallCount = 0;
            var overallWeight = 0.0;
            var overallMoney = 0.0;

            r.Data.Customer.ForEach(x =>
            {
                overallCount += x.Count;
                overallWeight += x.Weight;
                overallMoney += x.Money;
            });
            r.Data.Retail.ForEach(x =>
            {
                overallCount += x.Count;
                overallWeight += x.Weight;
                overallMoney += x.Money;
            });

            ReportParameter overallCountParameter = new ReportParameter("OverallCount", overallCount.ToString());
            ReportParameter overallWeightParameter = new ReportParameter("OverallWeight", overallWeight.ToString());
            ReportParameter overallMoneyParameter = new ReportParameter("OverallMoney", overallMoney.ToString());
            ReportParameter referNicknameParameter = new ReportParameter("ReferNickname", Config.CURRENT_USER.NickName);
            ReportParameter StartDatetimeParameter = new ReportParameter("StartDatetime", (StartDate.SelectedDate.Value.Date + StartTime.SelectedTime.Value.TimeOfDay).ToString());
            ReportParameter EndDatetimeParameter = new ReportParameter("EndDatetime", EndDate.SelectedDate == null ?DateTime.Now.ToString():(EndDate.SelectedDate.Value.Date + EndTime.SelectedTime.Value.TimeOfDay).ToString());

            report.DataSources.Add(retailDataSource);
            report.DataSources.Add(customerDataSource);
            report.DataSources.Add(cusDataSource);
            BillReportViewer.LocalReport.ReportPath = path;
            BillReportViewer.LocalReport.DataSources.Add(retailDataSource);
            BillReportViewer.LocalReport.DataSources.Add(customerDataSource);
            BillReportViewer.LocalReport.DataSources.Add(cusDataSource);
            BillReportViewer.LocalReport.SetParameters(new ReportParameter[] { overallCountParameter, overallWeightParameter, overallMoneyParameter, referNicknameParameter, StartDatetimeParameter, EndDatetimeParameter });
            BillReportViewer.RefreshReport();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reset_Control();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Reset_Control();
        }

        /// <summary>
        /// 重置控件
        /// </summary>
        private void Reset_Control()
        {
            TypeComboBox.SelectedIndex = 0;
            StartDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndDate.SelectedDate = null;
            EndTime.SelectedTime = null;
            BillReportViewer.Clear();
        }
    }
}
