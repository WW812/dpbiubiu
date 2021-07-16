using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.print;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
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

namespace biubiu.view_model.ship_order
{
    public class ShipOrderDetailsViewModel : INotifyPropertyChanged
    {
        private ShipOrder _order;
        public ShipOrder Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                NotifyPropertyChanged("Order");
            }
        }

        /// <summary>
        /// 原单据对象
        /// </summary>
        private ShipOrder _oldOrder;
        public ShipOrder OldOrder
        {
            get { return _oldOrder; }
            set
            {
                _oldOrder = value;
                NotifyPropertyChanged("OldOrder");
            }
        }

        /// <summary>
        /// 是否处于修改状态
        /// </summary>
        private Boolean _isEditing = false;
        public Boolean IsEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                _isEditing = value;
                NotifyPropertyChanged("IsEditing");
            }
        }

        /// <summary>
        /// 客户滚动条页码
        /// </summary>
        private PageModel _currentCustomerPage = new PageModel(500);
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
        /// 料品集合
        /// </summary>
        private ObservableCollection<ShipGoods> _goodsItems;
        public ObservableCollection<ShipGoods> GoodsItems
        {
            get { return _goodsItems; }
            set
            {
                _goodsItems = value;
                NotifyPropertyChanged("GoodsItems");
            }
        }

        /// <summary>
        /// 提交按钮是否可用
        /// </summary>
        /*
        private Boolean _submitButtonIsEnabled = true;
        public Boolean SubmitButtonIsEnabled
        {
            get { return _submitButtonIsEnabled; }
            set
            {
                _submitButtonIsEnabled = value;
                NotifyPropertyChanged("SubmitButtonIsEnabled");
            }
        }
        */

        /// <summary>
        /// 修改原因集合
        /// </summary>
        private ObservableCollection<string> _editReasonItems;
        public ObservableCollection<string> EditReasonItems
        {
            get { return _editReasonItems; }
            set
            {
                _editReasonItems = value;
                NotifyPropertyChanged("EditReasonItems");
            }
        }

        /// <summary>
        /// 抓拍图片地址集合
        /// </summary>
        /*
        private ObservableCollection<string> _pictureURLItems;
        public ObservableCollection<string> PictureURLItems
        {
            get { return _pictureURLItems; }
            set
            {
                _pictureURLItems = value;
                NotifyPropertyChanged("PictureURLItems");
            }
        }
        */

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
        /// 客户集合
        /// </summary>
        private ObservableCollection<ShipCustomer> _customerItems;
        public ObservableCollection<ShipCustomer> CustomerItems
        {
            get { return _customerItems; }
            set
            {
                _customerItems = value;
                NotifyPropertyChanged("CustomerItems");
            }
        }

        // 补印按钮是否可用
        private bool _mendPrintBtnEnabled = true;
        public bool MendPrintBtnEnabled
        {
            get { return _mendPrintBtnEnabled; }
            set
            {
                _mendPrintBtnEnabled = value;
                NotifyPropertyChanged("MendPrintBtnEnabled");
            }
        }

        /// <summary>
        /// 第一页客户分页是否含有 选中项
        /// </summary>
        private bool isFind = false;

        public ShipOrderDetailsViewModel(ShipOrder order)
        {
            RequestStatus.StartRequest(() =>
            {
                //SubmitButtonIsEnabled = false;
                OldOrder = Common.DeepCopy(order);
                Order = Common.DeepCopy(order);
                GoodsItems = new ObservableCollection<ShipGoods>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetGoodsAsync,
                        new ShipGoods { Valid = 1, Deleted = 0 }).Result.Data);

                CustomerItems = new ObservableCollection<ShipCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetShipCustomerAsync, new
                   {
                       Page = CurrentCustomerPage.Page - 1,
                       Size = CurrentCustomerPage.Size
                   }, delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);

                try
                {
                    foreach (var item in GoodsItems)
                    {
                        if (item.ID == Order.Goods.ID)
                        {
                            Order.Goods = item;
                        }
                    }

                    foreach (var item in CustomerItems)
                    {
                        if (item.ID == Order.Customer?.ID)
                        {
                            Order.Customer = item;
                            isFind = true;
                        }
                    }
                    if (!isFind)
                    {
                        CustomerItems.Add(Order.Customer);
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

                //SubmitButtonIsEnabled = true;
            });
            Task.Run(() =>
            {
                //获取图片地址
                /*
                PictureURLItems = new ObservableCollection<string>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetPictureURL,
                        new { id = order.ID }).Result.Data);
                        */
                EditReasonItems = new ObservableCollection<string> {
                    "1.皮重错误",
                    "2.毛重错误",
                    "3.车号错误",
                    "4.料品选择错误",
                    "5.客户选择错误",
                    "6.执行单价错误",
                    "7.其他"
                };
            });
        }

        #region 发卡器
        public void AwakeRFID_LJYZN()
        {
            EventCenter.AddListener<LJYZN_RFIDEventInfomation>(EventType.LJYZN_RFID, DecodeRFIDCode);
        }

        public void RemoveRFID_LJYZN()
        {
            EventCenter.RemoveListener<LJYZN_RFIDEventInfomation>(EventType.LJYZN_RFID, DecodeRFIDCode);
        }

        /// <summary>
        /// 解析RFID返回码
        /// </summary>
        private void DecodeRFIDCode(LJYZN_RFIDEventInfomation rfidInfo)
        {
            if (!IsEditing) return;
            switch (rfidInfo.Code)
            {
                case 0:
                    // 代表通讯正常
                    //BtnRFIDShow = Visibility.Hidden;
                    break;
                case 1:
                    // 得到卡内容
                    Order.RFID = rfidInfo.Data;
                    break;
                case -1:
                // 未找到发卡器
                case -2:
                    // 未知异常
                    BiuMessageBoxWindows.BiuShow(rfidInfo.Error);
                    //BtnRFIDShow = Visibility.Visible;
                    break;
                default:
                    Order.RFID = "";
                    //BtnRFIDShow = Visibility.Hidden;
                    break;
            }
        }
        #endregion

        public ICommand EditingCommand => new AnotherCommandImplementation(ExecuteEditingCommand); //进行修改编辑
        public ICommand CancelEditingCommand => new AnotherCommandImplementation(ExecuteCancelEditingCommand); //取消修改编辑
        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand); //提交修改
        public ICommand SubmitExitCommand => new AnotherCommandImplementation(ExecuteSubmitExitCommand); //提交已结账单据修改
        public ICommand MendPrintBillCommand => new AnotherCommandImplementation(ExecuteMendPrintBillCommand);
        public ICommand DeleteShipOrderCommand => new AnotherCommandImplementation(ExecuteDeleteShipOrderCommand);

        public void ExecuteEditingCommand(object o)
        {
            IsEditing = true;
        }
        public void ExecuteCancelEditingCommand(object o)
        {
            Order = Common.DeepCopy(OldOrder);
            IsEditing = false;
        }
        public void ExecuteSubmitCommand(object o)
        {
            if (Order.CustomerType == 1 && Order.Customer == null) { BiuMessageBoxWindows.BiuShow("请选择客户!", image: BiuMessageBoxImage.Warning); ; return; }
            //SubmitButtonIsEnabled = false;
            RequestStatus.StartRequest(async () =>
            {
                await ModelHelper.GetInstance().GetApiDataArg(ApiClient.ChangeEnterShipOrderAsync, Order,
                    delegate (DataInfo<ShipOrder> result)
                {
                    IsEditing = false;
                });
               
                //SubmitButtonIsEnabled = true;
                if (!IsEditing)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                }
                //打印票据
                try
                {
                    BillPrinter.GetInstance().PrintShip(Order);
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
                }
            });
        }
        public void ExecuteSubmitExitCommand(object o)
        {
            if (Order.EditReason.Equals(""))
            {
                BiuMessageBoxWindows.BiuShow("请选择修改原因!", image: BiuMessageBoxImage.Warning);
                return;
            }
            if (Order.CarNetWeight < 0.0 || Order.CarGrossWeight < 0.0 || Order.CarTare < 0.0)
            {
                BiuMessageBoxWindows.BiuShow("毛、皮、净重不可小于0!", image: BiuMessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Order.CarId) || Order.Goods == null) {
                BiuMessageBoxWindows.BiuShow("未填写车牌号或未选择料品!", image: BiuMessageBoxImage.Warning);
                return;
            }
            if (Order.CustomerType == 1 && Order.Customer == null) {
                BiuMessageBoxWindows.BiuShow("请选择客户!", image: BiuMessageBoxImage.Warning);
                return;
            }
            //SubmitButtonIsEnabled = false;
            RequestStatus.StartRequest(async () =>
            {
                var arg = new ApiDelegateArg<ShipOrder>(ApiClient.ChangeExitShipOrderAsync);
                await ModelHelper.GetInstance().GetApiDataArg(arg, Order, delegate (DataInfo<ShipOrder> result)
                {
                    IsEditing = false;
                });
               
                //SubmitButtonIsEnabled = true;
                if (!IsEditing)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                }

                //打印票据
                try
                {
                    BillPrinter.GetInstance().PrintShip(Order);
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message, image: BiuMessageBoxImage.Error);
                }
            });
        }
        private void ExecuteMendPrintBillCommand(object o)
        {
            try
            {
                MendPrintBtnEnabled = false;
                BillPrinter.GetInstance().PrintShip(Order);
                MendPrintBtnEnabled = true;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message,image:BiuMessageBoxImage.Error);
            }
            finally
            {
                MendPrintBtnEnabled = true;
            }
        }
        private void ExecuteDeleteShipOrderCommand(object o)
        {
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认作废该订单? 作废后不可恢复!", BiuMessageBoxButton.YesNo)))
            {
                RequestStatus.StartRequest(()=> {
                    var r = ModelHelper.GetInstance().GetApiDataArg(ApiClient.DeleteExitShipOrderAsync,new { ID = Order.ID}).Result;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            }
        }

        /// <summary>
        /// 获取当前料品的价格
        /// </summary>
        public void SetPriceByGoods(int sender = -1)
        {
            RequestStatus.StartRequest(() =>
            {
                try
                {
                    //SubmitButtonIsEnabled = false;
                    if (Order.Goods == null) throw new Exception("出错：料品为空!");
                    if (Order.Goods.ID == OldOrder.Goods.ID)
                    {
                        Order.GoodsPrice = OldOrder.GoodsPrice;
                        Order.GoodsRealPrice = OldOrder.GoodsRealPrice;
                    }
                    else
                    {
                        var apiDelegateArg = new ApiDelegateArg<ShipCustomerGoodsPrice>(ApiClient.GetShipCustomerGoodsPriceSingleAsync);
                        if (Order.CustomerType == 1 && Order.Customer != null)
                        {
                            var g = ModelHelper.GetInstance().GetApiDataArg(apiDelegateArg, new ShipCustomerGoodsPrice { GoodsId = Order.Goods.ID, CustomerId = Order.Customer.ID }).Result.Data;
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
                    //SubmitButtonIsEnabled = true;
                    if (sender != -1)
                        Calculate(sender);
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                    RequestStatus.CompleteOneRequest();
                }
            });
        }

        public void Calculate(int sender)
        {
            //Order.Calculate(sender);
            try
            {
                if (sender != 1 && IsEditing)
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
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
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
                //SubmitButtonIsEnabled = false;
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r) // 初始化时已经加上选中客户，后续再找到时，不再添加
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (!isFind && Order.Customer?.ID == item.ID) { }
                        else
                        {
                            CustomerItems.Add(item);
                        }
                    });
                }
                //SubmitButtonIsEnabled = true;
            });
        }

        /// <summary>
        /// 搜索出料客户
        /// </summary>
        public void GetShipCustomerItems(string SearchCusSeed)
        {
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<ShipCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetShipCustomerAsync,
                  new { szm = SearchCusSeed, Page = 0, Size = 500 },
                 delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
