using biubiu.Domain;
using biubiu.model;
using biubiu.model.bill;
using biubiu.model.stock_order;
using biubiu.view_model.stock_bill;
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

namespace biubiu.views.marketing.stock_bill
{
    /// <summary>
    /// Interaction logic for StockBillPage.xaml
    /// </summary>
    public partial class StockBillPage : UserControl
    {
        public StockBillPage()
        {
            InitializeComponent();
            DataContext = new StockBillViewModel();
            ((StockBillViewModel)DataContext)._reportViewer = BillReportViewer;
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

        /// <summary>
        /// 自动生成行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BillsDataGrid.SelectedItem != null)
            {
                var bill = BillsDataGrid.SelectedItem as BillModel;
                (DataContext as StockBillViewModel).GetStockOrdersByBillID(bill.ID);
                //ShowBillReport(bill);
                Task.Run(() =>
                {
                    var detail = ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetStockBillsByIDAsync,
                       new
                       {
                           ID = bill.ID
                       }).Result.Data;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ShowBillReport(detail);
                    });
                });
            }
        }

        public void ShowBillReport(BillModel data)
        {
            m_streams = new List<Stream>();

            var customerDataSource = new ReportDataSource
            {
                Name = "CustomerDataSet",
                Value = data.Customer
            };
            var retailDataSource = new ReportDataSource
            {
                Name = "RetailDataSet",
                Value = data.Retail
            };
            var cusDataSource = new ReportDataSource
            {
                Name = "CusDataSet",
                Value = data.CusTotal
            };
            LocalReport report = new LocalReport
            {
                ReportPath = Config.REFER_SHIP_BILL
            };

            ReportParameter overallCountParameter = new ReportParameter("OverallCount", data.Count.ToString());
            ReportParameter overallWeightParameter = new ReportParameter("OverallWeight", data.Weight.ToString());
            ReportParameter overallMoneyParameter = new ReportParameter("OverallMoney", data.Money.ToString());
            ReportParameter referNicknameParameter = new ReportParameter("ReferNickname", data.CreateUserName);
            ReportParameter referDatetimeParameter = new ReportParameter("ReferDatetime", Common.TimeStamp2DateTime(data.CreateTime ?? 0).ToString());

            // 清除数据源
            report.DataSources.Clear();
            // 报表控件添加数据源
            report.DataSources.Add(retailDataSource);
            report.DataSources.Add(customerDataSource);
            report.DataSources.Add(cusDataSource);
            // 清除数据源
            BillReportViewer.LocalReport.DataSources.Clear();
            // 报表控件添加数据源
            BillReportViewer.LocalReport.ReportPath = Config.REFER_STOCK_BILL;
            BillReportViewer.LocalReport.DataSources.Add(retailDataSource);
            BillReportViewer.LocalReport.DataSources.Add(customerDataSource);
            BillReportViewer.LocalReport.DataSources.Add(cusDataSource);
            BillReportViewer.LocalReport.SetParameters(new ReportParameter[] { overallCountParameter, overallWeightParameter, overallMoneyParameter, referNicknameParameter, referDatetimeParameter });
            BillReportViewer.RefreshReport();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as StockBillViewModel;
            datacontext.GetBills();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var datacontext = DataContext as StockBillViewModel;
            if (e.OriginalSource is ScrollViewer sv)
            {
                if (Common.IsVerticalScrollBarAtButtom(sv))
                {
                    datacontext.NextPageCustomerItems(sender as DataGrid);
                }
            }
        }

        private void DataGrid_LoadingRow_1(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;
            StockOrder dataRow = e.Row.Item as StockOrder;
            if (dataRow.Hedge == 1)
            {
                dataGridRow.Foreground = Brushes.Red;
            }
        }
    }
}
