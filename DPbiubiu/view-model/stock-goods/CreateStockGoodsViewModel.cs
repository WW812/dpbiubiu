using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.goods.stock_goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static biubiu.model.ModelHelper;

namespace biubiu.view_model.stock_goods
{
    public class CreateStockGoodsViewModel : BaseModel
    {
        /// <summary>
        /// 采购料品
        /// </summary>
        private StockGoods _goods;
        public StockGoods Goods
        {
            get { return _goods; }
            set
            {
                _goods = value;
                NotifyPropertyChanged("Goods");
            }
        }

        /// <summary>
        /// 提交按钮是否可用
        /// </summary>
        private Boolean _submitButtonIsEnabled = true;
        public Boolean SubmitButtonIsEnabled
        {
            get { return _submitButtonIsEnabled; }
            set
            {
                _submitButtonIsEnabled = value;
                NotifyPropertyChanged("SubmitButtonIsEnabled");
            }
        }

        public CreateStockGoodsViewModel()
        {
            Goods = new StockGoods();
        }

        public ICommand SubmitCommand => new AnotherCommandImplementation(ExecuteSubmitCommand);

        private async void ExecuteSubmitCommand(Object o)
        {
            /*
            try
            {
                SubmitButtonIsEnabled = false;
                var Result = await ApiClient.CreateStockGoodsAsync(Goods);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                }
            }
            catch (Exception er)
            {
                var msg = "";
                if (er.InnerException != null)
                    msg += er.InnerException.Message + "\n";
                msg += er.Message;
                BiuMessageBoxWindows.BiuShow(msg,image:BiuMessageBoxImage.Error);
                SubmitButtonIsEnabled = true;
            }
            */
            SubmitButtonIsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateStockGoodsAsync,
                Goods,
                delegate (DataInfo<StockGoods> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, (o as UserControl));
                    }));
                });
            SubmitButtonIsEnabled = true;
        }
    }
}
