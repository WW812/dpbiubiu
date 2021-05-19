using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_order_manage
{
    public class PrecisionSearchStockOrderViewModel : INotifyPropertyChanged
    {
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
        /// 用户集合
        /// </summary>
        private ObservableCollection<User> _userItems;
        public ObservableCollection<User> UserItems
        {
            get { return _userItems; }
            set
            {
                _userItems = value;
                NotifyPropertyChanged("UserItems");
            }
        }

        /// <summary>
        /// 客户集合
        /// </summary>
        private ObservableCollection<StockCustomer> _stockCustomerItems;
        public ObservableCollection<StockCustomer> StockCustomerItems
        {
            get { return _stockCustomerItems; }
            set
            {
                _stockCustomerItems = value;
                NotifyPropertyChanged("StockCustomerItems");
            }
        }

        /// <summary>
        /// 料品集合
        /// </summary>
        private ObservableCollection<StockGoods> _stockGoodsItems;
        public ObservableCollection<StockGoods> StockGoodsItems
        {
            get { return _stockGoodsItems; }
            set
            {
                _stockGoodsItems = value;
                NotifyPropertyChanged("StockGoodsItems");
            }
        }

        private PageModel _customerPage = new PageModel();
        public PageModel CustomerPage
        {
            get { return _customerPage; }
            set { _customerPage = value;
                NotifyPropertyChanged("CustomerPage");
            }
        }

        public PrecisionSearchStockOrderViewModel(SearchStockOrder sso)
        {
            SearchOrder = sso;
            SearchOrder.Reset();
            GetUser();
            GetGoods();
            GetCustomer();
        }

        public void GetUser()
        {
            Task.Run(() => {
                UserItems = new ObservableCollection<User>(ModelHelper.GetInstance().GetApiData(new ApiDelegate<List<User>>(ApiClient.GetAllUserAsync)).Result.Data);
            });
        }

        public void GetGoods()
        {
            Task.Run(() => {
                StockGoodsItems = new ObservableCollection<StockGoods>(ModelHelper.GetInstance().GetApiData(ApiClient.GetStockGoodsAsync).Result.Data);
            });
        }

        public void GetCustomer()
        {
            Task.Run(() => {
                StockCustomerItems = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockCustomerAsync,
                    new { Page = CustomerPage.Page - 1, Size = CustomerPage.Size},
                    delegate(DataInfo<List<StockCustomer>> result) { CustomerPage = result.Page; }).Result.Data);
            });
        }

        public void NextPageCustomerItems(ComboBox lv)
        {
            if (CustomerPage.Last || lv.Items.IsEmpty)
            {
                return;
            }
            Task.Run(() =>
            {
                CustomerPage.Page++;
                var r = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetStockCustomerAsync,
                new { Page = CustomerPage.Page - 1, Size = CustomerPage.Size },
                delegate (DataInfo<List<StockCustomer>> result) { CustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        StockCustomerItems.Add(item);
                    });
                }
            });
        }

        public ICommand ResetSearchOrderCommand => new AnotherCommandImplementation(ExcuteResetSearchOrderCommand);

        private void ExcuteResetSearchOrderCommand(object o)
        {
            SearchOrder.Reset();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
