using biubiu.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.ship_customer
{
    public class ShipCustomerSettle : BaseModel
    {
        // 预付款
        private double _money;
        public double Money
        {
            get { return _money; }
            set
            {
                _money = value;
                NotifyDiscountPropertyChanged("Money");
            }
        }
        // 料款
        private double _useMoney;
        public double UseMoney
        {
            get { return _useMoney; }
            set { _useMoney = value;
                NotifyDiscountPropertyChanged("UseMoney");
            }
        }
        // 余额
        public double Balance { get; set; }
        // 结算开始时间
        public long SettleStartTime { get; set; }
        // 结算结束时间
        public long SettleEndTime { get; set; }

        public event PropertyChangedEventHandler DiscountPropertyChanged;

        protected virtual void NotifyDiscountPropertyChanged(string propertyName)
        {
            Balance = Common.Double2DecimalCalculate(Money - UseMoney);
            DiscountPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
