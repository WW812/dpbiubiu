using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.views.finance.accept;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace biubiu.view_model.accept
{
    public class AcceptViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 承兑集合
        /// </summary>
        private ObservableCollection<ShipCustomerMoney> _acceptItems;
        public ObservableCollection<ShipCustomerMoney> AcceptItems
        {
            get { return _acceptItems; }
            set
            {
                _acceptItems = value;
                NotifyPropertyChanged("AcceptItems");
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
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel();
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
        /// 编号搜索
        /// </summary>
        private string _acceptNumFeed;
        public string AcceptNumFeed
        {
            get { return _acceptNumFeed; }
            set
            {
                _acceptNumFeed = value;
                NotifyPropertyChanged("AcceptNumFeed");
            }
        }

        /// <summary>
        /// 金额搜索
        /// </summary>
        private double? _acceptMoneyFeed;
        public double? AcceptMoneyFeed
        {
            get { return _acceptMoneyFeed; }
            set
            {
                _acceptMoneyFeed = value;
                NotifyPropertyChanged("AcceptMoneyFeed");
            }
        }

        private ShipCustomer _acceptCustomerFeed;
        public ShipCustomer AcceptCustomerFeed
        {
            get { return _acceptCustomerFeed; }
            set { _acceptCustomerFeed = value;
                NotifyPropertyChanged("AcceptCustomerFeed");
            }
        }

        /// <summary>
        /// 承兑选择状态
        /// </summary>
        public int? SelectedAcceptStatus;

        public AcceptViewModel()
        {
            GetCustomerItems();
            GetAcceptItems();
        }

        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand RunEditAceeptCommand => new AnotherCommandImplementation(ExecuteRunEditAceeptCommand);

        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
            }
        }
        private async void ExecuteRunEditAceeptCommand(object o)
        {
            var view = new EditAcceptDialog {
                AcceptObject = o as ShipCustomerMoney
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingMoneyEventHandler);
        }

        public void GetCustomerItems(string SearchCusSeed = "")
        {
            Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<ShipCustomer>(
                    ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.GetShipCustomerAsync,
                    new { szm = SearchCusSeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size }).Result.Data);
            });
        }

        public void GetAcceptItems()
        {
            Task.Run(() =>
            {
                AcceptItems = new ObservableCollection<ShipCustomerMoney>(ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.GetAcceptsAsync,
                    new
                    {
                        HonourNum = string.IsNullOrWhiteSpace(AcceptNumFeed) ? null: AcceptNumFeed,
                        Money = AcceptMoneyFeed,
                        Customer = AcceptCustomerFeed,
                        HonourStatus = SelectedAcceptStatus,
                        Page = CurrentPage.Page - 1,
                        Size = CurrentPage.Size
                    },
                    delegate (DataInfo<List<ShipCustomerMoney>> result)
                    {
                        CurrentPage = result.Page;
                    }).Result.Data);
            });
        }

        /// <summary>
        /// 料款界面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingMoneyEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetAcceptItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
