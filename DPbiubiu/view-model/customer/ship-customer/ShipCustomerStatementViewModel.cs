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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.customer.ship_customer
{
    public class ShipCustomerStatementViewModel : INotifyPropertyChanged
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
        /// 选中的支付类型
        /// </summary>
        private PayType _currentPayType;
        public PayType CurrentPayType
        {
            get { return _currentPayType; }
            set
            {
                _currentPayType = value;
                NotifyPropertyChanged("CurrentPayType");
            }
        }

        /// <summary>
        /// 预付款流水
        /// </summary>
        private ObservableCollection<ShipCustomerMoney> _shipCustomerMoney;
        public ObservableCollection<ShipCustomerMoney> ShipCustomerMoney
        {
            get
            {
                return _shipCustomerMoney;
            }
            set
            {
                _shipCustomerMoney = value;
                NotifyPropertyChanged("ShipCustomerMoney");
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

        /// <summary>
        /// 预付款流水分页size
        /// </summary>
        private readonly static int _pageSize = 18;

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel(_pageSize);
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
        /// 开始日期
        /// </summary>
        private DateTime? _dateStart = null;
        public DateTime? DateStart
        {
            get { return _dateStart; }
            set
            {
                _dateStart = value;
                NotifyPropertyChanged("DateStart");
            }
        }

        /// <summary>
        /// 结束日期
        /// </summary>
        private DateTime? _dateEnd = null;
        public DateTime? DateEnd
        {
            get { return _dateEnd; }
            set
            {
                _dateEnd = value;
                NotifyPropertyChanged("DateEnd");
            }
        }


        public ShipCustomerStatementViewModel()
        {
            GetShipCustomerItems();
            GetPayTypeItems();
            GetShipCustomerStatement();
        }

        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);
        public ICommand ResetSearchCommand => new AnotherCommandImplementation(ExecuteResetSearchCommand);
        public ICommand SearchCommand => new AnotherCommandImplementation(ExecuteSearchCommand);
        public ICommand ExportShipCustomerStatementCommand => new AnotherCommandImplementation(ExecuteExportShipCustomerStatementCommand);

        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetShipCustomerStatement();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetShipCustomerStatement();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetShipCustomerStatement();
            }
        }
        private void ExecuteResetSearchCommand(object o)
        {
            CurrentShipCustomer = null;
            DateStart = null;
            DateEnd = null;
            CurrentPayType = null;
            CurrentPage.Reset(_pageSize);
            GetShipCustomerStatement();
        }
        private void ExecuteSearchCommand(object o)
        {
            CurrentPage.Reset(_pageSize);
            GetShipCustomerStatement();
        }
        private void ExecuteExportShipCustomerStatementCommand(object o)
        {
            try
            {
                DateTime? dateend = null;
                if (DateEnd != null) dateend = DateEnd.Value.AddSeconds(86399);
                int? pt = 0;
                string at = null;
                if (CurrentPayType == null)
                {
                    pt = null;
                }
                else if (CurrentPayType.ID.Equals("honour"))
                {
                    pt = 1;
                }
                else
                {
                    at = CurrentPayType.ID;
                }
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog
                {
                    Title = "选择要保存的文件路径",
                    Filter = "*.xls|",
                    FileName = DateTime.Now.ToString("yyyy-MM-dd HH：mm") + " 预付款流水.xls",
                };
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RequestStatus.StartRequest(() =>
                    {
                        var fs = ApiClient.ExportShipCustomerStatement(new
                        {
                            payType = pt,
                            account = at,
                            customerId = CurrentShipCustomer?.ID,
                            startTime = Common.DateTime2TimeStamp(DateStart),
                            endTime = Common.DateTime2TimeStamp(dateend),
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

                        BiuMessageBoxWindows.BiuShow("导出成功!", image: BiuMessageBoxImage.None);
                    });
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 拉取出料客户
        /// </summary>
        public void GetShipCustomerItems(string searchFeed = "")
        {
            CurrentCustomerPage = new PageModel(500);
            ShipCustomerItems?.Clear();
            if (string.IsNullOrWhiteSpace(searchFeed)) searchFeed = null;
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerItems = new ObservableCollection<ShipCustomer>(
                   ModelHelper.GetInstance().GetApiDataArg(
                   ApiClient.GetShipCustomerAsync,
                  new {szm= searchFeed, Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
                 delegate (DataInfo<List<ShipCustomer>> result) { CurrentCustomerPage = result.Page; }).Result.Data);
            },null);
        }

        /// <summary>
        /// 下一页进料客户
        /// </summary>
        public void NextPageCustomerItems(ComboBox lv)
        {
            if (CurrentCustomerPage.Last)
            {
                /*
                Task.Run(() =>
                {
                    SnackbarViewModel.GetInstance().PoupMessageAsync("已到底部");
                });
                */
                return;
            }
            RequestStatus.StartRequest(() =>
            {
                CurrentCustomerPage.Page++;
                var r = new ObservableCollection<ShipCustomer>(ModelHelper.GetInstance().GetApiDataArg(
                ApiClient.GetShipCustomerAsync,
                new { Page = CurrentCustomerPage.Page - 1, Size = CurrentCustomerPage.Size },
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

        /// <summary>
        /// 获取预付款流水
        /// </summary>
        public void GetShipCustomerStatement()
        {
            DateTime? dateend = null;
            if (DateEnd != null) dateend = DateEnd.Value.AddSeconds(86399);
            int? pt =  0;
            string at = null;
            if(CurrentPayType == null)
            {
                pt = null;
            }
            else if(CurrentPayType.ID.Equals("honour"))
            {
                pt = 1;
            }
            else
            {
                at = CurrentPayType.ID;
            }
            RequestStatus.StartRequest(() =>
            {
                ShipCustomerMoney = new ObservableCollection<ShipCustomerMoney>(
                        ModelHelper.GetInstance().GetApiDataArg(
                            ApiClient.GetShipCustomerStatement,
                            new
                            {
                                PayType= pt,
                                Account = at,
                                Customer = CurrentShipCustomer,
                                CreateTime = Common.DateTime2TimeStamp(DateStart),
                                CreateTimeEnd = Common.DateTime2TimeStamp(dateend),
                                Page = CurrentPage.Page - 1,
                                Size = CurrentPage.Size
                            },
                        delegate (DataInfo<List<ShipCustomerMoney>> result) { CurrentPage = result.Page; }).Result.Data);
            });
        }

        /// <summary>
        /// 获取支付类型
        /// </summary>
        public void GetPayTypeItems()
        {
            CurrentPayTypePage = new PageModel(20);
            RequestStatus.StartRequest(() =>
            {
                var r = new ObservableCollection<PayType>(
                   ModelHelper.GetInstance().GetApiDataArg(
                       ApiClient.GetPayTypeAsync,
                       new { Page = CurrentPayTypePage.Page - 1, Size = CurrentPayTypePage.Size },
                       delegate (DataInfo<List<PayType>> result) { CurrentPayTypePage = result.Page; }
                       ).Result.Data);
                r.Insert(0, new PayType { ID = "honour", Name = "承兑" });
                PayTypeItems = r;
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
            RequestStatus.StartRequest(() =>
            {
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
