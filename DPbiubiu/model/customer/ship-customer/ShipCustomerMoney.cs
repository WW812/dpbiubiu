using biubiu.model.paytype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.ship_customer
{
    public class ShipCustomerMoney : BaseModel
    {
        /// <summary>
        /// 预付款金额
        /// </summary>
        private double _money;
        public double Money
        {
            get { return _money; }
            set
            {
                _money = value;
                NotifyPropertyChanged("Money");
            }
        }

        /// <summary>
        /// 支付日期
        /// </summary>
        private long? _payTime;
        public long? PayTime
        {
            get { return _payTime; }
            set
            {
                _payTime = value;
                NotifyPropertyChanged("PayTime");
            }
        }

        /// <summary>
        /// 客户名
        /// </summary>
        private string _cusName;
        public string CusName
        {
            get { return _cusName; }
            set
            {
                _cusName = value;
                NotifyPropertyChanged("CusName");
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        private ShipCustomer _customer;
        public ShipCustomer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }


        /// <summary>
        /// 支付账户类型
        /// </summary>
        private string _account;
        public string Account
        {
            get { return _account; }
            set
            {
                _account = (value.Equals("")&&PayType == 1) ? "承兑" : value;
                //_account = value;
                NotifyPropertyChanged("Account");
            }
        }

        /// <summary>
        /// 支付类型 0 非承兑 1承兑
        /// </summary>
        private int _payType;
        public int PayType
        {
            get { return _payType; }
            set
            {
                _payType = value;

                NotifyPropertyChanged("PayType");
            }
        }

        /// <summary>
        /// 支付类型显示
        /// </summary>
        /*
        private string _payTypeDisplay;
        public string PayTypeDisplay
        {
            get
            {
                if (_payType == 1)
                    return "承兑";
                else
                    return Account;
                //return _payTypeDisplay;
            }
            set
            {
                _payTypeDisplay = value;
                NotifyPropertyChanged("PayTypeDisplay");
            }
        }
        */

        #region 承兑

        /// <summary>
        /// 承兑编号
        /// </summary>
        public string HonourNum { get; set; }

        /// <summary>
        /// 承兑时间(承兑到期日)
        /// </summary>
        public long? HonourTime { get; set; }

        /// <summary>
        /// 承兑折扣
        /// </summary>
        public double HonourDiscount { get; set; }

        /// <summary>
        /// 承兑状态 0 未兑现； 1 已兑现； 2 已出账； 3 退还原主； 4 有问题； 5 兑换中； 6 到期承兑
        /// </summary>
        public int HonourStatus { get; set; }
        #endregion

    }
}
