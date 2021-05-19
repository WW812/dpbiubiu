using biubiu.model.goods.stock_goods;
using biubiu.model.stock_order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.stock_customer
{
    public class StockCustomerGoodsPrice : BaseModel
    {
        /// <summary>
        /// 料品
        /// </summary>
        /*
        private StockGoods _goods;
        public StockGoods Goods
        {
            get
            {
                return _goods;
            }
            set
            {
                _goods = value;
                NotifyPropertyChanged("Goods");
            }
        }
        */

        //料品ID
        private string _goodsId;
        public string GoodsId
        {
            get { return _goodsId; }
            set
            {
                _goodsId = value;
                NotifyPropertyChanged("GoodsId");
            }
        }
        //料品名称
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

        /*
        // 扣吨
        private double _deductWeight;
        public double DeductWeight
        {
            get { return _deductWeight; }
            set
            {
                _deductWeight = value;
                NotifyPropertyChanged("DeductWeight");
            }
        }

        // 扣吨方式
        private DeductWeightType _deductWeightType;
        public DeductWeightType DeductWeightType
        {
            get { return _deductWeightType; }
            set
            {
                _deductWeightType = value;
                NotifyPropertyChanged("DeductWeightType");
            }
        }

        /// <summary>
        /// 扣吨展示
        /// </summary>
        private string _deductWeightText;
        public string DeductWeightText
        {
            get { return _deductWeightText; }
            set
            {
                _deductWeightText = value;
                NotifyPropertyChanged("DeductWeightText");
            }
        }
        */

        public StockCustomerGoodsPrice()
        {
            //DeductWeightText = DeductWeight.ToString() + " " + (DeductWeightType == DeductWeightType.Percent ? "%" : "吨");
        }
    }
}
