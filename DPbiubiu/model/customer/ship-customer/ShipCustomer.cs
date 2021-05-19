using biubiu.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.ship_customer
{
    public class ShipCustomer : BaseModel
    {
        /// <summary>
        /// 客户名
        /// </summary>
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

        /// <summary>
        /// 客户名首字母
        /// </summary>
        private string _szm = "";
        public string Szm
        {
            get { return _szm; }
            set
            {
                _szm = value;
                NotifyPropertyChanged("Szm");
            }
        }

        /// <summary>
        /// 联系人
        /// </summary>
        private string _linkMan = "";
        public string LinkMan
        {
            get { return _linkMan; }
            set
            {
                _linkMan = value;
                NotifyPropertyChanged("LinkMan");
            }
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        private string _contact = "";
        public string Contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                NotifyPropertyChanged("Contact");
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string _address = "";
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyPropertyChanged("Address");
            }
        }

        /// <summary>
        /// 预付款总额
        /// </summary>
        private double _money = 0;
        public double Money
        {
            get { return _money; }
            set
            {
                _money = value;
                NotifyPropertyChanged("Money");
                NotifyDiscountPropertyChanged("Money");
            }
        }

        /// <summary>
        /// 已用预付款
        /// </summary>
        private double _useMoney = 0;
        public double UseMoney
        {
            get { return _useMoney; }
            set
            {
                _useMoney = value;
                NotifyPropertyChanged("UseMoney");
                NotifyDiscountPropertyChanged("UseMoney");
            }
        }

        /// <summary>
        /// 余额
        /// </summary>
        private double _balance = 0.0;
        public double Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                NotifyPropertyChanged("Balance");
            }
        }

        /// <summary>
        /// 是否打印金额 （1显示，0不显示）
        /// </summary>
        private int _showMoney = 0;
        public int ShowMoney
        {
            get { return _showMoney; }
            set
            {
                _showMoney = value;
                NotifyPropertyChanged("ShowMoney");
            }
        }

        /// <summary>
        /// 是否抹零 （1抹零，0不抹零）
        /// </summary>
        private int _sale = 0;
        public int Sale
        {
            get { return _sale; }
            set
            {
                _sale = value;
                NotifyPropertyChanged("Sale");
            }
        }

        public event PropertyChangedEventHandler DiscountPropertyChanged;

        protected virtual void NotifyDiscountPropertyChanged(string propertyName)
        {
            Balance = Common.Double2DecimalCalculate(Money - UseMoney);
            DiscountPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
