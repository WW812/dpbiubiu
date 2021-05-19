using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
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

namespace biubiu.view_model.ship_order_manage
{
    public class PrecisionSearchShipOrderViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 搜索条件
        /// </summary>
        private SearchShipOrder _searchOrder;
        public SearchShipOrder SearchOrder
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
            set { _userItems = value;
                NotifyPropertyChanged("UserItems");
            }
        }

        /// <summary>
        /// 客户集合
        /// </summary>
        private ObservableCollection<ShipCustomer> _shipCustomerItems;
        public ObservableCollection<ShipCustomer> ShipCustomerItems
        {
            get { return _shipCustomerItems; }
            set
            {
                _shipCustomerItems = value;
                NotifyPropertyChanged("ShipCustomerItems");
            }
        }

        /// <summary>
        /// 料品集合
        /// </summary>
        private ObservableCollection<ShipGoods> _shipGoodsItems;
        public ObservableCollection<ShipGoods> ShipGoodsItems
        {
            get { return _shipGoodsItems; }
            set
            {
                _shipGoodsItems = value;
                NotifyPropertyChanged("ShipGoodsItems");
            }
        }

        private PageModel _customerPage = new PageModel();
        public PageModel CustomerPage
        {
            get { return _customerPage; }
            set
            {
                _customerPage = value;
                NotifyPropertyChanged("CustomerPage");
            }
        }

        public PrecisionSearchShipOrderViewModel(SearchShipOrder sso)
        {
            SearchOrder = sso;
            SearchOrder.Reset();
            GetUser();
            GetGoods();
            GetCustomer();
        }

        public void GetUser()
        {
            Task.Run(()=> {
                UserItems = new ObservableCollection<User>(ModelHelper.GetInstance().GetApiData(ApiClient.GetAllUserAsync).Result.Data);
            });
        }

        public void GetGoods()
        {
            Task.Run(()=> {
                ShipGoodsItems = new ObservableCollection<ShipGoods>(ModelHelper.GetInstance().GetApiData(ApiClient.GetGoodsAsync).Result.Data);
            });
        }

        public void GetCustomer(string szmCustomer = "")
        {
            if (szmCustomer.Equals("")) szmCustomer = null;
            Task.Run(() => {
                ShipCustomerItems = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetShipCustomerAsync,
                   new { szm = szmCustomer, Page = CustomerPage.Page - 1, Size = 500/*CustomerPage.Size*/ },
                   delegate (DataInfo<List<ShipCustomer>> result) { CustomerPage = result.Page; }).Result.Data);
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
                var r = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { Page = CustomerPage.Page - 1, Size = CustomerPage.Size },
                delegate (DataInfo<List<ShipCustomer>> result) { CustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ShipCustomerItems.Add(item);
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
