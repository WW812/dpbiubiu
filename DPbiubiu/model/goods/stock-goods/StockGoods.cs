using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.goods.stock_goods
{
    public class StockGoods : BaseModel
    {
        //料品名称
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        //执行价格
        private double _realPrice = 0;
        public double RealPrice
        {
            get { return _realPrice; }
            set
            {
                _realPrice = value;
                NotifyPropertyChanged("RealPrice");
            }
        }
        //是否启用 0禁用(停止采购) 1启用(启动采购)
        private int _valid = 1;
        public int Valid
        {
            get { return _valid; }
            set
            {
                _valid = value;
                SelfPropertyChanged("Valid");
            }
        }
        private string _validText = "";
        public string ValidText
        {
            get { return _validText; }
            set
            {
                _validText = value;
                NotifyPropertyChanged("ValidText");
            }
        }

        private string _validButton = "";
        public string ValidButton
        {
            get { return _validButton; }
            set
            {
                _validButton = value;
                NotifyPropertyChanged("ValidButton");
            }
        }

        public event PropertyChangedEventHandler SPropertyChanged;

        protected virtual void SelfPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "Valid":
                        if (Valid == 1) { ValidText = "采购中"; ValidButton = "停止"; }
                        else { ValidText = ""; ValidButton = "采购"; }
                        break;
            }
            SPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
