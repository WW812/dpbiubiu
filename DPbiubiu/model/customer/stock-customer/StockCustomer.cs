using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.stock_customer
{
    public class StockCustomer : BaseModel
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
        /// 未结账总金额
        /// </summary>
        private double _nonpaymentAmount = 0;
        public double NonpaymentAmount
        {
            get { return _nonpaymentAmount; }
            set
            {
                _nonpaymentAmount = value;
                NotifyPropertyChanged("NonpaymentAmount");
            }
        }

        /// <summary>
        /// 未支付的订单数
        /// </summary>
        private double _nonpaymentNum = 0;
        public double NonpaymentNum
        {
            get { return _nonpaymentNum; }
            set {
                _nonpaymentNum = value;
                NotifyPropertyChanged("NonpaymentNum");
            }
        }
    }
}
