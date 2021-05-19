using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.paytype;
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

namespace biubiu.view_model.customer.ship_customer
{
    public class SetShipCustomerMoneyViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 当前客户
        /// </summary>
        private ShipCustomer _currentCustomer;
        public ShipCustomer CurrentCustomer
        {
            get
            {
                return _currentCustomer;
            }
            set
            {
                _currentCustomer = value;
                NotifyPropertyChanged("CurrentCustomer");
            }
        }

        private ShipCustomerMoney _customerMoney;
        public ShipCustomerMoney CustomerMoney
        {
            get { return _customerMoney; }
            set
            {
                _customerMoney = value;
                NotifyPropertyChanged("CustomerMoney");
            }
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        private ObservableCollection<PayType> _payTypeItems;
        public ObservableCollection<PayType> PayTypeItems
        {
            get { return _payTypeItems; }
            set
            {
                _payTypeItems = value;
                NotifyPropertyChanged("PayTypeItems");
            }
        }

        /// <summary>
        /// 支付列表的分页
        /// </summary>
        private PageModel _currentPayTypePage;
        public PageModel CurrentPayTypePage
        {
            get { return _currentPayTypePage; }
            set
            {
                _currentPayTypePage = value;
                NotifyPropertyChanged("CurrentPayTypePage");
            }
        }

        /// <summary>
        /// 是否可以提交
        /// </summary>
        private bool _isSubmitEnabled = true;
        public bool IsSubmitEnabled
        {
            get { return _isSubmitEnabled; }
            set
            {
                _isSubmitEnabled = value;
                NotifyPropertyChanged("IsSubmitEnabled");
            }
        }

        public SetShipCustomerMoneyViewModel(ShipCustomer shipCustomer)
        {
            CurrentCustomer = shipCustomer;
            CustomerMoney = new ShipCustomerMoney { Customer = CurrentCustomer };
            GetPayTypeItems();
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExcuteSubmitCommand);
        public async void ExcuteSubmitCommand(object o)
        {
            if (!IsSubmitEnabled) return;

            IsSubmitEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(
                ModelHelper.ApiClient.SetShipCustomerMoneyAsync,
                CustomerMoney,
                delegate(DataInfo<ShipCustomerMoney> result) {
                    Application.Current.Dispatcher.Invoke(new Action(()=> {
                        CurrentCustomer.Balance += CustomerMoney.Money;
                        CurrentCustomer.Money += CustomerMoney.Money;
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            IsSubmitEnabled = true;
        }

        /// <summary>
        /// 获取支付类型
        /// </summary>
        public void GetPayTypeItems()
        {
            CurrentPayTypePage = new PageModel(20, 1);
            Task.Run(() =>
            {
                PayTypeItems = new ObservableCollection<PayType>(
                   ModelHelper.GetInstance().GetApiDataArg(
                       ApiClient.GetPayTypeAsync,
                       new { Page = CurrentPayTypePage.Page - 1, Size = CurrentPayTypePage.Size },
                       delegate (DataInfo<List<PayType>> result) { CurrentPayTypePage = result.Page; }
                       ).Result.Data);
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPayTypeItems(ComboBox lv)
        {
            if (CurrentPayTypePage.Last)
            {
                return;
            }
            Task.Run(()=> {
                CurrentPayTypePage.Page++;
                var r = ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetPayTypeAsync,
                new { Page = CurrentPayTypePage.Page - 1, Size = CurrentPayTypePage.Size },
                delegate (DataInfo<List<PayType>> result)
                {
                    CurrentPayTypePage = result.Page;
                    foreach (var item in result.Data)
                    {
                        lv.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            PayTypeItems.Add(item);
                        });
                    }
                },
                delegate (DataInfo<List<PayType>> result)
                {
                    CurrentPayTypePage.Page--;
                }).Result;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
