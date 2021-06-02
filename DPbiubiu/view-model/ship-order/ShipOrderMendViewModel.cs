using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
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

namespace biubiu.view_model.ship_order
{
    public class ShipOrderMendViewModel : INotifyPropertyChanged
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
        private ShipOrder _order;
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
        /// 当前页码
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
        /// 是否连续创建,主要作用是不关闭窗口
        /// </summary>
        private Boolean _isCreateAgain;
        public Boolean IsCreateAgain
        {
            get { return _isCreateAgain; }
            set
            {
                _isCreateAgain = value;
                NotifyPropertyChanged("IsCreateAgain");
            }
        }

        public ShipOrderMendViewModel()
        {
            Order = new ShipOrder { Status = 1 };
            GetCustomerItems();
            GetGoodsItems();
        }

        public ICommand SubmitOrderCommand => new AnotherCommandImplementation(ExecuteSubmitOrderCommand);
        private async void ExecuteSubmitOrderCommand(object o)
        {
            if (Order.CarNetWeight < 0.0) { BiuMessageBoxWindows.BiuShow("净重不可小于0!",image:BiuMessageBoxImage.Warning); return; }
            if (string.IsNullOrWhiteSpace(Order.CarId) || Order.Goods == null)
            {
                BiuMessageBoxWindows.BiuShow("未填写车牌号或未选择料品!",image:BiuMessageBoxImage.Warning);
                return;
            }
            if (Order.CustomerType == 1 && Order.Customer == null)
            {
                BiuMessageBoxWindows.BiuShow("请选择客户!", image: BiuMessageBoxImage.Warning);
                return;
            }
            RequestStatus.AddOneRequest();
            try
            {
                var Result = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.MendShipOrderAsync, Order);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    if (!IsCreateAgain)
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    else
                    {
                        Order.Reset();
                        Order.Status = 1;
                    }
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

        public void GetCustomerItems()
        {
            RequestStatus.StartRequest(() =>
            {
                CustomerItems = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetShipCustomerAsync,
                    new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                    delegate (DataInfo<List<ShipCustomer>> result ) { CurrentCustomerPage = result.Page; }).Result.Data);
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
            Task.Run(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        CustomerItems.Add(item);
                    });
                }
            });
        }

        public void Calculate(int sender)
        {
            //Order.Calculate(sender);
            try
            {
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
                                Order.DiscountMoney = Common.Double2DecimalCalculate(Order.OrderMoney % 10);
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

        public void GetGoodsItems()
        {
            RequestStatus.StartRequest(() =>
            {
                GoodsItems = new ObservableCollection<ShipGoods>(ModelHelper.GetInstance().GetApiDataArg(new ApiDelegateArg<List<ShipGoods>>(ApiClient.GetGoodsAsync),
                       new ShipGoods { Valid = 1, Deleted = 0 }).Result.Data);
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
        /// 获取当前料品的价格
        /// </summary>
        public void SetPriceByGoods(int sender = -1)
        {
            RequestStatus.StartRequest(() =>
            {
                try
                {
                    if (Order.Goods == null) return;
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
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetGoodsByIdAsync,Order.Goods).Result.Data;
                        Order.GoodsPrice = g.Price;
                        Order.GoodsRealPrice = g.RealPrice;
                    }
                    if (sender != -1)
                        Calculate(sender);
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
