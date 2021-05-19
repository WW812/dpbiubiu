using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.print;
using biubiu.model.stock_order;
using biubiu.view_model.ship_order;
using biubiu.views.marketing.stock_order;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Qiniu.Http;
using Qiniu.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wow;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_order
{
    public class StockOrderViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 料品
        /// </summary>
        private ObservableCollection<StockGoods> _goodsItems;
        public ObservableCollection<StockGoods> GoodsItems
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
        private StockOrder _order;
        public StockOrder Order
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
        private StockOrder _currentEnterOrder;
        public StockOrder CurrentEnterOrder
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
        private StockOrderColumnsVisibility _columnsVisibility;
        public StockOrderColumnsVisibility ColumnsVisibility
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
        private ObservableCollection<StockCustomerCar> _customerCarItems;
        public ObservableCollection<StockCustomerCar> CustomerCarItems
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
        private ObservableCollection<StockCustomer> _customerItems;
        public ObservableCollection<StockCustomer> CustomerItems
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
        private Boolean _orderListBoxIsEnabled = true;
        public Boolean OrderListBoxIsEnabled
        {
            get { return _orderListBoxIsEnabled; }
            set
            {
                _orderListBoxIsEnabled = value;
                NotifyPropertyChanged("OrderListBoxIsEnabled");
            }
        }

        /// <summary>
        /// 车牌号是否展开
        /// </summary>
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

        /// <summary>
        /// 单据集合
        /// </summary>
        private ObservableCollection<StockOrder> _ordersItems;
        public ObservableCollection<StockOrder> OrdersItems
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

        private Boolean IsClosePond = false;
        //private Snapshot SnapshotPicture = new Snapshot();

        /// <summary>
        /// 单据列表选择 作为拉取单子的标识
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

        /// <summary>
        /// 摄像头预览窗口
        /// </summary>
        private PreviewCameraWindow PreviewWindow = null;

        /// <summary>
        /// 交账窗口
        /// </summary>
        private ReferBillWindow BillWindow = null;


        public StockOrderViewModel()
        {
            Order = new StockOrder();
            PonderDisplayItems = new ObservableCollection<PonderationDisplay> { Pond1, Pond2, Pond3, Pond4 };
            ColumnsVisibility = Config.StockColVisibility;
        }

        public ICommand ResetOrderCommand => new AnotherCommandImplementation(ResetOrder);
        public ICommand SubmitOrderCommand => new AnotherCommandImplementation(SubmitOrderAsync);
        public ICommand RunOrderDetailsCommand => new AnotherCommandImplementation(ExcuteRunOrderDetails);
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExcuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExcutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExcuteNextPageCommand);
        public ICommand SoftKeyBoardClickCommand => new AnotherCommandImplementation(ExcuteSoftKeyBoardClickCommand);
        public ICommand SoftKeyBoardBackspaceCommand => new AnotherCommandImplementation(ExcuteSoftKeyBoardBackspaceCommand);
        public ICommand ApplySoftCarIdCommand => new AnotherCommandImplementation(ExcuteApplySoftCarIdCommand);
        public ICommand RunPreviewCameraWindowCommand => new AnotherCommandImplementation(ExecuteRunPreviewCameraWindowCommand);
        public ICommand RunReferBillWindowCommand => new AnotherCommandImplementation(ExecuteRunReferBillWindowCommand);
        public ICommand RunStockOrderMendPageCommand => new AnotherCommandImplementation(ExecuteRunStockOrderMendPageCommand);
        public ICommand MendPrintBillCommand => new AnotherCommandImplementation(ExecuteMendPrintBillCommand);
        public ICommand DeleteStockOrderCommand => new AnotherCommandImplementation(ExecuteDeleteStockOrderCommand);



        public void ResetOrder(object o)
        {
            Order.Reset();
            CurrentPonderationDisplay = null;
            CustomerCarItems = null;
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
                if (CurrentPonderationDisplay == null) throw new Exception("未选择地磅!");
                if (Order.CarNetWeight < 0.0) throw new Exception("净重不可小于0!");
                if (Order.CarId == null || Order.Goods == null) throw new Exception("未填写车牌号或未选择料品!");
                if (Order.CustomerType == 1 && Order.Customer == null) throw new Exception("请选择客户!");
                if (Order.Status == 1 && Math.Abs(Order.CarGrossWeight - Order.CarTare) <= 0.4 && !BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("毛重皮重相差较小，是否继续提交?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
                #endregion

                var Result = new DataInfo<StockOrder>();
                #region 进厂
                if (Order.Status == 0)
                {
                    Order.EnterPonderation = CurrentPonderationDisplay?.Name ?? "#";
                    //Result = await ApiClient.CreateEnterStockOrderAsync(Order);
                    Result = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateEnterStockOrderAsync, Order);
                }
                #endregion
                #region 出厂
                if (Order.Status == 1)
                {
                    Order.ExitPonderation = CurrentPonderationDisplay?.Name ?? "#";
                    //Result = await ApiClient.CreateExitStockOrderAsync(Order);
                    Result = await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateExitStockOrderAsync, Order);
                }
                #endregion
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    try
                    {
                        BillPrinter.GetInstance().PrintStock(Result.Data, "");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
                    }

                    //抓拍
                    var pondConfig = CurrentPonderationDisplay.PondConfig;
                    if (pondConfig.CaptureEnable)
                        CapturePicture(pondConfig, Result.Data.ID);

                    ResetOrder(true);
                    MenuIndex = 0;
                    GetOrders();
                    GetGoods();
                    GetCustomer();
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
        public async void ExcuteRunOrderDetails(object o)
        {
            if (!(o is StockOrder order)) return;
            //var orderDetail = new StockOrderDetailsViewModel(order);
            if (order.Status == 0)
            {
                var enterDetail = new StockOrderEnterDetailsDialog
                {
                    DataContext = new StockOrderDetailsViewModel(order)
                };
                await DialogHost.Show(enterDetail, "RootDialog", ClosingEventHandler);
            }
            else
            {
                var detail = new StockOrderDetailsDialog
                {
                    DataContext = new StockOrderDetailsViewModel(order)
                    //IsChange = false
                };
                await DialogHost.Show(detail, "RootDialog", ClosingEventHandler);
            }
        }
        private void ExcuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            //GetOrders();
        }
        private void ExcutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                //GetOrders();
            }
        }
        private void ExcuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                //GetOrders();
            }
        }
        private void ExcuteSoftKeyBoardClickCommand(object o)
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
        private void ExcuteSoftKeyBoardBackspaceCommand(object o)
        {
            if (!string.IsNullOrWhiteSpace(SoftCarId))
                SoftCarId = SoftCarId.Remove(SoftCarId.Count() - 1);
        }
        private void ExcuteApplySoftCarIdCommand(object o)
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
            if (BillWindow == null || BillWindow.IsClose) BillWindow = new ReferBillWindow(1);
            BillWindow.IShow();
            /*
            BillWindow.ShowDialog();
            BillWindow.Focus();
            */
            GetOrders();
        }
        private async void ExecuteRunStockOrderMendPageCommand(object o)
        {
            var view = new StockOrderMendPage();
            await DialogHost.Show(view, "RootDialog");
            ResetOrder(true);
            MenuIndex = 1;
        }
        private void ExecuteMendPrintBillCommand(object o)
        {
            try
            {
                if (o == null) return;
                BillPrinter.GetInstance().PrintStock((StockOrder)o);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
            }
        }
        private async void ExecuteDeleteStockOrderCommand(object o)
        {
            if (!(o is StockOrder order)) return;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认作废该订单? 作废后不可恢复!", BiuMessageBoxButton.YesNo)))
            {
                await ModelHelper.GetInstance().GetApiDataArg(ApiClient.DeleteExitStockOrderAsync, new { ID = (o as StockOrder).ID });
                ResetOrder(true);
                GetOrders();
            }
        }


        /// <summary>
        /// 根据地磅配置运行串口
        /// </summary>
        /// <param name="p"></param>
        private void RunPortByPond(PonderationConfig pConfig, PonderationDisplay pDisplay, PondDataParameter pParameter)
        {
            pDisplay.IsError = false;
            try
            {
                //if (!pConfig.Enable) return;
                SerialPort sPort = pConfig.GetSerialPort();
                PonderationCommon pondCommon = new PonderationCommon(sPort, pConfig.PondTypeName, pParameter);
                Thread.Sleep(500);
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
                        pDisplay.Weight = data;
                        pDisplay.WeightList.Add(data);
                        if (pDisplay.WeightList.Count > 5) pDisplay.WeightList.RemoveAt(0);
                    }
                }
                sPort.Close();
                sPort.Dispose();
                pDisplay.Reset();
            }
            catch (Exception er)
            {
                pDisplay.Error();
                BiuMessageBoxWindows.BiuShow(pConfig.Name + "报错, 原因: " + er.Message, image: BiuMessageBoxImage.Error);
            }
        }
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
            /*
            // 1磅
            Task.Run(() =>
            {
                Pond1.PondConfig = Config.P1;
                if (Config.P1.Enable)
                {
                    RunPortByPond(Config.P1, Pond1, Config.PondDataP1);
                }
            });

            // 2磅
            Task.Run(() =>
            {
                Pond2.PondConfig = Config.P2;
                if (Config.P2.Enable)
                {
                    RunPortByPond(Config.P2, Pond2, Config.PondDataP2);
                }
            });

            // 3磅
            Task.Run(() =>
            {
                Pond3.PondConfig = Config.P3;
                if (Config.P3.Enable)
                {
                    RunPortByPond(Config.P3, Pond3, Config.PondDataP3);
                }
            });

            // 4磅
            Task.Run(() =>
            {
                Pond4.PondConfig = Config.P4;
                if (Config.P4.Enable)
                {
                    RunPortByPond(Config.P4, Pond4, Config.PondDataP4);
                }
            });
            */
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

        public void OpenPond()
        {
            //IsClosePond = false;
        }

        public void Calculate(int sender)
        {
            Order.Calculate(sender);
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
                    //var token = ApiClient.GetQiniuUploadToken(new { id = orderID, Companyid = Config.COMPANY_ID }).GetAwaiter().GetResult();
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
                                BiuMessageBoxWindows.BiuShow("抓拍图片不存在，上传失败!", image: BiuMessageBoxImage.Error);
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
        /// 获取单据
        /// </summary>
        public void GetOrders()
        {
            Thread.Sleep(80);
            OrdersItems?.Clear();
            RequestStatus.StartRequest(() =>
            {
                OrdersItems = new ObservableCollection<StockOrder>(ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetStockOrdersAsync,
                    new
                    {
                        Status = MenuIndex,
                        Page = CurrentPage.Page - 1,
                        Size = 20
                    },
                    delegate (DataInfo<List<StockOrder>> result)
                    {
                        CurrentPage = result.Page;
                        CurrentPage = result.Page;
                        JObject jar = JObject.Parse(result.Obj.ToString());
                        var str = "车数： " + jar["totalCount"] + "            重量：" + (jar["totalWeight"] ?? 0) + "            金额：" + (jar["totalMoney"] ?? 0) + "            散户金额：" + (jar["salesMoney"] ?? 0) + "            客户金额：" + (jar["cusMoney"] ?? 0);
                        DetailOrderStr = str;
                    }).Result.Data);
            });
        }

        /// <summary>
        /// 获取车牌号
        /// </summary>
        public void GetCustomerCarByCarId()
        {
            CustomerCarItems?.Clear();
            Task.Run(() =>
            {
                CustomerCarItems = new ObservableCollection<StockCustomerCar>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetStockCustomerCarAsync,
                        new { CarId = Order.CarId }).Result.Data);
                CarIdIsDropDownOpen = CustomerCarItems.Count > 0;
            });
        }

        public void NextPageOrderItems(DataGrid lv)
        {
            if (CurrentPage.Last)
            {
                Task.Run(() =>
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                });
                return;
            }
            var m = MenuIndex == 2 ? -1 : MenuIndex;
            RequestStatus.StartRequest(() =>
            {
                CurrentPage.Page++;
                OrderListBoxIsEnabled = false;
                var r = new ObservableCollection<StockOrder>(
                    ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockOrdersAsync,
                    new
                    {
                        //CarId = CarIDSearchFeed.Equals("") ? null : CarIDSearchFeed,
                        Status = m,
                        Page = CurrentPage.Page - 1,
                        Size = CurrentPage.Size
                    }, delegate (DataInfo<List<StockOrder>> result) { CurrentPage = result.Page; },
                    delegate (DataInfo<List<StockOrder>> result) { CurrentPage.Page--; }).Result.Data);
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
        /// 获取料品
        /// </summary>
        public void GetGoods()
        {
            RequestStatus.StartRequest(() =>
            {
                GoodsItems = new ObservableCollection<StockGoods>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetStockGoodsAsync,
                        new { Valid = 1 }).Result.Data);
            });
        }

        /// <summary>
        /// 获取客户
        /// </summary>
        public void GetCustomer()
        {
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Reset();
                CustomerItems = new ObservableCollection<StockCustomer>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetStockCustomerAsync,
                        new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                        delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(ComboBox lv)
        {
            if (CurrentCustomerPage.Last || lv.Items.IsEmpty)
            {
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetStockCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        CustomerItems.Add(item);
                    });
                }
            });
        }

        /// <summary>
        /// 获取当前料品的价格
        /// </summary>
        public void SetPriceByGoods()
        {
            RequestStatus.StartRequest(() =>
            {
                if (Order.Goods == null) return;
                if (MenuIndex == 0 && CurrentEnterOrder != null
                && Order.Customer?.ID == CurrentEnterOrder.Customer?.ID
                && Order.Goods.ID == CurrentEnterOrder.Goods.ID)
                {
                    Order.GoodsRealPrice = CurrentEnterOrder.GoodsRealPrice;
                }
                else
                {
                    if (Order.CustomerType == 1 && Order.Customer != null)
                    {
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockCustomerGoodsPriceSingleAsync, new { GoodsId = Order.Goods.ID, CustomerId = Order.Customer.ID }).Result.Data;
                        Order.GoodsRealPrice = g.CustomerPrice;
                    }
                    else
                    {
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockGoodsByIdAsync, Order.Goods).Result.Data;
                        Order.GoodsRealPrice = g.RealPrice;
                    }
                }
            });
        }

        /// <summary>
        /// 获取重量到Order对象中
        /// </summary>
        public void RefreshWeight()
        {
            if (CurrentPonderationDisplay != null)
            {
                if (Order.Status == 0)
                {
                    Order.CarGrossWeight = Convert.ToDouble(CurrentPonderationDisplay.Weight);
                }
                else if (Order.Status == 1)
                {
                    Order.CarTare = Convert.ToDouble(CurrentPonderationDisplay.Weight);
                }
            }
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
        }

        public void ResetSnapshotPicture()
        {
            //SnapshotPicture = null;
            //SnapshotPicture = new Snapshot();
        }

        /// <summary>
        /// 搜索进料客户
        /// </summary>
        public void GetShipCustomerItems(string SearchCusSeed = "")
        {
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<StockCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetStockCustomerAsync,
                  new { szm = SearchCusSeed, Page = 0, Size = 500 },
                 delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
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
            GetOrders();
            GetCustomer();
            GetGoods();
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
                    ColumnsVisibility.ColumnsMode(MenuIndex);
                    GetOrders();
                    CurrentPage.Page = 1;
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
