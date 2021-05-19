using biubiu.Domain;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.view_model.goods;
using biubiu.views.marketing.goods;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.ship_goods
{
    public class ShipGoodsViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<ShipGoods> _shipGoodsItems;
        public ObservableCollection<ShipGoods> ShipGoodsItems
        {
            get { return _shipGoodsItems; }
            set
            {
                _shipGoodsItems = value;
                NotifyPropertyChanged("ShipGoodsItems");
            }
        }

        public ICommand RunChangesGoodsPriceDialog => new AnotherCommandImplementation(ExecuteRunChangesGoodsPriceDialog);
        public ICommand RunNewGoodsDialogCommand => new AnotherCommandImplementation(ExecuteRunNewGoodsDialog);

        private async void ExecuteRunNewGoodsDialog(object o)
        {
            var view = new NewGoodsDialog
            {
                //DataContext = new GoodsViewModel(),
                //GoodsPageObject = GoodsPageObject,
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        /// <summary>
        /// 价格调整窗口
        /// </summary>
        /// <param name="o"></param>
        private async void ExecuteRunChangesGoodsPriceDialog(object o)
        {
            var view = new ChangeGoodsPriceDialog
            {
                DataContext = new ChangeGoodsPriceViewModel(),
                TitleStr = "料品" + (int.Parse(o.ToString()) == -1 ? "降价" : "涨价"),
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        public void GetShipGoodsItems()
        {
            ShipGoodsItems?.Clear();
            Task.Run(()=> {
                ShipGoodsItems = new ObservableCollection<ShipGoods>(ModelHelper.GetInstance().GetApiData(ApiClient.GetGoodsAsync).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            //Console.WriteLine("You can intercept the closing event, and cancel here.");
            GetShipGoodsItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
