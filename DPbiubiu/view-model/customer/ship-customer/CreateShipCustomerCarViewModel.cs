using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.ship_customer;
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

namespace biubiu.view_model.customer.ship_customer
{
    public class CreateShipCustomerCarViewModel : INotifyPropertyChanged
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
        /// 客户车辆Model
        /// </summary>
        private ShipCustomerCar _car;
        public ShipCustomerCar Car
        {
            get
            {
                return _car;
            }
            set
            {
                _car = value;
                NotifyPropertyChanged("Car");
            }
        }

        /// <summary>
        /// 是否继续添加
        /// </summary>
        private Boolean _isCreateAgain;
        public Boolean IsCreateAgain
        {
            get
            {
                return _isCreateAgain;
            }
            set
            {
                _isCreateAgain = value;
                NotifyPropertyChanged("IsCreateAgain");
            }
        }

        /// <summary>
        /// 提交按钮是否可用
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

        public CreateShipCustomerCarViewModel(ShipCustomer shipCustomer)
        {
            CurrentCustomer = shipCustomer;
            Car = new ShipCustomerCar { Customer = CurrentCustomer };
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExcuteSubmit);

        private async void ExcuteSubmit(object o)
        {
            /*
            try
            {
                ButtonIsEnabled = false;
                var Result = await ModelHelper.ApiClient.CreateShipCustomerCarAsync(Car);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    Car = new ShipCustomerCar { Customer = CurrentCustomer };
                    if (!IsCreateAgain)
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    else ButtonIsEnabled = true;
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                ButtonIsEnabled = true;
            }
            */
            ButtonIsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(
                ModelHelper.ApiClient.CreateShipCustomerCarAsync,
                Car,
                delegate (DataInfo<ShipCustomerCar> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Car = new ShipCustomerCar { Customer = CurrentCustomer };
                        if (!IsCreateAgain)
                            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                        else ButtonIsEnabled = true;
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
