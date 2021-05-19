using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.stock_order
{

    /// <summary>
    /// 进料单据模型
    /// </summary>
    public class StockOrder : BaseModel
    {
        public StockOrder()
        {
            Reset();
            /*
            DeductWeightText = DeductWeight.ToString() + " " + (DeductWeightType == 1 ? "%" : "吨");
            PaidText = Paid == 0 ?"未支付":"已支付";
            DeductWeightTypeText = DeductWeightType == 0 ? "扣吨: " : "扣率: ";
            */
        }

        //订单号
        private string _orderNo;
        public string OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                _orderNo = value;
                NotifyPropertyChanged("OrderNo");
            }
        }
        //进厂单号 => 改为编号
        private string _enterOrderNo;
        public string EnterOrderNo
        {
            get
            {
                return _enterOrderNo;
            }
            set
            {
                _enterOrderNo = value;
                NotifyPropertyChanged("EnterOrderNo");
            }
        }
        //皮重
        private double _carTare = 0.0;
        public double CarTare
        {
            get
            {
                return _carTare;
            }
            set
            {
                _carTare = value;
                SelfPropertyChanged("CarTare");
                NotifyPropertyChanged("CarTare");
            }
        }
        //车号
        private string _carId = "";
        public string CarId
        {
            get
            {
                return _carId;
            }
            set
            {
                _carId = value;
                NotifyPropertyChanged("CarId");
            }
        }
        //料品执行价格
        private double _goodsRealPrice = 0.0;
        public double GoodsRealPrice
        {
            get
            {
                return _goodsRealPrice;
            }
            set
            {
                _goodsRealPrice = value;
                NotifyPropertyChanged("GoodsRealPrice");
            }
        }
        private StockGoods _goods = null;
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
        //毛重
        private double _carGrossWeight = 0.0;
        public double CarGrossWeight
        {
            get
            {
                return _carGrossWeight;
            }
            set
            {
                _carGrossWeight = value;
                SelfPropertyChanged("CarGrossWeight");
                NotifyPropertyChanged("CarGrossWeight");
            }
        }
        //净重
        private double _carNetWeight = 0.0;
        public double CarNetWeight
        {
            get
            {
                return _carNetWeight;
            }
            set
            {
                _carNetWeight = value;
                SelfPropertyChanged("CarNetWeight");
                NotifyPropertyChanged("CarNetWeight");
            }
        }
        private StockCustomer _customer = null;
        public StockCustomer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }
        //应收金额
        private double _orderMoney = 0.0;
        public double OrderMoney
        {
            get
            {
                return _orderMoney;
            }
            set
            {
                _orderMoney = value;
                NotifyPropertyChanged("OrderMoney");
            }
        }
        //优惠金额
        private double _discountMoney = 0.0;
        public double DiscountMoney
        {
            get
            {
                return _discountMoney;
            }
            set
            {
                _discountMoney = value;
                NotifyPropertyChanged("DiscountMoney");
            }
        }
        //实付金额
        private double _realMoney = 0.0;
        public double RealMoney
        {
            get
            {
                return _realMoney;
            }
            set
            {
                _realMoney = value;
                NotifyPropertyChanged("RealMoney");
            }
        }

        //状态码 0进厂订单，1出场订单，2已交帐
        private int _status = 0;
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }
        //单子类型：0自然生成  1，补单
        private int _type = 0;
        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                NotifyPropertyChanged("Type");
            }
        }
        //客户和非客户 0散户 1客户
        private int _customerType = 0;
        public int CustomerType
        {
            get
            {
                return _customerType;
            }
            set
            {
                _customerType = value;
                NotifyPropertyChanged("CustomerType");
            }
        }

        //0,正常,1，假删
        private int _hedge = 0;
        public int Hedge
        {
            get
            {
                return _hedge;
            }
            set
            {
                _hedge = value;
                NotifyPropertyChanged("Hedge");
            }
        }

        // 扣吨
        private double _deductWeight;
        public double DeductWeight
        {
            get { return _deductWeight; }
            set
            {
                _deductWeight = value;
                SelfPropertyChanged("DeductWeight");
                NotifyPropertyChanged("DeductWeight");
            }
        }

        // 扣吨方式 扣吨方式 1 为百分比 ， 0为直接扣除相应吨数
        private int _deductWeightType;
        public int DeductWeightType
        {
            get { return _deductWeightType; }
            set
            {
                _deductWeightType = value;
                SelfPropertyChanged("DeductWeightType");
                NotifyPropertyChanged("DeductWeightType");
            }
        }

        private string _deductWeightTypeText;
        public string DeductWeightTypeText
        {
            get { return _deductWeightTypeText; }
            set { _deductWeightTypeText = value;
                NotifyPropertyChanged("DeductWeightTypeText");
            }
        }

        //每吨运费
        private double _freightOfTon;
        public double FreightOfTon
        {
            get { return _freightOfTon; }
            set
            {
                _freightOfTon = value;
                NotifyPropertyChanged("FreightOfTon");
            }
        }

        //是否已结账 0未结 1已结
        private int _paid;
        public int Paid
        {
            get { return _paid; }
            set
            {
                _paid = value;
                SelfPropertyChanged("Paid");
                NotifyPropertyChanged("Paid");
            }
        }

        //支付日期
        private long _paidDate;
        public long PaidDate
        {
            get { return _paidDate; }
            set { _paidDate = value;
                NotifyPropertyChanged("PaidDate");
            }
        }

        //结账展示
        private string _paidText;
        public string PaidText
        {
            get { return _paidText; }
            set { _paidText = value;
                NotifyPropertyChanged("PaidText");
            }
        }

        //进厂地磅号
        private string _enterPonderation = "#";
        public string EnterPonderation
        {
            get
            {
                return _enterPonderation;
            }
            set
            {
                _enterPonderation = value;
                NotifyPropertyChanged("EnterPonderation");
            }
        }
        //出厂地磅号
        private string _exitPonderation = "#";
        public string ExitPonderation
        {
            get
            {
                return _exitPonderation;
            }
            set
            {
                _exitPonderation = value;
                NotifyPropertyChanged("ExitPonderation");
            }
        }
        //修改时间
        private long _editTime = 0;
        public long EditTime
        {
            get
            {
                return _editTime;
            }
            set
            {
                _editTime = value;
                NotifyPropertyChanged("EditTime");
            }
        }
        //修改人
        private User _editUser = null;
        public User EditUser
        {
            get
            {
                return _editUser;
            }
            set
            {
                _editUser = value;
                NotifyPropertyChanged("EditUser");
            }
        }
        //修改原因
        private string _editReason = "";
        public string EditReason
        {
            get
            {
                return _editReason;
            }
            set
            {
                _editReason = value;
                NotifyPropertyChanged("EditReason");
            }
        }
        //修改描述
        private string _editNote = "";
        public string EditNote
        {
            get
            {
                return _editNote;
            }
            set
            {
                _editNote = value;
                NotifyPropertyChanged("EditNote");
            }
        }
        //修改备注
        private string _manualEditNote = "";
        public string ManualEditNote
        {
            get
            {
                return _manualEditNote;
            }
            set
            {
                _manualEditNote = value;
                NotifyPropertyChanged("ManualEditNote");
            }
        }
        //进厂时间
        private long _enterTime = 0;
        public long EnterTime
        {
            get
            {
                return _enterTime;
            }
            set
            {
                _enterTime = value;
                NotifyPropertyChanged("EnterTime");
            }
        }
        //出厂时间
        private long _exitTime = 0;
        public long ExitTime
        {
            get
            {
                return _exitTime;
            }
            set
            {
                _exitTime = value;
                NotifyPropertyChanged("ExitTime");
            }
        }
        //进厂司磅员
        private User _enterUser;
        public User EnterUser
        {
            get
            {
                return _enterUser;
            }
            set
            {
                _enterUser = value;
                NotifyPropertyChanged("EnterUser");
            }
        }
        //出厂司磅员
        private User _exitUser;
        public User ExitUser
        {
            get { return _exitUser; }
            set
            {
                _exitUser = value;
                NotifyPropertyChanged("ExitUser");
            }
        }

        //修改次数
        private int _modifyNum;
        public int ModifyNum
        {
            get { return _modifyNum; }
            set
            {
                _modifyNum = value;
                NotifyPropertyChanged("ModifyNum");
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

        /// <summary>
        /// 重置所有变量
        /// </summary>
        public void Reset()
        {
            OrderNo = "";
            EnterOrderNo = "";
            CarTare = 0.0;
            CarId = "";
            GoodsRealPrice = 0.0;
            Goods = null;
            CarGrossWeight = 0.0;
            CarNetWeight = 0.0;
            Customer = null;
            RealMoney = 0.0;
            Status = 0;
            Type = 0;
            CustomerType = 0;
            EnterPonderation = "#";
            EditTime = 0;
            EditUser = null;
            EditReason = "";
            EditNote = "";
            EnterTime = 0;
            ExitTime = 0;
            ExitPonderation = "#";
            Note = "";
            ModifyNum = 0;
            DeductWeight = 0.0;
            DeductWeightType = 0;
            FreightOfTon = 0;
            Paid = 0;
        }

        /// <summary>
        /// 刷新所有计算
        /// </summary>
        /// <param name="sender">1为优惠价格发送</param>
        public void Calculate(int sender = 0)
        {
            try
            {
                if (sender != 1)
                {
                    if (DeductWeightType == 1) // 百分比扣吨
                    {
                        //CarNetWeight = Common.Double2DecimalCalculate(CarNetWeight * (1 - DeductWeight / 100),1);
                        OrderMoney = Common.Double2DecimalCalculate(CarNetWeight * (GoodsRealPrice + FreightOfTon));
                    }
                    else // 吨数扣吨
                    {
                        //CarNetWeight = Common.Double2DecimalCalculate(CarGrossWeight - CarTare - DeductWeight,1);
                        OrderMoney = Common.Double2DecimalCalculate(CarNetWeight * (GoodsRealPrice + FreightOfTon));
                    }
                    DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 10);
                }
                RealMoney = Common.Double2DecimalCalculate(OrderMoney - DiscountMoney);
            }
            catch(Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        private void CalculateCarNetWeight()
        {
            if (DeductWeightType == 1) // 百分比扣吨
            {
                CarNetWeight = Common.Double2DecimalCalculate((CarGrossWeight - CarTare) * (1 - DeductWeight / 100));
            }
            else // 吨数扣吨
            {
                CarNetWeight = Common.Double2DecimalCalculate((CarGrossWeight - CarTare - DeductWeight));
            }
        }

        public event PropertyChangedEventHandler CalculatePropertyChanged;

        protected virtual void SelfPropertyChanged(string propertyName)
        {
            if (Status != 0)  //单据状态
            {
                switch (propertyName)
                {
                    case "CarTare":
                    case "CarGrossWeight":
                        CalculateCarNetWeight();
                        break;
                    case "CarNetWeight":
                        Calculate(0);
                        break;
                }
            }
            switch(propertyName)
            {
                case "DeductWeightType":
                    DeductWeightTypeText = DeductWeightType == 0 ?"扣吨: ": "扣率: ";
                    DeductWeightText = DeductWeight.ToString() + " " + (DeductWeightType == 1 ? "%" : "吨");
                    CalculateCarNetWeight();
                    break;
                case "DeductWeight":
                    DeductWeightText = DeductWeight.ToString() + " " + (DeductWeightType == 1 ? "%" : "吨");
                    CalculateCarNetWeight();
                    break;
                case "Paid":
                    PaidText = Paid == 0 ? "未支付" : "已支付";
                    break;
            }
            CalculatePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
