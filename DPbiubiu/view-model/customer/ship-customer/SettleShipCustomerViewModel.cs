using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace biubiu.view_model.customer.ship_customer
{
    public class SettleShipCustomerViewModel : INotifyPropertyChanged
    {
        public ShipCustomer _customer;

        public long? _dateTime; //结算日期

        /// <summary>
        /// 总计描述
        /// </summary>
        private string _detail;
        public string Detail
        {
            get { return _detail; }
            set
            {
                _detail = value;
                NotifyPropertyChanged("Detail");
            }
        }

        private bool _submitBtnEnabled = true;
        public bool SubmitBtnEnabled
        {
            get { return _submitBtnEnabled; }
            set
            {
                _submitBtnEnabled = value;
                NotifyPropertyChanged("SubmitBtnEnabled");
            }
        }

        /// <summary>
        /// 是否继承余额
        /// </summary>
        private bool _inherit;
        public bool Inherit
        {
            get { return _inherit; }
            set {
                _inherit = value;
                NotifyPropertyChanged("Inherit");
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

        public SettleShipCustomerViewModel(ShipCustomer c, long? d)
        {
            _customer = c;
            _dateTime = d;
            GetDetail(d);
        }

        public ICommand SettleCustomerCommand => new AnotherCommandImplementation(ExecuteSettleCustomerCommand);

        private void ExecuteSettleCustomerCommand(object o)
        {
            var msg = "确定结算此客户？";
            if (!BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow(msg, BiuMessageBoxButton.YesNo))) return;
            RequestStatus.StartRequest(() =>
            {
                var r = ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.SettleShipCustomerAsync, new { inherit = Inherit?1:0 },
                    delegate(DataInfo<object> result) {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                        }));
                    }).Result;
            });
        }

        public void GetDetail(long? date)
        {
            RequestStatus.StartRequest(() =>
            {
                var o = ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.GetSettleShipCustomerAsync,
                    new { id = _customer.ID, CreateTimeEnd = date }/*,
                    apiFaild:delegate (object obj)
                    {
                        RequestStatus.CompleteOneRequest();
                    }*/).Result.Data;
                var str = @"时间： " + Common.TimeStamp2DateTime(o.Statistics.SettleStartTime).ToString("yyyy-MM-dd HH:mm") + " 至 " + (date == null ? DateTime.Now : Common.TimeStamp2DateTime(date ?? 0)).ToString("yyyy-MM-dd HH:mm") + "\n\n";
                str += @"客户： " + o.Statistics.Name + "\n\n";
                str += @"预付款： " + o.Statistics.Money + "\n\n";
                str += @"料款： " + o.Statistics.UseMoney + "\n\n";
                str += @"余额： " + (o.Statistics.Money - o.Statistics.UseMoney) ;
                Detail = str;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
