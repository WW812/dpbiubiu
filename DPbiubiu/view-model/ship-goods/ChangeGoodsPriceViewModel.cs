using biubiu.model;
using biubiu.model.ship_goods;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.goods
{
    public class ChangeGoodsPriceViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 料品标准价格变动幅度
        /// </summary>
        private double _priceChangeRange;
        public double PriceChangeRange
        {
            get { return _priceChangeRange; }
            set
            {
                _priceChangeRange = value;
                NotifyPropertyChanged("PriceChangeRange");
            }
        }

        /// <summary>
        /// 料品执行价格变动幅度
        /// </summary>
        private double _realPriceChangeRange;
        public double RealPriceChangeRange
        {
            get { return _realPriceChangeRange; }
            set
            {
                _realPriceChangeRange = value;
                NotifyPropertyChanged("RealPriceChangeRange");
            }
        }

        private ObservableCollection<ShipGoods> _goodsItems;
        public ObservableCollection<ShipGoods> GoodsItems
        {
            get
            {
                return _goodsItems;
            }
            set
            {
                _goodsItems = value;
                NotifyPropertyChanged("GoodsItems");
            }
        }

        public ChangeGoodsPriceViewModel()
        {
            GetGoodsItems();
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        private void GetGoodsItems()
        {
            GoodsItems?.Clear();
            Task.Run(()=> {
            GoodsItems = new ObservableCollection<ShipGoods>(
                ModelHelper.GetInstance().GetApiData(
                    ApiClient.GetGoodsAsync).Result.Data);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
