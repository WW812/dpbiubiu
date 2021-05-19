using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.views.marketing.customer.ship_customer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.customer.ship_customer
{
    public class ShipCustomerDetailsViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 当前客户
        /// </summary>
        private ShipCustomer _currentCustomer;
        public ShipCustomer CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
                NotifyPropertyChanged("CurrentCustomer");
            }
        }

        private ShipCustomer _old;
        public ShipCustomer Old
        {
            get { return _old; }
            set
            {
                _old = value;
                NotifyPropertyChanged("Old");
            }
        }

        /// <summary>
        /// 是否处于修改状态
        /// </summary>
        private Boolean _isEditing = false;
        public Boolean IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                NotifyPropertyChanged("IsEditing");
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

        public ShipCustomerDetailsViewModel(ShipCustomer shipCustomer)
        {
            Task.Run(() =>
            {
                SubmitButtonIsEnabled = false;
                Old = Common.DeepCopy(shipCustomer);
                CurrentCustomer = Common.DeepCopy(shipCustomer);
                SubmitButtonIsEnabled = true;
            });
            CurrentCustomer = shipCustomer;
        }

        public ICommand EditingCommand => new AnotherCommandImplementation(ExcuteEditingCommand); //进行修改编辑
        public ICommand CancelEditingCommand => new AnotherCommandImplementation(ExcuteCancelEditingCommand); //取消修改编辑
        public ICommand SubmitCommand => new AnotherCommandImplementation(ExcuteSubmitCommandAsync); //提交修改

        public void ExcuteEditingCommand(object o)
        {
            IsEditing = true;
        }
        public void ExcuteCancelEditingCommand(object o)
        {
            CurrentCustomer = Common.DeepCopy(Old);
            IsEditing = false;
        }
        public async void ExcuteSubmitCommandAsync(object o)
        {
            SubmitButtonIsEnabled = false;
            /*
                try
                {
                    var r = await ApiClient.ChangeShipCustomerAsync(CurrentCustomer);
                    if (r.Code != 200)
                    {
                        throw new Exception(r.Msg);
                    }
                    else
                    {
                        IsEditing = false;
                        DialogHost.CloseDialogCommand.Execute(false, o as ShipCustomerDetailsDialog);
                    }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                }
                */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.ChangeShipCustomerAsync,
                CurrentCustomer,
                delegate(DataInfo<ShipCustomer> result) {
                    Application.Current.Dispatcher.Invoke(new Action(()=> {
                        IsEditing = false;
                        DialogHost.CloseDialogCommand.Execute(false, o as ShipCustomerDetailsDialog);
                    }));
                });
            SubmitButtonIsEnabled = true;
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
