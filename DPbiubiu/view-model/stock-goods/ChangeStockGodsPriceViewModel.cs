using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.goods.stock_goods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_goods
{
    public class ChangeStockGodsPriceViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 采购料品集合
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

        private StockGoods _editGoods;
        public StockGoods EditGoods
        {
            get { return _editGoods; }
            set { _editGoods = value;
                NotifyPropertyChanged("EditGoods");
            }
        }

        private StockGoods _currentGoods;
        public StockGoods CurrentGoods
        {
            get { return _currentGoods; }
            set { _currentGoods = value;
                NotifyPropertyChanged("CurrentGoods");
            }
        }

        private GetAllDataStatus _requestStatus = new GetAllDataStatus();
        public GetAllDataStatus RequestStatus
        {
            get { return _requestStatus; }
            set { _requestStatus = value;
                NotifyPropertyChanged("RequestStatus");
            }
        }

        public ChangeStockGodsPriceViewModel()
        {
            GetStockGoodsItems();
            EditGoods = new StockGoods();
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand);

        private void ExecuteSubmitCommand(object o)
        {
            if (CurrentGoods == null) return;
            RequestStatus.StartRequest(async ()=> {
                await ModelHelper.GetInstance().GetApiDataArg(
                    ApiClient.ChangeStockGoodsPriceAsync,
                    EditGoods,
                    delegate(DataInfo<StockGoods> result) {
                        EditGoods = new StockGoods();
                    });
                GetStockGoodsItems();
            });
        }

        public void GetStockGoodsItems()
        {
            RequestStatus.StartRequest(() =>
            {
                GoodsItems = new ObservableCollection<StockGoods>(
                    ModelHelper.GetInstance().GetApiData(
                        ApiClient.GetStockGoodsAsync).Result.Data);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "CurrentGoods":
                    if (CurrentGoods == null) { EditGoods = new StockGoods(); return; }
                    EditGoods = Common.DeepCopy(CurrentGoods);
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
