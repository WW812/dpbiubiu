using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.goods.stock_goods;
using biubiu.views.marketing.stock_goods;
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

namespace biubiu.view_model.stock_goods
{
    public class StockGoodsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 采购料品集合
        /// </summary>
        private ObservableCollection<StockGoods> _stockGoodsItems;
        public ObservableCollection<StockGoods> StockGoodsItems
        {
            get { return _stockGoodsItems; }
            set
            {
                _stockGoodsItems = value;
                NotifyPropertyChanged("StockGoodsItems");
            }
        }


        public ICommand RunCreateStockGoodsDialogCommand => new AnotherCommandImplementation(ExecuteRunCreateStockGoodsDialogCommand);
        //public ICommand ChangeValidCommand => new AnotherCommandImplementation(ExecuteChangeValidCommand);
        public ICommand RunChangeStockGoodsPriceDialogCommand => new AnotherCommandImplementation(ExecuteRunChangeStockGoodsPriceDialogCommand);

        private async void ExecuteRunCreateStockGoodsDialogCommand(object o)
        {
            var view = new CreateStockGoodsDialog
            {
                DataContext = new CreateStockGoodsViewModel()
            };
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }
        public async void ExecuteChangeValidCommand(StockGoods sg)
        {
            var message = sg.Name;
            if (sg.Valid == 1)
            {
                sg.Valid = 0;
                message += "已经停止采购";
            }
            else
            {
                sg.Valid = 1;
                message += "已经开始采购";
            }
            await ModelHelper.GetInstance().GetApiDataArg(
                      ApiClient.ChangeStockGoodsValidAsync,
                      sg,
                      delegate (DataInfo<StockGoods> result) {
                          SnackbarViewModel.GetInstance().PoupMessageAsync(message);
                               }
                  );
        }
        public async void ExecuteRunChangeStockGoodsPriceDialogCommand(object o)
        {
            var view = new ChangeStockGoodsPriceDialog
            {
                DataContext = new ChangeStockGodsPriceViewModel()
            };
            await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        public void GetStockGoodsItems()
        {
            StockGoodsItems?.Clear();
            Task.Run(() =>
            {
                StockGoodsItems = new ObservableCollection<StockGoods>(
                    ModelHelper.GetInstance().GetApiData(new ApiDelegate<List<StockGoods>>(
                        ApiClient.GetStockGoodsAsync)).Result.Data);
            });
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            GetStockGoodsItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
