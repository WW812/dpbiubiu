using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
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

namespace biubiu.view_model.customer.ship_customer
{
    public class EditOrderForCusViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel(15);
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
        /// 当前选中的客户
        /// </summary>
        private ShipCustomer _currentShipCustomer;
        public ShipCustomer CurrentShipCustomer
        {
            get
            {
                return _currentShipCustomer;
            }
            set
            {
                _currentShipCustomer = value;
                NotifyPropertyChanged("CurrentShipCustomer");
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

        private ObservableCollection<ShipOrder> _shipCustomerOrderItems;
        public ObservableCollection<ShipOrder> ShipCustomerOrderItems
        {
            get { return _shipCustomerOrderItems; }
            set
            {
                _shipCustomerOrderItems = value;
                NotifyPropertyChanged("ShipCustomerOrderItems");
            }
        }

        public Dictionary<string, ShipOrder> ChangeOrders = new Dictionary<string, ShipOrder>();

        /// <summary>
        /// 进度条进度值 没有使用
        /// </summary>
        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                NotifyPropertyChanged("ProgressValue");
            }
        }

        private Visibility _showVisibility = Visibility.Hidden;
        public Visibility ShowVisibility
        {
            get { return _showVisibility; }
            set
            {
                _showVisibility = value;
                NotifyPropertyChanged("ShowVisibility");
            }
        }

        public EditOrderForCusViewModel(ShipCustomer customer)
        {
            CurrentShipCustomer = customer;
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand);

        private void ExecuteSubmitCommand(object o)
        {
            if (0 == ChangeOrders.Count)
            {
                BiuMessageBoxWindows.BiuShow("没有需要提交的修改的单据!");
                return;
            }
            if (BiuMessageBoxResult.No.Equals(BiuMessageBoxWindows.BiuShow("确定提交批量修改?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question))) return;
            RequestStatus.StartRequest(async () =>
            {
                await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.ChangeOrderForCus,
                   ChangeOrders.Values.ToList(),
                   delegate (DataInfo<object> result)
                   {
                       ChangeOrders?.Clear();
                       BiuMessageBoxWindows.BiuShow("修改成功!");
                   },
                   delegate (DataInfo<object> result)
                   {
                       BiuMessageBoxWindows.BiuShow("修改失败，请重试!", image: BiuMessageBoxImage.Warning);
                   });
            });
        }

        /// <summary>
        /// 获取出料客户的出料订单
        /// </summary>
        public void GetShipCustomerOrderItems()
        {
            CurrentPage.Reset(15);
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerOrderItems = new ObservableCollection<ShipOrder>(ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.GetShipOrderByShipCustomerAsync,
                        new ShipOrder { Customer = CurrentShipCustomer, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                        delegate (DataInfo<List<ShipOrder>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        public void NextPageOrderItems(DataGrid dg)
        {
            if (CurrentPage.Last)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                return;
            }
            RequestStatus.StartRequest(async () =>
            {
                CurrentPage.Page++;
                await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetShipOrderByShipCustomerAsync,
                new
                {
                    Customer = CurrentShipCustomer,
                    Page = CurrentPage.Page - 1,
                    Size = CurrentPage.Size
                },
                delegate (DataInfo<List<ShipOrder>> result)
                {
                    CurrentPage = result.Page;
                    foreach (var item in result.Data)
                    {
                        dg.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            ShipCustomerOrderItems.Add(item);
                        });
                    }
                },
                delegate (DataInfo<List<ShipOrder>> result)
                {
                    CurrentPage.Page--;
                });
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
