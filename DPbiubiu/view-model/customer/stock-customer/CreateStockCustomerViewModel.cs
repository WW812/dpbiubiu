using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebApiClient;

namespace biubiu.view_model.customer.stock_customer
{
    public class CreateStockCustomerViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 出料客户
        /// </summary>
        private StockCustomer _customer;
        public StockCustomer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        /// <summary>
        /// 按钮是否可用
        /// </summary>
        private Boolean _buttonIsEnabled = true;
        public Boolean ButtonIsEnabled
        {
            get { return _buttonIsEnabled; }
            set
            {
                _buttonIsEnabled = value;
                NotifyPropertyChanged("ButtonIsEnabled");
            }
        }

        public CreateStockCustomerViewModel()
        {
            Customer = new StockCustomer();
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmit);

        private async void ExecuteSubmit(object o)
        {
            /*
            try
            {
                ButtonIsEnabled = false;
                if (Customer.Name.Equals("散户") || Customer.Name.Equals("现金流水"))
                {
                    throw new Exception("“" + Customer.Name + "”为系统关键字,不可作为客户名称!");
                }
                var Result = await ModelHelper.ApiClient.CreateStockCustomerAsync(Customer);
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
                ButtonIsEnabled = true;
            }
            */
            ButtonIsEnabled = false;
            if (Customer.Name.Equals("散户") || Customer.Name.Equals("现金流水"))
            {
                BiuMessageBoxWindows.BiuShow("“" + Customer.Name + "”为系统关键字,不可作为客户名称!", image: BiuMessageBoxImage.Error);
                return;
            }
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateStockCustomerAsync,
                Customer,
                delegate (DataInfo<StockCustomer> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            ButtonIsEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
