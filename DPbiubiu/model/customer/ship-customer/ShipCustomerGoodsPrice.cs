using biubiu.model.ship_goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.ship_customer
{
    /// <summary>
    /// 客户料品价格model
    /// </summary>
    public class ShipCustomerGoodsPrice : BaseModel
    {
        private double _realPrice;
        public double RealPrice
        {
            get { return _realPrice; }
            set { _realPrice = value;
                CustomerPrice = RealPrice - DiscountMoney;
                NotifyPropertyChanged("RealPrice");
            }
        }

        private double _price;
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyPropertyChanged("Price");
            }
        }

        
        /// <summary>
        /// 料品
        /// </summary>
        private string _goodsId;
        public string GoodsId
        {
            get
            {
                return _goodsId;
            }
            set
            {
                _goodsId = value;
                NotifyPropertyChanged("Goods");
            }
        }

        private string _goodsName;
        public string GoodsName
        {
            get { return _goodsName; }
            set
            {
                _goodsName = value;
                NotifyPropertyChanged("GoodsName");
            }
        }

        /// <summary>
        /// 料品优惠额度
        /// </summary>
        private double _discountMoney;
        public double DiscountMoney
        {
            get
            {
                return _discountMoney;
            }
            set
            {
                _discountMoney = value;
                //if (Goods != null)
                //CustomerPrice = Goods.RealPrice - _discountMoney;
                CustomerPrice = RealPrice - _discountMoney;
                NotifyPropertyChanged("DiscountMoney");
            }
        }

        /// <summary>
        /// 客户价格
        /// </summary>
        private double _customerPrice;
        public double CustomerPrice
        {
            get { return _customerPrice; }
            set
            {
                _customerPrice = value;
                NotifyPropertyChanged("CustomerPrice");
            }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        private string _customerId;
        public string CustomerId
        {
            get
            {
                return _customerId;
            }
            set
            {
                _customerId = value;
                NotifyPropertyChanged("CustomerId");
            }
        }
    }
}
