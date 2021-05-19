using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.stock_order;
using biubiu.views.marketing.customer.stock_customer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.customer.stock_customer
{
    public class StockCustomerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 出料客户集合
        /// </summary>
        private ObservableCollection<StockCustomer> _stockCustomerItems;
        public ObservableCollection<StockCustomer> StockCustomerItems
        {
            get
            {
                return _stockCustomerItems;
            }
            set
            {
                _stockCustomerItems = value;
                NotifyPropertyChanged("StockCustomerItems");
            }
        }

        /// <summary>
        /// 客户单价集合
        /// </summary>
        private ObservableCollection<StockCustomerGoodsPrice> _stockCustomerGoodsPriceItems;
        public ObservableCollection<StockCustomerGoodsPrice> StockCustomerGoodsPriceItems
        {
            get { return _stockCustomerGoodsPriceItems; }
            set
            {
                _stockCustomerGoodsPriceItems = value;
                NotifyPropertyChanged("StockCustomerGoodsPriceItems");
            }
        }

        /// <summary>
        /// 客户车辆集合
        /// </summary>
        private ObservableCollection<StockCustomerCar> _stockCustomerCarItems;
        public ObservableCollection<StockCustomerCar> StockCustomerCarItems
        {
            get { return _stockCustomerCarItems; }
            set
            {
                _stockCustomerCarItems = value;
                NotifyPropertyChanged("StockCustomerCarItems");
            }
        }

        /// <summary>
        /// 单据集合
        /// </summary>
        private ObservableCollection<StockOrder> _stockCustomerOrderItems;
        public ObservableCollection<StockOrder> StockCustomerOrderItems
        {
            get
            {
                return _stockCustomerOrderItems;
            }
            set
            {
                _stockCustomerOrderItems = value;
                NotifyPropertyChanged("StockCustomerOrderItems");
            }
        }

        /// <summary>
        /// 当前选中的客户
        /// </summary>
        private StockCustomer _currentStockCustomer;
        public StockCustomer CurrentStockCustomer
        {
            get
            {
                return _currentStockCustomer;
            }
            set
            {
                _currentStockCustomer = value;
                NotifyPropertyChanged("CurrentStockCustomer");
            }
        }

        /// <summary>
        /// 当前选中的客户单价
        /// </summary>
        private StockCustomerGoodsPrice _currentStockCustomerGoodsPrice;
        public StockCustomerGoodsPrice CurrentStockCustomerGoodsPrice
        {
            get { return _currentStockCustomerGoodsPrice; }
            set
            {
                _currentStockCustomerGoodsPrice = value;
                NotifyPropertyChanged("CurrentStockCustomerGoodsPrice");
            }
        }

        /// <summary>
        /// 修改中的客户单价
        /// </summary>
        private StockCustomerGoodsPrice _editStockCustomerGoodsPrice;
        public StockCustomerGoodsPrice EditStockCustomerGoodsPrice
        {
            get { return _editStockCustomerGoodsPrice; }
            set
            {
                _editStockCustomerGoodsPrice = value;
                NotifyPropertyChanged("EditStockCustomerGoodsPrice");
            }
        }

        /// <summary>
        /// 修改中的客户车辆
        /// </summary>
        private StockCustomerCar _editStockCustomerCar;
        public StockCustomerCar EditStockCustomerCar
        {
            get { return _editStockCustomerCar; }
            set
            {
                _editStockCustomerCar = value;
                NotifyPropertyChanged("EditStockCustomerCar");
            }
        }

        /// <summary>
        /// tabpanel SelectedIndex
        /// 0 单价 1车辆 2单据
        /// </summary>
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyPropertyChanged("SelectedIndex");
            }
        }

        /// <summary>
        /// 单据selected 0全部 1待支付 2已支付
        /// </summary>
        private int _selectedIndexOrder = 0;
        public int SelectedIndexOrder
        {
            get { return _selectedIndexOrder; }
            set
            {
                _selectedIndexOrder = value;
                NotifyPropertyChanged("SelectedIndexOrder");
            }
        }

        /// <summary>
        /// 判断当前接口是否走完，大于0还有数据在加载
        /// </summary>
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
        /// 底部弹窗
        /// </summary>
        /*
        private SnackbarViewModel _messageBar = new SnackbarViewModel();
        public SnackbarViewModel MessageBar
        {
            get { return _messageBar; }
            set
            {
                _messageBar = value;
                NotifyPropertyChanged("MessageBar");
            }
        }
        */

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel(16);
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
        private PageModel _currentCustomerPage;// = new PageModel();
        public PageModel CurrentCustomerPage
        {
            get { return _currentCustomerPage; }
            set
            {
                _currentCustomerPage = value;
                NotifyPropertyChanged("CurrentCustomerPage");
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

        /// <summary>
        /// 导出客户详单开始日期
        /// </summary>
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

        /// <summary>
        /// 导出客户详单开始时间
        /// </summary>
        private DateTime? _exportOrderStartTime = null;
        public DateTime? ExportOrderStartTime
        {
            get { return _exportOrderStartTime; }
            set
            {
                _exportOrderStartTime = value;
                NotifyPropertyChanged("ExportOrderStartTime");
            }
        }

        /// <summary>
        /// 导出客户详单结束日期
        /// </summary>
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

        /// <summary>
        /// 导出客户详单结束时间
        /// </summary>
        private DateTime? _exportOrderEndTime = null;
        public DateTime? ExportOrderEndTime
        {
            get { return _exportOrderEndTime; }
            set
            {
                _exportOrderEndTime = value;
                NotifyPropertyChanged("ExportOrderEndTime");
            }
        }

        /// <summary>
        /// 搜索客户输入框输入字段
        /// </summary>
        private string _searchCusSeed = null;
        public string SearchCusSeed
        {
            get { return _searchCusSeed; }
            set
            {
                _searchCusSeed = value;
                NotifyPropertyChanged("SearchCusSeed");
            }
        }

        /// <summary>
        /// 是不是删除客户
        /// </summary>
        private bool _deletedCustomer = false;
        public bool DeletedCustomer
        {
            get
            {
                return _deletedCustomer;
            }

            set
            {
                _deletedCustomer = value;
                NotifyPropertyChanged("DeletedCustomer");
            }
        }

        public StockCustomerViewModel()
        {
            EditStockCustomerCar = new StockCustomerCar();
        }

        public ICommand RunCreateStockCustomerDialogCommand => new AnotherCommandImplementation(ExecuteRunCreateShipCustomerDialog);
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand SubmitGoodsPriceCommand => new AnotherCommandImplementation(ExecuteSubmitGoodsPriceCommand);
        public ICommand SubmitCarCommand => new AnotherCommandImplementation(ExecuteSubmitCarCommand);
        public ICommand DeleteCarCommand => new AnotherCommandImplementation(ExecuteDeleteCarCommand);
        public ICommand RunChangeStockCustomerCommand => new AnotherCommandImplementation(ExecuteRunChangeStockCustomerCommand);
        public ICommand ExportCusOrderDetailReportCommand => new AnotherCommandImplementation(ExecuteExportCusOrderDetailReportCommand);
        public ICommand DeleteCustomerCommand => new AnotherCommandImplementation(ExecuteDeleteCustomerCommand);

        /// <summary>
        /// 开启新增客户Dialog(出料)
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunCreateShipCustomerDialog(object o)
        ///
        {
            var view = new CreateStockCustomerDialog();
            var result = await DialogHost.Show(view, "RootDialog", ClosingCustomerEventHandler);
        }
        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetDataByCustomer();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetDataByCustomer();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetDataByCustomer();
            }
        }
        private void ExecuteSubmitGoodsPriceCommand(object o)
        {
            if (CurrentStockCustomer == null || EditStockCustomerGoodsPrice == null) return;
            EditStockCustomerGoodsPrice.CustomerId = CurrentStockCustomer.ID;
            RequestStatus.StartRequest(() =>
            {
                /*
                try
                {
                    var Result = ApiClient.SetStockCustomerPriceAsync(EditStockCustomerGoodsPrice).GetAwaiter().GetResult();
                    if (Result.Code != 200)
                    {
                        throw new Exception(Result.ToString());
                    }
                    else
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
                    }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                }
                */
                var r = ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.SetStockCustomerPriceAsync,
                    EditStockCustomerGoodsPrice,
                    delegate(DataInfo<object> result) {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
                    }).Result;
                GetStockCustomerGoodsPriceItems();
            });
        }
        private void ExecuteSubmitCarCommand(object o)
        {
            RequestStatus.StartRequest(async () =>
            {
                if (CurrentStockCustomer == null)
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                    return;
                }
                if (EditStockCustomerCar.CarId.Equals(""))
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("请输入车牌号!");
                    return;
                }
                EditStockCustomerCar.Customer = CurrentStockCustomer;
                await ModelHelper.GetInstance().GetApiDataArg(ApiClient.CreateStockCustomerCarAsync,
                     EditStockCustomerCar,
                     delegate (DataInfo<StockCustomerCar> result)
                     {
                         EditStockCustomerCar = new StockCustomerCar();
                         SnackbarViewModel.GetInstance().PoupMessageAsync("添加成功!");
                     });
                GetStockCustomerCarItems();
            });
        }
        private void ExecuteDeleteCarCommand(object o)
        {
            if (BiuMessageBoxResult.Cancel.Equals(BiuMessageBoxWindows.BiuShow("确定删除该车辆?",BiuMessageBoxButton.OKCancel, BiuMessageBoxImage.Question))) return;
            RequestStatus.StartRequest(async () =>
            {
                await ModelHelper.GetInstance().GetApiDataArg(ApiClient.DeleteStockCustomerCarAsync,
                    (o as StockCustomerCar),
                    delegate (DataInfo<StockCustomerCar> result)
                    {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("删除成功!");
                    });
                GetStockCustomerCarItems();
            });
        }
        private async void ExecuteRunChangeStockCustomerCommand(object o)
        {
            if (CurrentStockCustomer == null)
            { SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!"); return; }
            var view = new StockCustomerInformationDialog
            {
                DataContext = new StockCustomerInformationViewModel(CurrentStockCustomer)
            };
            await DialogHost.Show(view, "RootDialog", ClosingChangeCustomerEventHandler);
        }
        private void ExecuteExportCusOrderDetailReportCommand(object o)
        {
        }
        private async void ExecuteDeleteCustomerCommand(object o)
        {
            var c = o as StockCustomer;
            if (o == null || DeletedCustomer) return;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确定删除客户: " + c.Name + "?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question)))
            {
                await ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.DeleteStockCustomerAsync,
                    new { ID = c.ID });
                GetStockCustomerItems();
                GetDataByCustomer();
            }
        }

        /// <summary>
        /// 获取进料客户
        /// </summary>
        public void GetStockCustomerItems()
        {
            CurrentCustomerPage = new PageModel { Page = 1, Size=25 };
            StockCustomerItems?.Clear();
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                StockCustomerItems = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetStockCustomerAsync,
                   new { szm = SearchCusSeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size, Deleted = DeletedCustomer},
                   delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(DataGrid lv)
        {
            if (CurrentCustomerPage.Last)
            {
                Task.Run(()=> {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                });
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetStockCustomerAsync,
                new {Szm=SearchCusSeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size, Deleted = DeletedCustomer },
                delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate() {
                        StockCustomerItems.Add(item);
                    });
                }
            });
        }

        /// <summary>
        /// 获取单价
        /// </summary>
        private void GetStockCustomerGoodsPriceItems()
        {
            if (CurrentStockCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            }
            CurrentStockCustomer.Page = CurrentPage.Page - 1;
            RequestStatus.StartRequest(() =>
            {
                StockCustomerGoodsPriceItems = new ObservableCollection<StockCustomerGoodsPrice>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetStockCustomerGoodsPriceAsync,
                        new { customerId = CurrentStockCustomer.ID ,Page = CurrentPage.Page - 1,Size = CurrentPage.Size},
                        delegate (DataInfo<List<StockCustomerGoodsPrice>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 获取车辆
        /// </summary>
        private void GetStockCustomerCarItems()
        {
            if (CurrentStockCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            }
            CurrentStockCustomer.Page = CurrentPage.Page - 1;
            RequestStatus.StartRequest(() =>
            {
                StockCustomerCarItems = new ObservableCollection<StockCustomerCar>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetStockCustomerCarAsync, new { Customer = CurrentStockCustomer, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                        delegate (DataInfo<List<StockCustomerCar>> result) { CurrentPage = result.Page; }
                        ).Result.Data);
            });
        }

        /// <summary>
        /// 获取单据
        /// </summary>
        private void GetStockCustomerOrderItems()
        {
            if (CurrentStockCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            }
            CurrentStockCustomer.Page = CurrentPage.Page - 1;
            var order = new StockOrder { Customer = CurrentStockCustomer , Page = CurrentPage.Page-1, Size = CurrentPage.Size };
            if (SelectedIndexOrder - 1 >= 0) order.Paid = SelectedIndexOrder - 1;
            RequestStatus.StartRequest(() =>
            {
                StockCustomerOrderItems = new ObservableCollection<StockOrder>(
                  ModelHelper.GetInstance().GetApiDataArg(
                      ApiClient.GetStockOrderByStockCustomerAsync, order,
                      delegate (DataInfo<List<StockOrder>> result) { CurrentPage = result.Page; }
                      ).Result.Data);
            });
        }

        /// <summary>
        /// 获取数据根据当前选中客户
        /// </summary>
        public void GetDataByCustomer()
        {
            if (CurrentStockCustomer == null)
            {
                StockCustomerGoodsPriceItems?.Clear();
                StockCustomerCarItems?.Clear();
                StockCustomerOrderItems?.Clear();
            }
            else
            {
                switch (SelectedIndex)
                {
                    case 0:
                        GetStockCustomerGoodsPriceItems();
                        break;
                    case 1:
                        GetStockCustomerCarItems();
                        break;
                    case 2:
                        GetStockCustomerOrderItems();
                        break;
                }
            }
        }

        /// <summary>
        /// 创建客户界面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingCustomerEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetStockCustomerItems();
        }

        /// <summary>
        /// 修改客户信息 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingChangeCustomerEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetStockCustomerItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "SelectedIndex":
                case "CurrentStockCustomer":
                    if (CurrentPage == null) CurrentPage = new PageModel(16); else CurrentPage.Reset(16);
                    GetDataByCustomer();
                    EditStockCustomerGoodsPrice = null;
                    break;
                case "CurrentStockCustomerGoodsPrice":
                    if (CurrentStockCustomerGoodsPrice == null) return;
                    EditStockCustomerGoodsPrice = Common.DeepCopy(CurrentStockCustomerGoodsPrice);
                    break;
                case "SelectedIndexOrder":
                    if (CurrentPage == null) CurrentPage = new PageModel(16); else CurrentPage.Reset(16);
                    GetDataByCustomer();
                    break;
                case "DeletedCustomer":
                    GetStockCustomerItems();
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}