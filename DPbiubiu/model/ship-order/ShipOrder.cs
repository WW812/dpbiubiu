using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.customer;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.model.ship_order
{
    /// <summary>
    /// 出料单据模型
    /// </summary>
    public class ShipOrder : BaseModel 
    {

        public ShipOrder()
        {
            Reset();
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
        //料品单价(平台价格)
        private double _goodsPrice = 0.0;
        public double GoodsPrice
        {
            get
            {
                return _goodsPrice;
            }
            set
            {
                _goodsPrice = value;
                NotifyPropertyChanged("GoodsPrice");
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
        private ShipGoods _goods = null;
        public ShipGoods Goods
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
        private ShipCustomer _customer = null;
        public ShipCustomer Customer
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
        //方量
        private double _cubic = 0.0;
        public double Cubic
        {
            get { return _cubic; }
            set
            {
                _cubic = value;
                NotifyPropertyChanged("Cubic");
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
        //实收金额
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
        //状态码 0新订单,1付款订单 2已交帐
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
        //是否空车,0不是空车,1空车
        private int _emptyCar = 0;
        public int EmptyCar
        {
            get
            {
                return _emptyCar;
            }
            set
            {
                _emptyCar = value;
                NotifyPropertyChanged("EmptyCar");
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
        //客户和非客户 0零售 1客户
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
        //预装吨数，只在进场时侯有用
        private double _advanceWeight = 0.0;
        public double AdvanceWeight
        {
            get
            {
                return _advanceWeight;
            }
            set
            {
                _advanceWeight = value;
                NotifyPropertyChanged("AdvanceWeight");
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

        // 电话
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        // 平台金额(总价)
        private double _platformMoney;
        public double PlatformMoney
        {
            get { return _platformMoney; }
            set
            {
                _platformMoney = value;
                NotifyPropertyChanged("PlatformMoney");
            }
        }

        // Sung
        private string _rfid;
        public string RFID
        {
            get { return _rfid; }
            set
            {
                _rfid = value;
                NotifyPropertyChanged("RFID");
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
            GoodsPrice = 0.0;
            GoodsRealPrice = 0.0;
            Goods = null;
            CarGrossWeight = 0.0;
            CarNetWeight = 0.0;
            Customer = null;
            OrderMoney = 0.0;
            DiscountMoney = 0.0;
            RealMoney = 0.0;
            Status = 0;
            EmptyCar = 0;
            Hedge = 0;
            Type = 0;
            CustomerType = 0;
            AdvanceWeight = 0.0;
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
            Cubic = 0.0;
            Phone = "";
            PlatformMoney = 0.0;
            // Sung
            RFID = "";
        }

        /// <summary>
        /// 刷新所有计算
        /// </summary>
        /// <param name="sender">1为优惠金额输入框发送</param>
        public void Calculate(int sender = 0)
        {
            try
            {
                if (sender != 1)
                {
                    OrderMoney = Common.Double2DecimalCalculate(CarNetWeight * GoodsRealPrice);
                    if (CustomerType == 0 && Customer == null)
                    { // 零售
                        switch (Config.SYSTEM_SETTING.ShipOrderDiscount)
                        {
                            case 1: //个位抹零
                                DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 10);
                                break;
                            case 2: //小数点抹零
                                DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 1);
                                break;
                            case 3: //四舍五入
                                var n = Common.Double2DecimalCalculate(OrderMoney % 10); //个位以后
                                var u = n - Common.Double2DecimalCalculate(n % 1); //个位
                                DiscountMoney = u >= 5 ? n - 10 : Common.Double2DecimalCalculate(OrderMoney % 10);
                                break;
                            case 0: //无
                            default:
                                DiscountMoney = 0;
                                break;
                        }
                    }
                    else
                    {
                        // 客户
                        //DiscountMoney = (Customer != null && Customer.Sale == 1) ? Common.Double2DecimalCalculate(OrderMoney % 10) : 0;   /// 修改
                        if(Customer != null)
                        {
                            if(Customer.Sale == 1)  
                            {
                                // 抹零
                                //DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 10);
                                switch (Config.SYSTEM_SETTING.CustomerDiscount)
                                {
                                    case 1: //个位抹零
                                        DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 10);
                                        break;
                                    case 2: //小数点抹零
                                        DiscountMoney = Common.Double2DecimalCalculate(OrderMoney % 1);
                                        break;
                                    case 3: //四舍五入
                                        var n = Common.Double2DecimalCalculate(OrderMoney % 10); //个位以后
                                        var u = n - Common.Double2DecimalCalculate(n % 1); //个位
                                        DiscountMoney = u >= 5 ? n - 10 : Common.Double2DecimalCalculate(OrderMoney % 10);
                                        break;
                                    case 0: //无
                                    default:
                                        DiscountMoney = 0;
                                        break;
                                }
                            }
                            else
                            {
                                //不抹零
                                DiscountMoney = 0;//Common.Double2DecimalCalculate(OrderMoney % 1);
                            }
                        }
                        else
                        {
                            DiscountMoney = 0;
                        }
                    }
                }
                RealMoney = Common.Double2DecimalCalculate(OrderMoney - DiscountMoney);
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
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
                        CarNetWeight = Common.Double2DecimalCalculate(CarGrossWeight - CarTare);
                        break;
                    case "CarNetWeight":
                        //Calculate(0);
                        break;
                }
            }
            switch(propertyName)
            {
                case "CarNetWeight":
                    if ("".Equals(OrderNo))
                        Cubic = Common.Double2DecimalCalculate(CarNetWeight / 2.38);
                    break;
            }
            CalculatePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
