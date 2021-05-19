using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.print;
using biubiu.model.stock_order;
using MaterialDesignThemes.Wpf;
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

namespace biubiu.view_model.stock_order
{
    public class StockOrderDetailsViewModel : INotifyPropertyChanged
    {
        private StockOrder _order;
        public StockOrder Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                NotifyPropertyChanged("Order");
            }
        }

        /// <summary>
        /// 原单据对象
        /// </summary>
        private StockOrder _oldOrder;
        public StockOrder OldOrder
        {
            get { return _oldOrder; }
            set
            {
                _oldOrder = value;
                NotifyPropertyChanged("OldOrder");
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
        /// 料品集合
        /// </summary>
        private ObservableCollection<StockGoods> _goodsItems;
        public ObservableCollection<StockGoods> GoodsItems
        {
            get { return _goodsItems; }
            set
            {
                _goodsItems = value;
                NotifyPropertyChanged("GoodsItems");
            }
        }

        /// <summary>
        /// 请求状态
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

        /// <summary>
        /// 修改原因集合
        /// </summary>
        private ObservableCollection<string> _editReasonItems;
        public ObservableCollection<string> EditReasonItems
        {
            get { return _editReasonItems; }
            set
            {
                _editReasonItems = value;
                NotifyPropertyChanged("EditReasonItems");
            }
        }

        /// <summary>
        /// 抓拍图片地址集合
        /// </summary>
        /*
        private ObservableCollection<string> _pictureURLItems;
        public ObservableCollection<string> PictureURLItems
        {
            get { return _pictureURLItems; }
            set
            {
                _pictureURLItems = value;
                NotifyPropertyChanged("PictureURLItems");
            }
        }
        */

        /// <summary>
        /// 客户集合
        /// </summary>
        private ObservableCollection<StockCustomer> _customerItems;
        public ObservableCollection<StockCustomer> CustomerItems
        {
            get { return _customerItems; }
            set
            {
                _customerItems = value;
                NotifyPropertyChanged("CustomerItems");
            }
        }

        /// <summary>
        /// 客户滚动条页码
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
        /// 第一页客户分页是否含有 选中项
        /// </summary>
        private bool isFind = false;

        // 补印按钮是否可用
        private bool _mendPrintBtnEnabled = true;
        public bool MendPrintBtnEnabled
        {
            get { return _mendPrintBtnEnabled; }
            set
            {
                _mendPrintBtnEnabled = value;
                NotifyPropertyChanged("MendPrintBtnEnabled");
            }
        }

        public StockOrderDetailsViewModel(StockOrder order)
        {
            RequestStatus.StartRequest(() =>
            {
                OldOrder = Common.DeepCopy(order);
                Order = Common.DeepCopy(order);
                GoodsItems = new ObservableCollection<StockGoods>(
                    ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockGoodsAsync,
                    new StockGoods { Valid = 1 }).Result.Data);

                CustomerItems = new ObservableCollection<StockCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockCustomerAsync, new
                   {
                       Page = CurrentCustomerPage.Page - 1,
                       Size = CurrentCustomerPage.Size
                   }, delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);

                try
                {
                    foreach (var item in GoodsItems)
                    {
                        if (item.ID == Order.Goods.ID)
                        {
                            Order.Goods = item;
                        }
                    }

                    foreach (var item in CustomerItems)
                    {
                        if (item.ID == Order.Customer?.ID)
                        {
                            Order.Customer = item;
                            isFind = true;
                        }
                    }
                    if (!isFind)
                    {
                        CustomerItems.Add(Order.Customer);
                    }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                }
            });
            Task.Run(() =>
            {
                //获取图片地址
                /*
                PictureURLItems = new ObservableCollection<string>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetPictureURL,
                        new { id = order.ID }).Result.Data);
                        */
                EditReasonItems = new ObservableCollection<string> {
                    "1.皮重错误",
                    "2.毛重错误",
                    "3.扣吨错误",
                    "4.车号错误",
                    "5.料品选择错误",
                    "6.运费错误",
                    "7.客户选择错误",
                    "8.采购单价错误",
                    "9.其他"
                };
            });
        }

        public ICommand EditingCommand => new AnotherCommandImplementation(ExecuteEditingCommand); //进行修改编辑
        public ICommand CancelEditingCommand => new AnotherCommandImplementation(ExecuteCancelEditingCommand); //取消修改编辑
        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand); //提交修改
        public ICommand SubmitExitCommand => new AnotherCommandImplementation(ExecuteSubmitExitCommand); //提交已结账单据修改
        public ICommand PaymentCommand => new AnotherCommandImplementation(ExecutePaymentCommand);
        public ICommand MendPrintBillCommand => new AnotherCommandImplementation(ExecuteMendPrintBillCommand);
        public ICommand DeleteStockOrderCommand => new AnotherCommandImplementation(ExecuteDeleteStockOrderCommand); //作废单据

        public void ExecuteEditingCommand(object o)
        {
            IsEditing = true;
        }
        public void ExecuteCancelEditingCommand(object o)
        {
            Order = Common.DeepCopy(OldOrder);
            IsEditing = false;
        }
        public void ExecuteSubmitCommand(object o)
        {
            if (Order.CustomerType == 1 && Order.Customer == null) { BiuMessageBoxWindows.BiuShow("请选择客户!",image:BiuMessageBoxImage.Warning); return; }
            RequestStatus.StartRequest(async () =>
            {
                var arg = new ApiDelegateArg<StockOrder>(ApiClient.ChangeEnterStockOrderAsync);
                await ModelHelper.GetInstance().GetApiDataArg(arg, Order, delegate (DataInfo<StockOrder> result)
                {
                    IsEditing = false;
                });
                if (!IsEditing)
                {
                    await (o as UserControl).Dispatcher.BeginInvoke((Action)delegate ()
                     {
                         DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                     });
                }
            });
        }
        public void ExecuteSubmitExitCommand(object o)
        {
            if (Order.EditReason.Equals(""))
            {
                BiuMessageBoxWindows.BiuShow("请选择修改原因!",image:BiuMessageBoxImage.Warning);
                return;
            }
            if (Order.CarNetWeight < 0.0 || Order.CarGrossWeight < 0.0 || Order.CarTare < 0.0)
            {
                BiuMessageBoxWindows.BiuShow("毛、皮、净重不可小于0!",image:BiuMessageBoxImage.Warning);
                return;
            }
            if (Order.CarId == null || Order.Goods == null) { BiuMessageBoxWindows.BiuShow("未填写车牌号或未选择料品!",image:BiuMessageBoxImage.Error); return; }
            if (Order.CustomerType == 1 && Order.Customer == null) { BiuMessageBoxWindows.BiuShow("请选择客户!",image:BiuMessageBoxImage.Error); return; }
            RequestStatus.StartRequest(async () =>
            {
                var arg = new ApiDelegateArg<StockOrder>(ApiClient.ChangeExitStockOrderAsync);
                await ModelHelper.GetInstance().GetApiDataArg(arg, Order, delegate (DataInfo<StockOrder> result)
                {
                    IsEditing = false;
                });
                if (!IsEditing)
                {
                    await (o as UserControl).Dispatcher.BeginInvoke((Action)delegate ()
                     {
                         DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                     });
                }
            });
        }
        public void ExecutePaymentCommand(object o)
        {
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确定支付?", BiuMessageBoxButton.YesNo,BiuMessageBoxImage.Question)))
            {
                RequestStatus.StartRequest(() =>
                {
                    var a = ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.PaymentStockOrderAsync,
                        Order,
                        delegate (DataInfo<StockOrder> result)
                        {
                            Order.Paid = 1;
                        }).Result;
                });
            }
        }
        private void ExecuteMendPrintBillCommand(object o)
        {
            try
            {
                MendPrintBtnEnabled = false;
                BillPrinter.GetInstance().PrintStock(Order);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow("打印出错，请检查打印机设置或者连接!\n" + er.Message,image:BiuMessageBoxImage.Error);
            }
            finally
            {
                MendPrintBtnEnabled = true;
            }
        }
        private void ExecuteDeleteStockOrderCommand(object o)
        {
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确认作废该订单? 作废后不可逆!", BiuMessageBoxButton.YesNo,BiuMessageBoxImage.Warning)))
            {
                RequestStatus.StartRequest(() => {
                    var r = ModelHelper.GetInstance().GetApiDataArg(ApiClient.DeleteExitStockOrderAsync, new { ID = Order.ID }).Result.Data;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            }
        }

        /// <summary>
        /// 获取当前料品的价格
        /// </summary>
        public void SetPriceByGoods()
        {
            RequestStatus.StartRequest(() =>
            {
                try
                {
                    if (Order.Goods == null) throw new Exception("出错：料品为空!");
                    if (Order.Goods.ID == OldOrder.Goods.ID)
                    {
                        Order.GoodsRealPrice = OldOrder.GoodsRealPrice;
                    }
                    else
                    {
                        var apiDelegateArg = new ApiDelegateArg<StockCustomerGoodsPrice>(ApiClient.GetStockCustomerGoodsPriceSingleAsync);

                        if (Order.CustomerType == 1 && Order.Customer != null)
                        {
                            var g = ModelHelper.GetInstance().GetApiDataArg(apiDelegateArg, new StockCustomerGoodsPrice { GoodsId = Order.Goods.ID, CustomerId = Order.Customer.ID }).Result.Data;
                            Order.GoodsRealPrice = g.CustomerPrice;
                        }
                        else
                        {
                            var g = ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetStockGoodsByIdAsync, Order.Goods).Result.Data;
                            Order.GoodsRealPrice = g.RealPrice;
                        }
                    }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(ComboBox lv)
        {
            if (CurrentCustomerPage.Last || lv.Items.IsEmpty)
            {
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<StockCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetStockCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (!isFind && Order.Customer?.ID == item.ID) { }
                        else
                        {
                            CustomerItems.Add(item);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 搜索出料客户
        /// </summary>
        public void GetShipCustomerItems(string SearchCusSeed)
        {
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                CustomerItems = new ObservableCollection<StockCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetStockCustomerAsync,
                  new { szm = SearchCusSeed, Page = 0, Size = 500 },
                 delegate (DataInfo<List<StockCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
