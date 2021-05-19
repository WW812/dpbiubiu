using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.stock_order;
using biubiu.model.user;
using biubiu.view_model.stock_order;
using biubiu.views.marketing.stock_order;
using biubiu.views.marketing.stock_order_manage;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_order_manage
{
    public class StockOrderManageViewModel : INotifyPropertyChanged
    {
        #region 属性
        /// <summary>
        /// 单据集合
        /// </summary>
        private ObservableCollection<StockOrder> _ordersItems;
        public ObservableCollection<StockOrder> OrderItems
        {
            get { return _ordersItems; }
            set
            {
                _ordersItems = value;
                NotifyPropertyChanged("OrderItems");
            }
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        private SearchStockOrder _searchOrder;
        public SearchStockOrder SearchOrder
        {
            get { return _searchOrder; }
            set
            {
                _searchOrder = value;
                NotifyPropertyChanged("SearchOrder");
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel { Page = 1, Size = Config.PageSize };
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
        /// 查询按钮是否可用
        /// </summary>
        private Boolean _buttonIsEnabled = true;
        public Boolean ButtonIsEnabled
        {
            get { return _buttonIsEnabled; }
            set
            {
                _buttonIsEnabled = value;
                NotifyPropertyChanged("ButtonIsEnabled");
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


        private DateTime? _exportOrderStartDate = null;
        public DateTime? ExportOrderStartDate
        {
            get { return _exportOrderStartDate; }
            set
            {
                _exportOrderStartDate = value;
                NotifyPropertyChanged("ExportOrderStartDate");
            }
        }

        private DateTime _exportOrderStartTime = new DateTime();
        public DateTime ExportOrderStartTime
        {
            get { return _exportOrderStartTime; }
            set
            {
                _exportOrderStartTime = value;
                NotifyPropertyChanged("ExportOrderStartTime");
            }
        }

        private DateTime? _exportOrderEndDate = null;
        public DateTime? ExportOrderEndDate
        {
            get { return _exportOrderEndDate; }
            set
            {
                _exportOrderEndDate = value;
                NotifyPropertyChanged("ExportOrderEndDate");
            }
        }

        private DateTime _exportOrderEndTime = new DateTime();
        public DateTime ExportOrderEndTime
        {
            get { return _exportOrderEndTime; }
            set
            {
                _exportOrderEndTime = value;
                NotifyPropertyChanged("ExportOrderEndTime");
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
        #endregion

        public StockOrderManageViewModel()
        {
            SearchOrder = new SearchStockOrder();
            GetOrders(SearchOrder);
        }

        #region 命令
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExcuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExcutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExcuteNextPageCommand);
        public ICommand SearchCommand => new AnotherCommandImplementation(ExcuteSearchCommand);
        public ICommand ResetSearchOrderCommand => new AnotherCommandImplementation(ExcuteResetSearchOrderCommand);
        public ICommand RunOrderDetailsDialogCommand => new AnotherCommandImplementation(ExcuteRunOrderDetailsDialogCommand);
        public ICommand RunPrecisionSearchStockOrderDialogCommand => new AnotherCommandImplementation(ExcuteRunPrecisionSearchStockOrderDialogCommand);
        public ICommand ExportOrderCommand => new AnotherCommandImplementation(ExecuteExportOrderCommand);

        private void ExcuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetOrders(SearchOrder);
        }
        private void ExcutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetOrders(SearchOrder);
            }
        }
        private void ExcuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetOrders(SearchOrder);
            }
        }
        private void ExcuteSearchCommand(object o)
        {
            GetOrders(SearchOrder);
        }
        private void ExcuteResetSearchOrderCommand(object o)
        {
            SearchOrder.Reset();
            GetOrders(SearchOrder);
        }
        private async void ExcuteRunOrderDetailsDialogCommand(object o)
        {
            var order = o as StockOrder;
            var stockOrderDetail = new StockOrderDetailsViewModel(order);
            var detail = new StockOrderDetailsDialog
            {
                DataContext = stockOrderDetail,
                IsChange = true
            };

            var result = await DialogHost.Show(detail, "RootDialog", ClosingEventHandler);
        }
        private async void ExcuteRunPrecisionSearchStockOrderDialogCommand(object o)
        {
            var view = new PrecisionSearchStockOrderDialog
            {
                DataContext = new PrecisionSearchStockOrderViewModel(SearchOrder)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }
        private void ExecuteExportOrderCommand(object o)
        {
            long? startStamp = null;
            long? endStamp = null;
            if (ExportOrderStartDate != null)
                startStamp = Common.DateTime2TimeStamp(ExportOrderStartDate.Value.Date + ExportOrderStartTime.TimeOfDay);
            if (ExportOrderEndDate != null)
                endStamp = Common.DateTime2TimeStamp(ExportOrderEndDate.Value.Date + ExportOrderEndTime.TimeOfDay + TimeSpan.FromMinutes(1) - TimeSpan.FromMilliseconds(1));

            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog
            {
                Title = "选择要保存的文件路径",
                Filter = "*.xls|",
                FileName = DateTime.Now.ToString("yyyy-MM-dd HH：mm") + " 导出单据.xls",
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RequestStatus.StartRequest(() =>
                {
                    try
                    {
                        var fs = ApiClient.ExportStockOrder(new
                        {
                            startTime = startStamp,
                            endTime = endStamp
                        }).GetAwaiter().GetResult();

                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);

                        // 设置当前流的位置为流的开始
                        fs.Seek(0, SeekOrigin.Begin);
                        using (FileStream fsWrite = new FileStream(ofd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            BinaryWriter bw = new BinaryWriter(fsWrite);
                            bw.Write(bytes);
                            bw.Close();
                        }
                        fs.Close();

                        BiuMessageBoxWindows.BiuShow("导出成功!");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow(er.Message);
                    }
                });
            }
        }
        #endregion


        public void GetOrders(SearchStockOrder stockOrder)
        {
            stockOrder.Page = CurrentPage.Page - 1;
            stockOrder.Size = Config.PageSize;
            Task.Run(() =>
            {
                OrderItems = new ObservableCollection<StockOrder>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        new ApiDelegateArg<List<StockOrder>>(ApiClient.GetAllStockOrdersAsync),
                        stockOrder,
                        delegate (DataInfo<List<StockOrder>> result) { CurrentPage = result.Page; }
                        ).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            CurrentPage.Page = 1;
            GetOrders(SearchOrder);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 单据管理 搜索单据条件类
    /// </summary>
    public class SearchStockOrder : INotifyPropertyChanged
    {
        //订单号
        private string _orderNo;
        public string OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                _orderNo = value;
                NotifyPropertyChanged("OrderNo");
            }
        }

        //车号
        private string _carId;
        public string CarId
        {
            get
            {
                return _carId;
            }
            set
            {
                _carId = value;
                NotifyPropertyChanged("CarId");
            }
        }

        //出厂时间
        private long? _exitTime = null;
        public long? ExitTime
        {
            get
            {
                return _exitTime;
            }
            set
            {
                _exitTime = value;
                NotifyPropertyChanged("ExitTime");
            }
        }

        //出厂时间结束
        private long? _exitTimeEnd = null;
        public long? ExitTimeEnd
        {
            get
            {
                return _exitTimeEnd;
            }
            set
            {
                _exitTimeEnd = value;
                NotifyPropertyChanged("ExitTimeEnd");
            }
        }

        //进厂司磅员
        private User _enterUser = null;
        public User EnterUser
        {
            get
            {
                return _enterUser;
            }
            set
            {
                _enterUser = value;
                NotifyPropertyChanged("EnterUser");
            }
        }
        //出厂司磅员
        private User _exitUser = null;
        public User ExitUser
        {
            get { return _exitUser; }
            set
            {
                _exitUser = value;
                NotifyPropertyChanged("ExitUser");
            }
        }

        //客户
        private StockCustomer _customer = null;
        public StockCustomer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        //料品
        private StockGoods _goods = null;
        public StockGoods Goods
        {
            get
            {
                return _goods;
            }
            set
            {
                _goods = value;
                NotifyPropertyChanged("Goods");
            }
        }


        public int Page { get; set; }
        public int Size { get; set; }

        public void Reset()
        {
            OrderNo = null;
            CarId = null;
            ExitTime = null;
            ExitTimeEnd = null;
            EnterUser = null;
            ExitUser = null;
            Customer = null;
            Goods = null;
            Page = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
