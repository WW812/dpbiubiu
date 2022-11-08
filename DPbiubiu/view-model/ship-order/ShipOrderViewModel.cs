using BenchmarkDotNet.Attributes;
using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer;
using biubiu.model.customer.ship_customer;
using biubiu.model.print;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
using biubiu.views.Loading;
using biubiu.views.marketing.ship_order;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Qiniu.Http;
using Qiniu.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WebApiClient;
using wow;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.ship_order
{
    public class ShipOrderViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 料品
        /// </summary>
        private ObservableCollection<ShipGoods> _goodsItems;
        public ObservableCollection<ShipGoods> GoodsItems
        {
            get
            {
                return _goodsItems;
            }
            set
            {
                _goodsItems = value;
                NotifyPropertyChanged("GoodsItems");
            }
        }
        /// <summary>
        /// 单据
        /// </summary>
        private ShipOrder _order = new ShipOrder();
        public ShipOrder Order
        {
            get { return _order; }
            set
            {
                _order = value;
                NotifyPropertyChanged("Order");
            }
        }
        /// <summary>
        /// 当前选中的进厂单据
        /// </summary>
        private ShipOrder _currentEnterOrder;
        public ShipOrder CurrentEnterOrder
        {
            get { return _currentEnterOrder; }
            set
            {
                _currentEnterOrder = value;
                NotifyPropertyChanged("CurrentEnterOrder");
            }
        }
        /*
        /// <summary>
        /// 未结账（进厂单据显示列控制）
        /// </summary>
        public ShipOrderEnterColumnsVisibility EnterColumnsVisibility { get; set; }
        /// <summary>
        /// 已结账（出厂单据显示列控制）
        /// </summary>
        public ShipOrderExitColumnsVisibility ExitColumnsVisibility { get; set; }
        /// <summary>
        /// 空车（空车单据显示列控制）
        /// </summary>
        public ShipOrderEmptyColumnsVisibility EmptyColumnsVisibility { get; set; }
        */
        /// <summary>
        /// 列显示控制
        /// </summary>
        private ShipOrderColumnsVisibility _columnsVisibility;
        public ShipOrderColumnsVisibility ColumnsVisibility
        {
            get
            {
                return _columnsVisibility;
            }
            set
            {
                _columnsVisibility = value;
                NotifyPropertyChanged("ColumnsVisibility");
            }
        }

        /// <summary>
        /// 车牌号使用， 客户车辆集合
        /// </summary>
        private ObservableCollection<ShipCustomerCar> _customerCarItems;
        public ObservableCollection<ShipCustomerCar> CustomerCarItems
        {
            get
            {
                return _customerCarItems;
            }
            set
            {
                _customerCarItems = value;
                NotifyPropertyChanged("CustomerCarItems");
            }
        }

        /// <summary>
        /// 客户列表
        /// </summary>
        private ObservableCollection<ShipCustomer> _customerItems;
        public ObservableCollection<ShipCustomer> CustomerItems
        {
            get
            {
                return _customerItems;
            }
            set
            {
                _customerItems = value;
                NotifyPropertyChanged("CustomerItems");
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
        /// 列表Menu
        /// </summary>
        private bool _orderListBoxIsEnabled = true;
        public bool OrderListBoxIsEnabled
        {
            get { return _orderListBoxIsEnabled; }
            set
            {
                _orderListBoxIsEnabled = value;
                NotifyPropertyChanged("OrderListBoxIsEnabled");
            }
        }

        /// <summary>
        /// 车牌号是否展开 已废弃
        /// </summary>
        /*
        private Boolean _carIdIsDropDownOpen = false;
        public Boolean CarIdIsDropDownOpen
        {
            get { return _carIdIsDropDownOpen; }
            set
            {
                _carIdIsDropDownOpen = value;
                NotifyPropertyChanged("CarIdIsDropDownOpen");
            }
        }
        */

        /// <summary>
        /// 单据集合
        /// </summary>
        private ObservableCollection<ShipOrder> _ordersItems;
        public ObservableCollection<ShipOrder> OrdersItems
        {
            get
            {
                return _ordersItems;
            }
            set
            {
                _ordersItems = value;
                NotifyPropertyChanged("OrdersItems");
            }
        }

        /// <summary>
        /// 一号地磅
        /// </summary>
        private PonderationDisplay _pond1 = new PonderationDisplay("1号地磅");
        public PonderationDisplay Pond1
        {
            get
            {
                return _pond1;
            }

            set
            {
                _pond1 = value;
                NotifyPropertyChanged("Pond1");
            }
        }

        /// <summary>
        /// 二号地磅
        /// </summary>
        private PonderationDisplay _pond2 = new PonderationDisplay("2号地磅");
        public PonderationDisplay Pond2
        {
            get
            {
                return _pond2;
            }

            set
            {
                _pond2 = value;
                NotifyPropertyChanged("Pond2");
            }
        }

        /// <summary>
        /// 三号地磅
        /// </summary>
        private PonderationDisplay _pond3 = new PonderationDisplay("3号地磅");
        public PonderationDisplay Pond3
        {
            get
            {
                return _pond3;
            }

            set
            {
                _pond3 = value;
                NotifyPropertyChanged("Pond3");
            }
        }

        /// <summary>
        /// 四号地磅
        /// </summary>
        private PonderationDisplay _pond4 = new PonderationDisplay("4号地磅");
        public PonderationDisplay Pond4
        {
            get
            {
                return _pond4;
            }

            set
            {
                _pond4 = value;
                NotifyPropertyChanged("Pond4");
            }
        }

        /// <summary>
        /// 地磅列表
        /// </summary>
        private ObservableCollection<PonderationDisplay> _ponderDisplayItems;
        public ObservableCollection<PonderationDisplay> PonderDisplayItems
        {
            get
            {
                return _ponderDisplayItems;
            }
            set
            {
                _ponderDisplayItems = value;
                NotifyPropertyChanged("PonderDisplayItems");
            }
        }

        /// <summary>
        /// 当前选中的地磅
        /// </summary>
        private PonderationDisplay _currentPonderationDisplay;
        public PonderationDisplay CurrentPonderationDisplay
        {
            get
            {
                return _currentPonderationDisplay;
            }
            set
            {
                _currentPonderationDisplay = value;
                NotifyPropertyChanged("CurrentPonderationDisplay");
            }
        }

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
        /// 单据列表选择 作为拉取单子(已结账，未结账)的标识
        /// </summary>
        private int _menuIndex = 0;
        public int MenuIndex
        {
            get { return _menuIndex; }
            set
            {

                _menuIndex = value;
                NotifyPropertyChanged("MenuIndex");
            }
        }

        /// <summary>
        /// 软键盘车牌号
        /// </summary>
        private string _softCarId = "";
        public string SoftCarId
        {
            get { return _softCarId; }
            set
            {
                _softCarId = value;
                NotifyPropertyChanged("SoftCarId");
            }
        }

        /// <summary>
        /// 虚拟键盘前缀替换数量
        /// </summary>
        private int SoftCarIdPre = 1;

        /// <summary>
        /// 虚拟键盘是否显示
        /// </summary>
        private Boolean _softKeyboardIsOpen = false;
        public Boolean SoftKeyboardIsOpen
        {
            get { return _softKeyboardIsOpen; }
            set
            {
                _softKeyboardIsOpen = value;
                NotifyPropertyChanged("SoftKeyboardIsOpen");
            }
        }

        /// <summary>
        /// 根据车牌号搜索单据
        /// </summary>
        private string _carIDSearchFeed = "";
        public string CarIDSearchFeed
        {
            get { return _carIDSearchFeed; }
            set
            {
                _carIDSearchFeed = value;
                NotifyPropertyChanged("CarIDSearchFeed");
            }
        }

        private bool IsClosePond = false;


        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentCustomerPage = new PageModel();
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
        /// 单据统计综述
        /// </summary>
        private string _detailOrderStr;
        public string DetailOrderStr
        {
            get { return _detailOrderStr; }
            set
            {
                _detailOrderStr = value;
                NotifyPropertyChanged("DetailOrderStr");
            }
        }


        //private Snapshot SnapshotPicture = new Snapshot();

        /// <summary>
        /// 摄像头预览窗口
        /// </summary>
        private PreviewCameraWindow PreviewWindow = null;

        /// <summary>
        /// 交账窗口
        /// </summary>
        private ReferBillWindow BillWindow = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShipOrderViewModel()
        {
            //Order = new ShipOrder();
            PonderDisplayItems = new ObservableCollection<PonderationDisplay> { Pond1, Pond2, Pond3, Pond4 };
            ColumnsVisibility = Config.ShipColVisibility;
            ColumnsVisibility.ColumnsMode(1);
            //GetData();
            //RunPond();
        }

        /// <summary>
        /// 根据地磅配置运行串口
        /// </summary>
        /// <param name="p"></param>
        /* 已废弃
        private void RunPortByPond(PonderationConfig pConfig, PonderationDisplay pDisplay, PondDataParameter pData)
        {
            //pDisplay.IsError = false;
            try
            {
                //if (!pConfig.Enable) return;
                SerialPort sPort = pConfig.GetSerialPort();
                PonderationCommon pondCommon = new PonderationCommon(sPort, pConfig.PondTypeName, pData);
                //Thread.Sleep(500); // 防止进出料过磅界面切换时串口没有关闭完全
                if (sPort.IsOpen) return;
                sPort.Open();
                while (!IsClosePond)
                {
                    string data = pondCommon.Run();
                    if (data != "")
                    {
                        var isStab = true;
                        pDisplay.WeightList.ForEach(delegate (string str)
                        {
                            if (str != data) isStab = false;
                        });
                        pDisplay.IsStability = isStab;
                        pDisplay.Weight = data; // 非展示用
                        pDisplay.WeightList.Add(data);
                        if (pDisplay.WeightList.Count > 5) pDisplay.WeightList.RemoveAt(0);
                        EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation { Name = pConfig.Name, Weight = data, Error = "" });
                    }
                }
                sPort.Close();
                sPort.Dispose();
                pDisplay.Reset();
            }
            catch (Exception er)
            {
                //pDisplay.Error();
                EventCenter.Broadcast(EventType.Ponder, new PonderEventInfomation { Name = pConfig.Name, Weight = "0", Error = "报错, 原因: " + er.Message + "\n检查仪表数据线是否松动，可尝试重启软件和地磅仪表。" });
                //EventCenter.Broadcast(EventType.Ponder, pConfig.Name + "报错, 原因: " + er.Message + "\n检查仪表数据线是否松动，可尝试重启软件和地磅仪表。");
            }
        }
        */

        public void RunPond()
        {
            if (Config.P1.Enable)
                Pond1.Awake();
            if (Config.P2.Enable)
                Pond2.Awake();
            if (Config.P3.Enable)
                Pond3.Awake();
            if (Config.P4.Enable)
                Pond4.Awake();
            Pond1.PondConfig = Config.P1;
            Pond2.PondConfig = Config.P2;
            Pond3.PondConfig = Config.P3;
            Pond4.PondConfig = Config.P4;
        }

        public void ClosePond()
        {
            //IsClosePond = true;
            if (Config.P1.Enable)
                Pond1.OnDestory();
            if (Config.P2.Enable)
                Pond2.OnDestory();
            if (Config.P3.Enable)
                Pond3.OnDestory();
            if (Config.P4.Enable)
                Pond4.OnDestory();
        }

        /// <summary>
        /// 拉取各项数据
        /// </summary>
        public void GetData()
        {
            GetOrders();
            RequestStatus.StartRequest(() =>
            {
                GoodsItems = new ObservableCollection<ShipGoods>(ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetGoodsAsync,
                    new ShipGoods { Valid = 1, Deleted = 0 }).Result.Data);
                CurrentCustomerPage.Reset(500);
                CustomerItems = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetShipCustomerAsync,
                    new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                     delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(ComboBox lv)
        {
            /*
            if (CurrentCustomerPage.Last || lv.Items.IsEmpty)
            {
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var result = new ObservableCollection<ShipCustomer>(GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                delegate (PageModel p) { CurrentCustomerPage = p; }).Result);
                foreach (var item in result)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        CustomerItems.Add(item);
                    });
                }
            });
            */
        }

        /// <summary>
        /// 获取单据信息并赋值绑定
        /// </summary>
        /// <param name="parameter"></param>
        public void GetOrders()
        {
            OrdersItems?.Clear();
            CurrentPage.Reset(20);
            var m = 1;// MenuIndex == 2 ? -1 : MenuIndex;
            RequestStatus.StartRequest(() =>
            {
                OrderListBoxIsEnabled = false;
                var r = ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetShipOrdersAsync,
                    new
                    {
                        CarId = CarIDSearchFeed.Equals("") ? null : CarIDSearchFeed,
                        Status = m,
                        Page = CurrentPage.Page - 1,
                        Size = CurrentPage.Size
                    },
                    delegate (DataInfo<List<ShipOrder>> result)
                    {
                        OrdersItems = new ObservableCollection<ShipOrder>(result.Data);
                        CurrentPage = result.Page;
                        JObject jar = JObject.Parse(result.Obj.ToString());
                        var str = "车数： " + jar["totalCount"] + "            重量：" + (jar["totalWeight"] ?? 0) + "            金额：" + (jar["totalMoney"] ?? 0) + "            零售金额：" + (jar["salesMoney"] ?? 0) + "            客户金额：" + (jar["cusMoney"] ?? 0);
                        DetailOrderStr = str;
                    }).Result;
                OrderListBoxIsEnabled = true;
            });
        }

        /// <summary>
        /// 获取CarIdItems
        /// </summary>
        public void GetCustomerCarByCarId()
        {
            CustomerCarItems?.Clear();
            Task.Run(() =>
            {
                List<ShipCustomerCar> scc = ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetShipCustomerCarAsync,
                         new { CarId = Order.CarId }).Result.Data;
                if (scc != null)
                    CustomerCarItems = new ObservableCollection<ShipCustomerCar>(scc);
                //CarIdIsDropDownOpen = CustomerCarItems.Count > 0;  // 废弃
            });
        }

        public void Calculate(int sender)
        {
            //Order.Calculate(sender);
            /*
            try
            {
            */
            if (sender != 1)
            {
                Order.OrderMoney = Common.Double2DecimalCalculate(Order.CarNetWeight * Order.GoodsRealPrice);
                Order.PlatformMoney = Common.Double2DecimalCalculate(Order.CarNetWeight * Order.GoodsPrice);
                if (Order.CustomerType == 0 && Order.Customer == null)
                {
                    switch (Config.SYSTEM_SETTING.ShipOrderDiscount)
                    {
                        case 1: //个位抹零
                            Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 10);
                            break;
                        case 2: //小数点抹零
                            Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 1);
                            break;
                        case 3: //四舍五入
                            var n = Common.Double2DecimalCalculate(Order.OrderMoney % 10); //个位以后
                            var u = n - Common.Double2DecimalCalculate(n % 1); //个位
                            Order.DiscountMoney = u >= 5 ? n - 10 : Common.Double2DecimalCalculate(Order.OrderMoney % 10);
                            break;
                        case 0: //无
                        default:
                            Order.DiscountMoney = 0;
                            break;
                    }
                }
                else
                {
                    //Order.DiscountMoney = (Order.Customer != null && Order.Customer.Sale == 1) ? Common.Double2DecimalCalculate(Order.OrderMoney % 10) : 0;
                    if (Order.Customer != null)
                    {
                        if (Order.Customer.Sale == 1)
                        {
                            // 抹零
                            //DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 10);
                            switch (Config.SYSTEM_SETTING.CustomerDiscount)
                            {
                                case 1: //个位抹零
                                    Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 10);
                                    break;
                                case 2: //小数点抹零
                                    Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 1);
                                    break;
                                case 3: //四舍五入
                                    var n = Common.Double2DecimalCalculate(Order.OrderMoney % 10); //个位以后
                                    var u = n - Common.Double2DecimalCalculate(n % 1); //个位
                                    Order.DiscountMoney = u >= 5 ? n - 10 : Common.Double2DecimalCalculate(Order.OrderMoney % 10);
                                    break;
                                case 0: //无
                                default:
                                    Order.DiscountMoney = 0;
                                    break;
                            }
                        }
                        else
                        {
                            //不抹零
                            Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 1);
                        }
                    }
                    else
                    {
                        Order.DiscountMoney = 0;
                    }
                }
            }
            Order.RealMoney = Common.Double2DecimalCalculate(Order.OrderMoney - Order.DiscountMoney);
            /*
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
        }

        public ICommand ResetOrderCommand => new AnotherCommandImplementation(ResetOrder);
        public ICommand SubmitOrderCommand => new AnotherCommandImplementation(SubmitOrderAsync);
        public ICommand RunOrderDetailsCommand => new AnotherCommandImplementation(ExecuteRunOrderDetails);
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand SoftKeyBoardClickCommand => new AnotherCommandImplementation(ExecuteSoftKeyBoardClickCommand);
        public ICommand SoftKeyBoardBackspaceCommand => new AnotherCommandImplementation(ExecuteSoftKeyBoardBackspaceCommand);
        public ICommand ApplySoftCarIdCommand => new AnotherCommandImplementation(ExecuteApplySoftCarIdCommand);
        public ICommand RunPreviewCameraWindowCommand => new AnotherCommandImplementation(ExecuteRunPreviewCameraWindowCommand);
        public ICommand RunReferBillWindowCommand => new AnotherCommandImplementation(ExecuteRunReferBillWindowCommand);
        public ICommand RunShipOrderMendPageCommand => new AnotherCommandImplementation(ExecuteRunShipOrderMendPageCommand);
        public ICommand SearchOrderByCarIDCommand => new AnotherCommandImplementation(ExecuteSearchOrderByCarIDCommand);
        public ICommand MendPrintBillCommand => new AnotherCommandImplementation(ExecuteMendPrintBillCommand);
        public ICommand DeleteShipOrderCommand => new AnotherCommandImplementation(ExecuteDeleteShipOrderCommand);

        public void ResetOrder(object o)
        {
            Order.Reset();
            CurrentPonderationDisplay = null;
            CustomerCarItems = null;
            CarIDSearchFeed = "";
        }
        /// <summary>
        /// 提交处理
        /// </summary>
        /// <param name="o"></param>
        private async void SubmitOrderAsync(object o)
        {
            try
            {
                RequestStatus.AddOneRequest();
                #region 提交校验
                //if (CurrentPonderationDisplay == null) { BiuMessageBoxWindows.BiuShow("未选择地磅!", image: BiuMessageBoxImage.Error); return; }
                if (Order.EmptyCar != 1 && Order.CarNetWeight < 0.0) { BiuMessageBoxWindows.BiuShow("净重不可小于0!", image: BiuMessageBoxImage.Error); return; }
                if (Order.CustomerType == 1 && Order.Customer == null) { BiuMessageBoxWindows.BiuShow("请选择客户!", image: BiuMessageBoxImage.Error); return; }
                //if (string.IsNullOrEmpty(Order.RFID)) { BiuMessageBoxWindows.BiuShow("请录入RFID!", image: BiuMessageBoxImage.Error); return; }
                if (string.IsNullOrWhiteSpace(Order.CarId) || Order.Goods == null) { BiuMessageBoxWindows.BiuShow("未填写车牌号或未选择料品!", image: BiuMessageBoxImage.Error); return; }
                if (Order.EmptyCar == 1 && Order.CarNetWeight >= 0.4) { BiuMessageBoxWindows.BiuShow("净重较大，不可设置为空车出厂!"); return; }
                //if (Order.Status == 0 && Order.CarTare < 0.4 && BiuMessageBoxResult.No.Equals(BiuMessageBoxWindows.BiuShow("皮重小于0.4，是否继续提交?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
                if (Order.Status == 1 && Order.EmptyCar == 0 && Math.Abs(Order.CarGrossWeight - Order.CarTare) <= 0.4 && !BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("毛重皮重相差较小，是否继续提交?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
                if (Order.Status == 1 && (Order.GoodsRealPrice == 0 || Order.RealMoney == 0) && BiuMessageBoxResult.No.Equals(BiuMessageBoxWindows.BiuShow("执行单价或实收金额为0，是否继续提交?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
                if (Order.EmptyCar == 1 && BiuMessageBoxResult.No.Equals(BiuMessageBoxWindows.BiuShow("是否设置为空车出厂?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
                #endregion
                var Result = new DataInfo<ShipOrder>();
                /*
                #region 进厂
                if (Order.Status == 0)
                {
                    Order.EnterPonderation = CurrentPonderationDisplay?.Name ?? "#";
                    //Result = await ApiClient.CreateEnterShipOrderAsync(Order);
                    Result = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateEnterShipOrderAsync, Order);
                }
                #endregion
                #region 出厂
                if (Order.Status == 1)
                {
                    Order.ExitPonderation = CurrentPonderationDisplay?.Name ?? "#";
                    //Result = await ApiClient.CreateExitShipOrderAsync(Order);
                    Result = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateExitShipOrderAsync, Order);
                }
                #endregion
                */
                RequestStatus.AddOneRequest();
                try
                {
                    Result = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.MendShipOrderAsync, Order);
                    if (Result.Code != 200)
                        throw new Exception(Result.ToString());
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                }
                finally
                {
                    RequestStatus.CompleteOneRequest();
                }

                if (Result.Code != 200)
                {
                    //throw new Exception(Result.ToString());
                    BiuMessageBoxWindows.BiuShow(Result.ToString(), image: BiuMessageBoxImage.Error);
                }
                else
                {
                    //打印票据
                    try
                    {
                        //BillPrinter.GetInstance().PrintShip(Result.Data, "");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
                    }

                    //抓拍
                    /*
                    var pondConfig = CurrentPonderationDisplay.PondConfig;
                    if (pondConfig.CaptureEnable)
                        CapturePicture(pondConfig, Result.Data.ID);
                    */

                    CarIDSearchFeed = "";
                    ResetOrder(true);
                    MenuIndex = 0;
                    GetData();
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            finally
            {
                RequestStatus.CompleteOneRequest();
            }
        }
        public async void ExecuteRunOrderDetails(object o)
        {
            try
            {
                if (!(o is ShipOrder order)) return;
                var shipOrderDetail = new ShipOrderDetailsViewModel(order);
                if (order.Status == 0)
                {
                    var enterDetail = new ShipOrderEnterDetailsDialog
                    {
                        DataContext = shipOrderDetail
                    };
                    await DialogHost.Show(enterDetail, "RootDialog", ClosingEventHandler);
                }
                else
                {
                    var detail = new ShipOrderDetailsDialog
                    {
                        DataContext = shipOrderDetail,
                    };
                    await DialogHost.Show(detail, "RootDialog", ClosingEventHandler);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }
        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetOrders();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetOrders();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetOrders();
            }
        }
        private void ExecuteSoftKeyBoardClickCommand(object o)
        {
            var str = o.ToString();
            if (string.IsNullOrWhiteSpace(SoftCarId))
            {
                SoftCarId = str;
                SoftCarIdPre = str.Count();
                return;
            }
            if (!RegexChecksum.IsChineseCharacters(str))
            {
                SoftCarId += str;
                return;
            }
            var str1 = SoftCarId.Substring(0, SoftCarIdPre);
            SoftCarId = SoftCarId.Remove(0, SoftCarIdPre);
            SoftCarId = SoftCarId.Insert(0, str);
            SoftCarIdPre = str.Count();
        }
        private void ExecuteSoftKeyBoardBackspaceCommand(object o)
        {
            if (!string.IsNullOrWhiteSpace(SoftCarId))
                SoftCarId = SoftCarId.Remove(SoftCarId.Count() - 1);
        }
        private void ExecuteApplySoftCarIdCommand(object o)
        {
            Order.CarId = SoftCarId;
            SoftCarId = "";
            SoftKeyboardIsOpen = false;
        }
        private void ExecuteRunPreviewCameraWindowCommand(object o)
        {
            if (PreviewWindow == null || PreviewWindow.IsClose) PreviewWindow = new PreviewCameraWindow();
            PreviewWindow.Show();
            PreviewWindow.Focus();
        }
        private void ExecuteRunReferBillWindowCommand(object o)
        {
            if (BillWindow == null || BillWindow.IsClose) BillWindow = new ReferBillWindow(0);
            BillWindow.IShow();
            /*
            BillWindow.ShowDialog();
            BillWindow.Focus();
            */
            GetOrders();
        }
        private async void ExecuteRunShipOrderMendPageCommand(object o)
        {
            var view = new ShipOrderMendPage();
            await DialogHost.Show(view, "RootDialog");
            ResetOrder(true);
            MenuIndex = 1;
            GetData();
        }
        private void ExecuteSearchOrderByCarIDCommand(object o)
        {
            //CurrentPage.Reset(20);
            GetOrders();
        }
        private void ExecuteMendPrintBillCommand(object o)
        {
            try
            {
                if (o == null) return;
                BillPrinter.GetInstance().PrintShip((ShipOrder)o);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
            }
        }
        private async void ExecuteDeleteShipOrderCommand(object o)
        {
            if (!(o is ShipOrder order)) return;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认作废该订单? 作废后不可恢复!", BiuMessageBoxButton.YesNo)))
            {
                await ModelHelper.GetInstance().GetApiDataArg(ApiClient.DeleteExitShipOrderAsync, new { ID = (o as ShipOrder).ID });
                ResetOrder(true);
                //GetOrders();
                GetData();
            }
        }


        public void NextPageOrderItems(DataGrid lv)
        {
            if (CurrentPage.Last)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                return;
            }
            var m = 1;// MenuIndex == 2 ? -1 : MenuIndex;
            RequestStatus.StartRequest(() =>
            {
                CurrentPage.Page++;
                OrderListBoxIsEnabled = false;
                var r = new ObservableCollection<ShipOrder>(
                    ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetShipOrdersAsync,
                    new
                    {
                        CarId = CarIDSearchFeed.Equals("") ? null : CarIDSearchFeed,
                        Status = m,
                        Page = CurrentPage.Page - 1,
                        Size = CurrentPage.Size
                    }, delegate (DataInfo<List<ShipOrder>> result) { CurrentPage = result.Page; },
                    delegate (DataInfo<List<ShipOrder>> result) { CurrentPage.Page--; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        OrdersItems.Add(item);
                    });
                }
                OrderListBoxIsEnabled = true;
            });
        }

        /// <summary>
        /// 获取当前料品的价格
        /// 传入 0 或 1 进行计算
        /// </summary>
        public void SetPriceByGoods(int sender = -1)
        {
            RequestStatus.StartRequest(() =>
            {
                /*
                try
                {
                */
                if (Order.Goods == null) return;
                if (MenuIndex == 0 && CurrentEnterOrder != null
                && Order.Customer?.ID == CurrentEnterOrder.Customer?.ID
                && Order.Goods.ID == CurrentEnterOrder.Goods.ID)
                {
                    Order.GoodsPrice = CurrentEnterOrder.GoodsPrice;
                    Order.GoodsRealPrice = CurrentEnterOrder.GoodsRealPrice;
                }
                else
                {
                    if (Order.CustomerType == 1 && Order.Customer != null)
                    {
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetShipCustomerGoodsPriceSingleAsync, new { GoodsId = Order.Goods.ID, CustomerId = Order.Customer.ID }).Result.Data;
                        Order.GoodsPrice = g.Price;
                        Order.GoodsRealPrice = g.CustomerPrice;
                    }
                    else
                    {
                        //var g = GetGoodsById(Order.Goods).Result;
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetGoodsByIdAsync, Order.Goods).Result.Data;
                        Order.GoodsPrice = g.Price;
                        Order.GoodsRealPrice = g.RealPrice;
                    }
                }
                if (sender != -1)
                    Calculate(sender);
                /*
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
            */
            });
        }

        /// <summary>
        /// 获取重量到Order对象中
        /// </summary>
        public void RefreshWeight()
        {
            //if (CurrentPonderationDisplay != null)
            //{
            /*
                if (Order.Status == 0)
                {
                    Order.CarTare = Convert.ToDouble(CurrentPonderationDisplay.Weight);
                }
                else if (Order.Status == 1)
                {
                    Order.CarGrossWeight = Convert.ToDouble(CurrentPonderationDisplay.Weight);
            */
                    Calculate(0);
            /*
                }
            */
            //}
            /*
            foreach (var item in PonderDisplayItems)
            {
                if (item.Name.Equals(CurrentPonderationDisplay?.Name))
                {
                    item.Selected();
                }
                else
                {
                    item.UnSelected();
                }
            }
            */
        }

        public void ResetSnapshotPicture()
        {
            //SnapshotPicture = null;
            //SnapshotPicture = new Snapshot();
        }

        /// <summary>
        /// 抓拍并上传图片
        /// </summary>
        private void CapturePicture(PonderationConfig pondConfig, String orderID)
        {
            Task.Run(() =>
            {
                try
                {
                    var token = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetQiniuUploadToken, new { id = orderID, Companyid = Config.COMPANY_ID }).GetAwaiter().GetResult();
                    if (token.Code != 200)
                    {
                        BiuMessageBoxWindows.BiuShow(token.Msg, image: BiuMessageBoxImage.Error);
                        return;
                    }
                    //抓拍截图
                    var paths = Config.SnapshotPicture.GetSnapshotByPonderation(pondConfig);
                    //上传
                    UploadManager target = new UploadManager(Config.QiniuUploadConfig);
                    paths.ForEach(path =>
                    {
                        if (!path.Equals(""))
                        {
                            if (!File.Exists(path))
                            {
                                BiuMessageBoxWindows.BiuShow("抓拍图片不存在，上传失败! \n path:" + path, image: BiuMessageBoxImage.Error);
                            }
                            else
                            {
                                var key = Common.GetMD5ByFile(path);
                                HttpResult result = target.UploadFile(path, key + ".jpg", token.Data, null);
                                if (result.Code != 200)
                                {
                                    BiuMessageBoxWindows.BiuShow("抓拍上传失败！\n" + result.ToString(), image: BiuMessageBoxImage.Error);
                                }
                                else
                                {
                                    File.Delete(path);
                                }
                            }
                        }
                    });
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// 搜索出料客户
        /// </summary>
        public void GetShipCustomerItems(string SearchCusSeed = "")
        {
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<ShipCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetShipCustomerAsync,
                  new { szm = SearchCusSeed, Page = 0, Size = 500 },
                 delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            }, null);
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            ResetOrder(true);
            GetData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "CurrentPonderationDisplay":
                    RefreshWeight();
                    break;
                case "MenuIndex":
                    //ColumnsVisibility.ColumnsMode(MenuIndex);
                    CarIDSearchFeed = "";
                    GetOrders();
                    CurrentPage.Page = 1;
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
