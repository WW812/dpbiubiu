using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.FactoryService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace biubiu.FactoryService.StoneProduce
{
    public class StoneProduceViewModel : INotifyPropertyChanged
    {
        #region 属性

        public GetAllDataStatus RequestStatus = new GetAllDataStatus();

        /// <summary>
        /// 车队集合
        /// </summary>
        private ObservableCollection<StoneCarGroupModel> _stoneCarGroupItems;
        public ObservableCollection<StoneCarGroupModel> StoneCarGroupItems
        {
            get { return _stoneCarGroupItems; }
            set { _stoneCarGroupItems = value;
                NotifyPropertyChanged("StoneCarGroupItems");
            }
        }

        /// <summary>
        /// 当前选中的车队
        /// </summary>
        private StoneCarGroupModel _selectionCarGroup;
        public StoneCarGroupModel SelectionCarGroup
        {
            get { return _selectionCarGroup; }
            set
            {
                _selectionCarGroup = value;
                NotifyPropertyChanged("SelectionCarGroup");
            }
        }

        /// <summary>
        /// 车辆集合
        /// </summary>
        private ObservableCollection<StoneCarModel> _stoneCarItems;
        public ObservableCollection<StoneCarModel> StoneCarItems
        {
            get { return _stoneCarItems; }
            set
            {
                _stoneCarItems = value;
                NotifyPropertyChanged("StoneCarItems");
            }
        }

        /// <summary>
        /// 当前选择的车辆
        /// </summary>
        private StoneCarModel _selectionCar;
        public StoneCarModel SelectionCar
        {
            get { return _selectionCar; }
            set { _selectionCar = value;
                NotifyPropertyChanged("SelectionCar");
            }
        }

        /// <summary>
        /// 单据集合
        /// </summary>
        private ObservableCollection<StoneOrderModel> _stoneOrderItems;
        public ObservableCollection<StoneOrderModel> StoneOrderItems
        {
            get { return _stoneOrderItems; }
            set { _stoneOrderItems = value;
                NotifyPropertyChanged("StoneOrderItems");
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel { Page = 1, Size = Config.PageSize };
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

        private DateTime? _searchOrderStartDate = null;
        public DateTime? SearchOrderStartDate
        {
            get { return _searchOrderStartDate; }
            set
            {
                _searchOrderStartDate = value;
                NotifyPropertyChanged("SearchOrderStartDate");
            }
        }

        private DateTime _searchOrderStartTime = new DateTime();
        public DateTime SearchOrderStartTime
        {
            get { return _searchOrderStartTime; }
            set
            {
                _searchOrderStartTime = value;
                NotifyPropertyChanged("SearchOrderStartTime");
            }
        }

        private DateTime? _searchOrderEndDate = null;
        public DateTime? SearchOrderEndDate
        {
            get { return _searchOrderEndDate; }
            set
            {
                _searchOrderEndDate = value;
                NotifyPropertyChanged("SearchOrderEndDate");
            }
        }

        private DateTime _searchOrderEndTime = new DateTime();
        public DateTime SearchOrderEndTime
        {
            get { return _searchOrderEndTime; }
            set
            {
                _searchOrderEndTime = value;
                NotifyPropertyChanged("SearchOrderEndTime");
            }
        }
        #endregion

        #region 构造函数
        public StoneProduceViewModel()
        {
        }
        #endregion

        #region 方法 
        /// <summary>
        /// 获取车队
        /// </summary>
        public void GetCarGroupItems()
        {
            RequestStatus.StartRequest(()=> {
                StoneCarGroupItems = new ObservableCollection<StoneCarGroupModel>(
                    ModelHelper.GetInstance().GetApiData(ModelHelper.ApiClient.GetStoneCarGroupAsync).Result.Data
                 );
            });
        }

        /// <summary>
        /// 获取车辆
        /// </summary>
        public void GetCarItems()
        {
            if (SelectionCarGroup is null) return;
            RequestStatus.StartRequest(()=> {
                StoneCarItems = new ObservableCollection<StoneCarModel>(
                        ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.GetStoneCarAsync, new { TeamID = SelectionCarGroup.ID }).Result.Data
                    );
            });
        }

        /// <summary>
        /// 获取单据
        /// </summary>
        public void GetStoneOrderItems(StoneOrderModel som = null)
        {
            if (som is null)
                som = new StoneOrderModel
            {
                TeamID = SelectionCarGroup?.ID,
                CarID = SelectionCar?.ID,
                Page = CurrentPage.Page - 1,
                Size = CurrentPage.Size
            };
            else
            {
                som.Page = CurrentPage.Page - 1;
                som.Size = CurrentPage.Size;
            }
            if (null != SearchOrderStartDate)
                som.StartTime = Common.DateTime2TimeStamp(SearchOrderStartDate.Value.Date + SearchOrderStartTime.TimeOfDay);
            if (null != SearchOrderEndDate)
                som.EndTime = Common.DateTime2TimeStamp(SearchOrderEndDate.Value.Date + SearchOrderEndTime.TimeOfDay + TimeSpan.FromMinutes(1) - TimeSpan.FromMilliseconds(1));
            RequestStatus.StartRequest(()=> {
                StoneOrderItems = new ObservableCollection<StoneOrderModel>(
                    ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.GetStoneOrderAsync,
                        som,
                        (DataInfo<List<StoneOrderModel>> success)=>{
                            CurrentPage = success.Page;
                        }
                        ).Result.Data);
            });
        }
        #endregion

        #region 命令
        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExcuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExcutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExcuteNextPageCommand);

        private void ExcuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetStoneOrderItems();
        }
        private void ExcutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetStoneOrderItems();
            }
        }
        private void ExcuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetStoneOrderItems();
            }
        }
        #endregion

        #region 通知
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "SelectionCarGroup":
                    GetCarItems();
                    CurrentPage.Reset();
                    GetStoneOrderItems(new StoneOrderModel { TeamID = SelectionCarGroup?.ID });
                    break;
                case "SelectionCar":
                    CurrentPage.Reset();
                    GetStoneOrderItems();
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
