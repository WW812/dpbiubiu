using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.stock_order;
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

namespace biubiu.view_model.stock_order
{
    public class StockOrderMendViewModel : INotifyPropertyChanged
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

        public StockOrderMendViewModel()
        {
            Order = new StockOrder { Status = 1 };
            GetCustomerItems();
            GetGoodsItems();
        }

        public ICommand SubmitOrderCommand => new AnotherCommandImplementation(ExecuteSubmitOrderCommand);
        private async void ExecuteSubmitOrderCommand(object o)
        {
            if (Order.CarNetWeight < 0.0) { BiuMessageBoxWindows.BiuShow("净重不可小于0!", image: BiuMessageBoxImage.Warning); return; }
            if (string.IsNullOrWhiteSpace(Order.CarId) || Order.Goods == null)
            {
                BiuMessageBoxWindows.BiuShow("未填写车牌号或未选择料品!", image: BiuMessageBoxImage.Warning);
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
                var Result = await ModelHelper.GetInstance().GetApiDataArg(ApiClient.MendStockOrderAsync, Order);
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
            RequestStatus.CompleteOneRequest();
        }

        /// <summary>
        /// 获取客户
        /// </summary>
        public void GetCustomerItems(string SearchCusSeed = "")
        {
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetStockCustomerAsync,
                    //new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                    new { szm = SearchCusSeed, Page = 0, Size = 500 },
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
            Task.Run(() =>
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

        public void GetGoodsItems()
        {
            RequestStatus.StartRequest(() =>
            {
                GoodsItems = new ObservableCollection<StockGoods>(ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockGoodsAsync,
                       new StockGoods { Valid = 1 }).Result.Data);
            });
        }

        /// <summary>
        /// 获取当前料品的价格
        /// </summary>
        public void SetPriceByGoods()
        {
            RequestStatus.StartRequest(() =>
            {
                try
                {
                    if (Order.Goods == null) return;
                    var apiDelegateArg = new ApiDelegateArg<StockCustomerGoodsPrice>(ApiClient.GetStockCustomerGoodsPriceSingleAsync);
                    if (Order.CustomerType == 1 && Order.Customer != null)
                    {
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockCustomerGoodsPriceSingleAsync, new StockCustomerGoodsPrice { GoodsId = Order.Goods.ID, CustomerId = Order.Customer.ID }).Result.Data;
                        Order.GoodsRealPrice = g.CustomerPrice;
                    }
                    else
                    {
                        var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockGoodsByIdAsync, Order.Goods).Result.Data;
                        Order.GoodsRealPrice = g.RealPrice;
                    }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                }
            });
        }

        public void Calculate(int sender)
        {
            Order.Calculate(sender);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
