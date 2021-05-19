using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.bill;
using biubiu.model.stock_order;
using biubiu.view_model.stock_order;
using biubiu.view_model.stock_order_manage;
using biubiu.views.marketing.stock_order;
using biubiu.views.marketing.stock_order_manage;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_bill
{
    public class StockBillViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel { Page = 1, Size = 20 };
        public PageModel CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPageOrder = new PageModel { Page = 1, Size = Config.PageSize };
        public PageModel CurrentPageOrder
        {
            get { return _currentPageOrder; }
            set
            {
                _currentPageOrder = value;
                NotifyPropertyChanged("CurrentPageOrder");
            }
        }

        /// <summary>
        /// 当前选中的交账单
        /// </summary>
        private BillModel _selectedBill;
        public BillModel SelectedBill
        {
            get { return _selectedBill; }
            set
            {
                _selectedBill = value;
                NotifyPropertyChanged("SelectedBill");
            }
        }

        /// <summary>
        /// 跳转到页面
        /// </summary>
        private int _jumpNum = 1;
        public int JumpNum
        {
            get { return _jumpNum; }
            set
            {
                _jumpNum = value;
                NotifyPropertyChanged("JumpNum");
            }
        }

        private GetAllDataStatus _requestStatus = new GetAllDataStatus();
        public GetAllDataStatus RequestStatus
        {
            get { return _requestStatus; }
            set
            {
                _requestStatus = value;
                NotifyPropertyChanged("RequestStatus");
            }
        }

        /// <summary>
        /// 交账单集合
        /// </summary>
        private ObservableCollection<BillModel> _billsItems;
        public ObservableCollection<BillModel> BillsItems
        {
            get { return _billsItems; }
            set
            {
                _billsItems = value;
                NotifyPropertyChanged("BillsItems");
            }
        }

        /// <summary>
        /// 订单集合
        /// </summary>
        private ObservableCollection<StockOrder> _orderItems;
        public ObservableCollection<StockOrder> OrderItems
        {
            get { return _orderItems; }
            set
            {
                _orderItems = value;
                NotifyPropertyChanged("OrderItems");
            }
        }

        public ReportViewer _reportViewer;

        public StockBillViewModel()
        {
            GetBills();
        }

        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand RunOrderDetailsCommand => new AnotherCommandImplementation(ExecuteRunOrderDetails);
        public ICommand RunEditOrdersForBillCommand => new AnotherCommandImplementation(ExecuteRunEditOrdersForBillCommand);

        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetBills();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetBills();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetBills();
            }
        }
        public async void ExecuteRunOrderDetails(object o)
        {
            var order = o as StockOrder;
            var shipOrderDetail = new StockOrderDetailsViewModel(order);
            var detail = new StockOrderDetailsDialog
            {
                DataContext = shipOrderDetail,
                IsChange = true
            };
            await DialogHost.Show(detail, "RootDialog", ClosingEventHandler);
        }
        public async void ExecuteRunEditOrdersForBillCommand(object o)
        {
            if (o == null) return;
            var view = new EditOrdersDialog
            {
                DataContext = new EditOrdersViewModel(o as BillModel)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPageCustomerItems(DataGrid lv)
        {
            if (SelectedBill == null)
            {
                BiuMessageBoxWindows.BiuShow("未选中交账单！",image:BiuMessageBoxImage.Warning);
                return;
            }
            if (CurrentPageOrder.Last || lv.Items.IsEmpty)
            {
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentPageOrder.Page++;
                var r = new ObservableCollection<StockOrder>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetStockOrderByBillIDAsync,
                new { Page = CurrentPageOrder.Page - 1, Size = CurrentPageOrder.Size, ID = SelectedBill.ID },
                delegate (DataInfo<List<StockOrder>> result) { CurrentPageOrder = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        OrderItems.Add(item);
                    });
                }
            });
        }

        public void GetBills()
        {
            BillsItems?.Clear();
            RequestStatus.StartRequest(() =>
            {
                BillsItems = new ObservableCollection<BillModel>(
                    ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetStockBillsAsync,
                    new
                    {
                        Page = CurrentPage.Page - 1,
                        CurrentPage.Size
                    },
                    delegate (DataInfo<List<BillModel>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        public void GetStockOrdersByBillID(string billID)
        {
            OrderItems?.Clear();
            CurrentPageOrder.Page = 1;
            RequestStatus.StartRequest(() =>
            {
                OrderItems = new ObservableCollection<StockOrder>(
                    ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetStockOrderByBillIDAsync,
                    new
                    {
                        Page = CurrentPageOrder.Page - 1,
                        Size = Config.PageSize,
                        ID = billID
                    },
                    delegate (DataInfo<List<StockOrder>> result) { CurrentPageOrder = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //GetStockOrdersByBillID(SelectedBill.ID);
            _reportViewer.Clear();
            OrderItems.Clear();
            GetBills();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "SelectedBill":
                    if (SelectedBill != null)
                        GetStockOrdersByBillID(SelectedBill.ID);
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
