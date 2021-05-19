using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_order;
using biubiu.views.marketing.customer.ship_customer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebApiClient;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.customer.ship_customer
{
    public class ShipCustomerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 出料客户集合
        /// </summary>
        private ObservableCollection<ShipCustomer> _shipCustomerItems;
        public ObservableCollection<ShipCustomer> ShipCustomerItems
        {
            get
            {
                return _shipCustomerItems;
            }
            set
            {
                _shipCustomerItems = value;
                NotifyPropertyChanged("ShipCustomerItems");
            }
        }

        /// <summary>
        /// 客户车辆
        /// </summary>
        private ObservableCollection<ShipCustomerCar> _shipCustomerCarItems;
        public ObservableCollection<ShipCustomerCar> ShipCustomerCarItems
        {
            get
            {
                return _shipCustomerCarItems;
            }
            set
            {
                _shipCustomerCarItems = value;
                NotifyPropertyChanged("ShipCustomerCarItems");
            }
        }

        /// <summary>
        /// 客户单据集合
        /// </summary>
        private ObservableCollection<ShipOrder> _shipCustomerOrderItems;
        public ObservableCollection<ShipOrder> ShipCustomerOrderItems
        {
            get
            {
                return _shipCustomerOrderItems;
            }
            set
            {
                _shipCustomerOrderItems = value;
                NotifyPropertyChanged("ShipCustomerOrderItems");
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
        /// 客户金额集合
        /// </summary>
        private ObservableCollection<ShipCustomerMoney> _shipCustomerMoneyItems;
        public ObservableCollection<ShipCustomerMoney> ShipCustomerMoneyItems
        {
            get { return _shipCustomerMoneyItems; }
            set
            {
                _shipCustomerMoneyItems = value;
                NotifyPropertyChanged("ShipCustomerMoneyItems");
            }
        }

        /// <summary>
        /// 客户结算记录集合
        /// </summary>
        private ObservableCollection<ShipCustomerSettle> _shipCustomerSettleItems;
        public ObservableCollection<ShipCustomerSettle> ShipCustomerSettleItems
        {
            get { return _shipCustomerSettleItems; }
            set
            {
                _shipCustomerSettleItems = value;
                NotifyPropertyChanged("ShipCustomerSettleItems");
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
        /// tabpanel SelectedIndex
        /// 0 单价 1车辆 2单据
        /// </summary>
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyPropertyChanged("SelectedIndex");
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

        /// <summary>
        /// 底部弹窗
        /// </summary>
        /*
        private SnackbarViewModel _messageBar = new SnackbarViewModel();
        public SnackbarViewModel MessageBar
        {
            get { return _messageBar; }
            set
            {
                _messageBar = value;
                NotifyPropertyChanged("MessageBar");
            }
        }
        */

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel(16);
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
        private PageModel _currentCustomerPage;// = new PageModel();
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
        /// 导出客户详单开始日期
        /// </summary>
        private DateTime? _exportOrderStartDate = null;
        public DateTime? ExportOrderStartDate
        {
            get { return _exportOrderStartDate; }
            set
            {
                _exportOrderStartDate = value;
                NotifyPropertyChanged("ExportOrderStartDate");
            }
        }

        /// <summary>
        /// 导出客户详单开始时间
        /// </summary>
        private DateTime _exportOrderStartTime = new DateTime();
        public DateTime ExportOrderStartTime
        {
            get { return _exportOrderStartTime; }
            set
            {
                _exportOrderStartTime = value;
                NotifyPropertyChanged("ExportOrderStartTime");
            }
        }

        /// <summary>
        /// 导出客户详单结束日期
        /// </summary>
        private DateTime? _exportOrderEndDate = null;
        public DateTime? ExportOrderEndDate
        {
            get { return _exportOrderEndDate; }
            set
            {
                _exportOrderEndDate = value;
                NotifyPropertyChanged("ExportOrderEndDate");
            }
        }

        /// <summary>
        /// 导出客户详单结束时间
        /// </summary>
        private DateTime _exportOrderEndTime = new DateTime();
        public DateTime ExportOrderEndTime
        {
            get { return _exportOrderEndTime; }
            set
            {
                _exportOrderEndTime = value;
                NotifyPropertyChanged("ExportOrderEndTime");
            }
        }

        /// <summary>
        /// 结算日期
        /// </summary>
        private DateTime? _settleDate = null;
        public DateTime? SettleDate
        {
            get { return _settleDate; }
            set
            {
                _settleDate = value;
                NotifyPropertyChanged("SettleDate");
            }
        }

        /// <summary>
        /// 结算时间
        /// </summary>
        private DateTime _settleTime = DateTime.Now;
        public DateTime SettleTime
        {
            get { return _settleTime; }
            set
            {
                _settleTime = value;
                NotifyPropertyChanged("SettleTime");
            }
        }

        /// <summary>
        /// 搜索客户输入框输入字段
        /// </summary>
        private string _searchCusSeed = null;
        public string SearchCusSeed
        {
            get { return _searchCusSeed; }
            set
            {
                _searchCusSeed = value;
                NotifyPropertyChanged("SearchCusSeed");
            }
        }

        /// <summary>
        /// 是不是删除客户
        /// </summary>
        private bool _deletedCustomer = false;
        public bool DeletedCustomer
        {
            get
            {
                return _deletedCustomer;
            }

            set
            {
                _deletedCustomer = value;
                NotifyPropertyChanged("DeletedCustomer");
            }
        }

        public ShipCustomerViewModel()
        {
            GetShipCustomerItems();
        }

        /// <summary>
        /// 拉取出料客户
        /// </summary>
        public void GetShipCustomerItems()
        {
            CurrentCustomerPage = new PageModel { Page = 1, Size = 25 };
            ShipCustomerItems?.Clear();
            RequestStatus.StartRequest(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchCusSeed)) SearchCusSeed = null;
                ShipCustomerItems = new ObservableCollection<ShipCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetShipCustomerAsync,
                  new { szm = SearchCusSeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size, Deleted = DeletedCustomer },
                 delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(DataGrid lv)
        {
            if (CurrentCustomerPage.Last)
            {
                Task.Run(() =>
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                });
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { szm = SearchCusSeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size, Deleted = DeletedCustomer },
                delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
                foreach (var item in r)
                {
                    lv.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ShipCustomerItems.Add(item);
                    });
                }
            });
        }

        public ICommand RunCreateShipCustomerDialogCommand => new AnotherCommandImplementation(ExecuteRunCreateShipCustomerDialog);
        public ICommand RunCreateShipCustomerCarDialogCommand => new AnotherCommandImplementation(ExecuteRunCreateShipCustomerCarDialog);
        public ICommand RunSetShipCustomerPriceDialogCommand => new AnotherCommandImplementation(ExecuteRunSetShipCustomerDialog);
        public ICommand RunSetShipCustomerMoneyDialogCommand => new AnotherCommandImplementation(ExecuteRunSetShipCustomerMoneyDialogCommand);
        public ICommand DeleteShipCustomerCarDialogCommand => new AnotherCommandImplementation(ExecuteDeleteShipCustomerCarDialogCommand);
        public ICommand RunShipCustomerDetailCommand => new AnotherCommandImplementation(ExecuteRunShipCustomerDetailCommand);
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand ExportCusOrderDetailReportCommand => new AnotherCommandImplementation(ExecuteExportCusOrderDetailReportCommand);
        public ICommand VoidOneMoneyCommand => new AnotherCommandImplementation(ExecuteVoidOneMoneyCommand);
        public ICommand RunStatementWindowCommand => new AnotherCommandImplementation(ExecuteRunStatementWindowCommand);
        public ICommand SearchCustomerCommand => new AnotherCommandImplementation(ExecuteSearchCustomerCommand);
        public ICommand RunSettleCustomerWindowCommand => new AnotherCommandImplementation(ExecuteRunSettleCustomerWindowCommand);
        public ICommand ExportSettleCommand => new AnotherCommandImplementation(ExecuteExportSettleCommand);
        public ICommand RunEditOrderForCusDialogCommand => new AnotherCommandImplementation(ExecuteRunEditOrderForCusDialogCommand);
        public ICommand DeleteCustomerCommand => new AnotherCommandImplementation(ExecuteDeleteCustomerCommand);

        /// <summary>
        /// 开启新增客户Dialog(出料)
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunCreateShipCustomerDialog(object o)
        {
            var view = new CreateShipCustomerDialog();
            var result = await DialogHost.Show(view, "RootDialog", ClosingCustomerEventHandler);
        }
        /// <summary>
        /// 开启新增客户车辆Dialog(出料)
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunCreateShipCustomerCarDialog(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            var view = new CreateShipCustomerCarDialog
            {
                DataContext = new CreateShipCustomerCarViewModel(CurrentShipCustomer)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingCarEventHandler);
        }
        /// <summary>
        /// 删除客户车辆
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteDeleteShipCustomerCarDialogCommand(object o)
        {
            if (BiuMessageBoxResult.Cancel.Equals(BiuMessageBoxWindows.BiuShow("确定删除该车辆?", BiuMessageBoxButton.OKCancel, BiuMessageBoxImage.Question))) return;
            /*
            try
            {
                var Result = await ModelHelper.ApiClient.DeleteShipCustomerCarAsync(o as ShipCustomerCar);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    SetShipCustomerCarItems();
                    SnackbarViewModel.GetInstance().PoupMessageAsync("删除成功!");
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.DeleteShipCustomerCarAsync,
                o as ShipCustomerCar,
                delegate (DataInfo<ShipCustomerCar> result)
                {
                    SetShipCustomerCarItems();
                    SnackbarViewModel.GetInstance().PoupMessageAsync("删除成功!");
                });
        }
        /// <summary>
        /// 设置优惠额度
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunSetShipCustomerDialog(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            var view = new SetShipCustomerPriceDialog
            {
                DataContext = new SetShipCustomerPriceViewModel(CurrentShipCustomer)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingDiscountEventHandler);
        }
        /// <summary>
        /// 运行新增料款窗口
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunSetShipCustomerMoneyDialogCommand(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            var view = new SetShipCustomerMoneyDialog
            {
                DataContext = new SetShipCustomerMoneyViewModel(CurrentShipCustomer)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingMoneyEventHandler);
        }
        /// <summary>
        /// 运行客户详情窗口
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunShipCustomerDetailCommand(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            var view = new ShipCustomerDetailsDialog
            {
                DataContext = new ShipCustomerDetailsViewModel(CurrentShipCustomer)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }
        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetDataByCustomer();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetDataByCustomer();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetDataByCustomer();
            }
        }
        private void ExecuteExportCusOrderDetailReportCommand(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            long? startStamp = null;
            long? endStamp = null;
            if (ExportOrderStartDate != null)
                startStamp = Common.DateTime2TimeStamp(ExportOrderStartDate.Value.Date + ExportOrderStartTime.TimeOfDay);
            if (ExportOrderEndDate != null)
                endStamp = Common.DateTime2TimeStamp(ExportOrderEndDate.Value.Date + ExportOrderEndTime.TimeOfDay);

            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog
            {
                Title = "选择要保存的文件路径",
                Filter = "*.xls|",
                FileName = CurrentShipCustomer.Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH：mm") + " 导出详单.xls",
            };

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RequestStatus.StartRequest(() =>
                {
                    try
                    {
                        var fs = ApiClient.ExportShipCusOrder(new
                        {
                            customerId = CurrentShipCustomer.ID,
                            startTime = startStamp,
                            endTime = endStamp
                        }).GetAwaiter().GetResult();
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);

                        // 设置当前流的位置为流的开始
                        fs.Seek(0, SeekOrigin.Begin);
                        using (FileStream fsWrite = new FileStream(ofd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            BinaryWriter bw = new BinaryWriter(fsWrite);
                            bw.Write(bytes);
                            bw.Close();
                        }
                        fs.Close();
                        BiuMessageBoxWindows.BiuShow("导出成功!");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow(er.Message);
                    }
                });
            }
        }
        private void ExecuteVoidOneMoneyCommand(object o)
        {
            if (BiuMessageBoxResult.Cancel.Equals(BiuMessageBoxWindows.BiuShow("确定要作废该笔款项吗?", BiuMessageBoxButton.OKCancel, BiuMessageBoxImage.Question))) return;
            if (o == null) return;
            Task.Run(() =>
            {
                var a = ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.VoidCustomerMoney,
                        new { ID = (o as ShipCustomerMoney).ID }).Result;
                SetShipCustomerMoneyItems();
                ReloadCurrentShipCustomer();
            });
        }
        private void ExecuteRunStatementWindowCommand(object o)
        {
            /*
            var win = new ShipCustomerStatementWindow
            {
                Owner = Application.Current.MainWindow
            };
            win.ShowDialog();
            */
            try
            {
                var win = new ShipCustomerStatementWindow
                {
                    Owner = Application.Current.MainWindow
                };
                win.ShowDialog();
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }
        private void ExecuteSearchCustomerCommand(object o)
        {
            GetShipCustomerItems();
        }
        private async void ExecuteRunSettleCustomerWindowCommand(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先点选客户！");
                return;
            }
            /*
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("是否结算客户：\n" + CurrentShipCustomer.Name, BiuMessageBoxButton.YesNo)))
            {
                Console.WriteLine("结算!");
            }
            */
            long? startStamp = null;
            if (SettleDate != null)
                startStamp = Common.DateTime2TimeStamp(SettleDate.Value.Date + SettleTime.TimeOfDay);
            var view = new SettleShipCustomerDialog
            {
                DataContext = new SettleShipCustomerViewModel(CurrentShipCustomer, startStamp)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            SettleDate = null;
            SettleTime = DateTime.Now;
        }
        private void ExecuteExportSettleCommand(object o)
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("未选择客户!");
                return;
            }
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog
            {
                Title = "选择要保存的文件路径",
                Filter = "*.xls|",
                FileName = CurrentShipCustomer.Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH：mm") + " 结算详单.xls",
            };

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RequestStatus.StartRequest(() =>
                {
                    try
                    {
                        var fs = ApiClient.ExportSettleAsync(new
                        {
                            settleId = o.ToString()
                        }).GetAwaiter().GetResult();
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);

                        // 设置当前流的位置为流的开始
                        fs.Seek(0, SeekOrigin.Begin);
                        using (FileStream fsWrite = new FileStream(ofd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            BinaryWriter bw = new BinaryWriter(fsWrite);
                            bw.Write(bytes);
                            bw.Close();
                        }
                        fs.Close();
                        BiuMessageBoxWindows.BiuShow("导出成功!");
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow(er.Message);
                    }
                });
            }
        }
        private async void ExecuteRunEditOrderForCusDialogCommand(object o)
        {
            if (o == null || DeletedCustomer) return;
            var view = new EditOrderForCusDialog
            {
                DataContext = new EditOrderForCusViewModel(o as ShipCustomer)
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }
        private async void ExecuteDeleteCustomerCommand(object o)
        {
            var c = o as ShipCustomer;
            if (o == null || DeletedCustomer) return;
            if (BiuMessageBoxResult.Yes.Equals(BiuMessageBoxWindows.BiuShow("确定删除客户: " + c.Name + "?", BiuMessageBoxButton.YesNo, BiuMessageBoxImage.Question)))
            {
                await ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.DeleteShipCustomerAsync,
                    new { ID = c.ID });
                GetShipCustomerItems();
                GetDataByCustomer();
            }
        }

        /// <summary>
        /// 刷新当前选中的客户信息
        /// </summary>
        private void ReloadCurrentShipCustomer()
        {
            try
            {
                var customer = ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetShipCustomerByIDAsync,
                    new { ID = CurrentShipCustomer.ID }).Result.Data;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var index = ShipCustomerItems.IndexOf(CurrentShipCustomer);
                    ShipCustomerItems.RemoveAt(index);
                    ShipCustomerItems.Insert(index, customer);
                    CurrentShipCustomer = customer;
                }));
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetShipCustomerItems();
            GetDataByCustomer();
        }

        /// <summary>
        /// 车辆关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingCarEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            SetShipCustomerCarItems();
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingDiscountEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            SetShipCustomerGoodsPriceItems();
        }

        /// <summary>
        /// 创建客户界面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingCustomerEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetShipCustomerItems();
        }

        /// <summary>
        /// 料款界面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingMoneyEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            SetShipCustomerMoneyItems();
        }

        /// <summary>
        /// 获取客户车辆集合
        /// </summary>
        private void SetShipCustomerCarItems()
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            };
            RequestStatus.StartRequest(() =>
           {
               ShipCustomerCarItems = new ObservableCollection<ShipCustomerCar>(
                   ModelHelper.GetInstance().GetApiDataArg(ApiClient.GetShipCustomerCarAsync,
                   new { Customer = CurrentShipCustomer, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                    delegate (DataInfo<List<ShipCustomerCar>> result) { CurrentPage = result.Page; }
                   ).Result.Data);
           });
        }

        /// <summary>
        /// 获取出料客户的预付款流水
        /// </summary>
        private void SetShipCustomerMoneyItems()
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            };
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerMoneyItems = new ObservableCollection<ShipCustomerMoney>(ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.GetShipCustomerMoneyAsync,
                    new { Customer = CurrentShipCustomer, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                     delegate (DataInfo<List<ShipCustomerMoney>> result) { CurrentPage = result.Page; }
                    ).Result.Data);
            });
        }

        /// <summary>
        /// 获取出料客户的出料订单
        /// </summary>
        private void SetShipCustomerOrderItems()
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            };
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerOrderItems = new ObservableCollection<ShipOrder>(ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetShipOrderByShipCustomerAsync,
                        new ShipOrder { Customer = CurrentShipCustomer, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                        delegate (DataInfo<List<ShipOrder>> result) { CurrentPage = result.Page; }
                        ).Result.Data);
            });
        }

        /// <summary>
        /// 获取客户出料价格列表
        /// </summary>
        private void SetShipCustomerGoodsPriceItems()
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            };
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerGoodsPriceItems = new ObservableCollection<ShipCustomerGoodsPrice>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetShipCustomerGoodsPriceAsync,
                       new ShipCustomerGoodsPrice { CustomerId = CurrentShipCustomer.ID, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                       delegate (DataInfo<List<ShipCustomerGoodsPrice>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 获取客户结算记录
        /// </summary>
        private void SetShipCustomerSettleItems()
        {
            if (CurrentShipCustomer == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个客户!");
                return;
            };
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerSettleItems = new ObservableCollection<ShipCustomerSettle>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ApiClient.GetShipCustomerSettleAsync,
                        new { ID = CurrentShipCustomer.ID, Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                        delegate (DataInfo<List<ShipCustomerSettle>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 获取数据根据当前选中客户
        /// </summary>
        public void GetDataByCustomer()
        {
            if (CurrentShipCustomer == null)
            {
                ShipCustomerGoodsPriceItems?.Clear();
                ShipCustomerMoneyItems?.Clear();
                ShipCustomerCarItems?.Clear();
                ShipCustomerOrderItems?.Clear();
                ShipCustomerSettleItems?.Clear();
            }
            else
            {
                switch (SelectedIndex)
                {
                    case 0:
                        SetShipCustomerGoodsPriceItems();
                        break;
                    case 1:
                        SetShipCustomerMoneyItems();
                        break;
                    case 2:
                        SetShipCustomerCarItems();
                        break;
                    case 3:
                        SetShipCustomerOrderItems();
                        break;
                    case 4:
                        SetShipCustomerSettleItems();
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "SelectedIndex":
                case "CurrentShipCustomer":
                    if (CurrentPage == null) CurrentPage = new PageModel(16); else CurrentPage.Reset(16);
                    GetDataByCustomer();
                    break;
                case "DeletedCustomer":
                    GetShipCustomerItems();
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
