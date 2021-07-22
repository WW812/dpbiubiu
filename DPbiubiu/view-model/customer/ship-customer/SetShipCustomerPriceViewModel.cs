using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.ship_customer;
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
    public class SetShipCustomerPriceViewModel : INotifyPropertyChanged
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

        /// <summary>
        /// 客户料品价格集合
        /// </summary>
        private ObservableCollection<ShipCustomerGoodsPrice> _shipCustomerGoodsPriceItems;
        public ObservableCollection<ShipCustomerGoodsPrice> ShipCustomerGoodsPriceItems
        {
            get
            {
                return _shipCustomerGoodsPriceItems;
            }
            set
            {
                _shipCustomerGoodsPriceItems = value;
                NotifyPropertyChanged("ShipCustomerGoodsPriceItems");
            }
        }

        /// <summary>
        /// 提交按钮是否可用
        /// </summary>
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


        public SetShipCustomerPriceViewModel(ShipCustomer shipCustomer)
        {
            CurrentCustomer = shipCustomer;
            SetShipCustomerGoodsPriceItems();
        }

        /// <summary>
        /// 获取客户出料价格列表
        /// </summary>
        private void SetShipCustomerGoodsPriceItems()
        {
            Task.Run(() =>
            {
                ShipCustomerGoodsPriceItems = new ObservableCollection<ShipCustomerGoodsPrice>(
                   ModelHelper.GetInstance().GetApiDataArg(new ApiDelegateArg<List<ShipCustomerGoodsPrice>>(
                       ApiClient.GetShipCustomerGoodsPriceAsync),
                       new ShipCustomerGoodsPrice { CustomerId = CurrentCustomer.ID,Size = 500}).Result.Data);
            });
        }

        /// <summary>
        /// 统一设置优惠额度
        /// </summary>
        public void SetAllDiscountmoney(double discountmoney)
        {
            foreach (var item in ShipCustomerGoodsPriceItems)
            {
                item.DiscountMoney = discountmoney;
            }
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand);

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteSubmitCommand(object o)
        {
            SubmitButtonIsEnabled = false;
            /*
            var param = ShipCustomerGoodsPriceItems.ToList();
            try
            {
                var Result = await ApiClient.SetShipCustomerPriceAsync(param);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                SubmitButtonIsEnabled = true;
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.SetShipCustomerPriceAsync,
                ShipCustomerGoodsPriceItems.ToList(),
                delegate (DataInfo<object> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            SubmitButtonIsEnabled = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
