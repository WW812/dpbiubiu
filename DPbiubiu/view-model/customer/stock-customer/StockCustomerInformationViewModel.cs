using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.views.marketing.customer.stock_customer;
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

namespace biubiu.view_model.customer.stock_customer
{
    public class StockCustomerInformationViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 客户model
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
        /// 原客户对象
        /// </summary>
        private StockCustomer _oldCustomer;
        public StockCustomer OldCustomer
        {
            get { return _oldCustomer; }
            set
            {
                _oldCustomer = value;
                NotifyPropertyChanged("OldCustomer");
            }
        }

        /// <summary>
        /// 是否处于修改状态
        /// </summary>
        private Boolean _isEditing = false;
        public Boolean IsEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                _isEditing = value;
                NotifyPropertyChanged("IsEditing");
            }
        }

        /// <summary>
        /// 判断当前接口是否走完，小于0还有数据在加载
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

        public StockCustomerInformationViewModel(StockCustomer customer)
        {
            Task.Run(() =>
            {
                OldCustomer = Common.DeepCopy(customer);
                Customer = Common.DeepCopy(customer);
            });
        }

        public ICommand EditingCommand => new AnotherCommandImplementation(ExecuteEditingCommand); //进行修改编辑
        public ICommand CancelEditingCommand => new AnotherCommandImplementation(ExecuteCancelEditingCommand); //取消修改编辑
        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand); //提交修改

        private void ExecuteSubmitCommand(object o)
        {
            /*
            RequestStatus.AddOneRequest();
            try
            {
                var r = await ApiClient.ChangeStockCustomerAsync(Customer);
                if (r.Code != 200)
                {
                    throw new Exception(r.Msg);
                }else
                {
                    IsEditing = false;
                    DialogHost.CloseDialogCommand.Execute(false, o as StockCustomerInformationDialog);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
            }
            RequestStatus.CompleteOneRequest();
            */
            RequestStatus.StartRequest(()=> {
                var r = ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.ChangeStockCustomerAsync,
                    Customer,
                    delegate(DataInfo<StockCustomer> result) {
                        Application.Current.Dispatcher.Invoke(new Action(()=> {
                            IsEditing = false;
                            DialogHost.CloseDialogCommand.Execute(false, o as StockCustomerInformationDialog);
                        }));
                    }).Result;
            });
        }

        private void ExecuteEditingCommand(object o)
        {
            IsEditing = true;
        }
        private void ExecuteCancelEditingCommand(object o)
        {
            Customer = Common.DeepCopy(OldCustomer);
            IsEditing = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
