using biubiu.Domain;
using biubiu.model.customer.ship_customer;
using biubiu.model.ship_goods;
using biubiu.model.ship_order;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.model.print
{
    public class ShipOrderDataSet
    {
        //订单号
        public string OrderNo
        {
            get;
            set;
        }
        //进厂单号 => 改为编号
        public string EnterOrderNo
        {
            get;
            set;
        }
        //皮重
        public double CarTare
        {
            get;
            set;
        }
        //车号
        public string CarId
        {
            get;
            set;
        }
        //料品单价
        public double GoodsPrice
        {
            get;
            set;
        }
        //料品执行价格
        public double GoodsRealPrice
        {
            get;
            set;
        }
        public string GoodsName
        {
            get;
            set;
        }
        //毛重
        public double CarGrossWeight
        {
            get;
            set;
        }
        //净重
        public double CarNetWeight
        {
            get;
            set;
        }
        public string CustomerName
        {
            get;
            set;
        }
        public double CustomerBalance { get; set; }
        public bool CustomerShowMoney { get; set; }
        //应收金额
        public double OrderMoney
        {
            get;
            set;
        }
        //优惠金额
        public double DiscountMoney
        {
            get;
            set;
        }
        //实收金额
        public double RealMoney
        {
            get;
            set;
        }
        //状态码 0新订单,1付款订单 2已交帐
        public string Status
        {
            get;
            set;
        }
        //是否空车,0不是空车,1空车
        public string EmptyCar
        {
            get;
            set;
        }
        //0,正常,1，假删
        public string Hedge
        {
            get;
            set;
        }
        //单子类型：0自然生成  1，补单
        public string Type
        {
            get;
            set;
        }
        //客户和非客户 0零售 1客户
        public string CustomerType
        {
            get;
            set;
        }
        //预装吨数，只在进场时侯有用
        public double AdvanceWeight
        {
            get;
            set;
        }
        //进厂地磅号
        public string EnterPonderation
        {
            get;
            set;
        }
        //出厂地磅号
        public string ExitPonderation
        {
            get;
            set;
        }
        //修改时间
        public string EditTime
        {
            get;
            set;
        }
        //修改人
        public string EditUser
        {
            get;
            set;
        }
        //修改原因
        public string EditReason
        {
            get;
            set;
        }
        //修改备注
        public string EditNote
        {
            get;
            set;
        }
        //进厂时间
        public string EnterTime
        {
            get;
            set;
        }
        //出厂时间
        public string ExitTime
        {
            get;
            set;
        }
        //进厂司磅员
        public string EnterUser
        {
            get;
            set;
        }
        //出厂司磅员
        public string ExitUser
        {
            get;
            set;
        }
        //修改次数
        public int ModifyNum
        {
            get;
            set;
        }
        public string ID { get; set; }
        public string Note
        { get; set; }
        // 是否删除 0 正常 1 删除
        public string Deleted { get; set; }

        public ShipOrderDataSet()
        {
            OrderNo = "XS00002112858672";
            EnterOrderNo = "ZC00002145856847";
            CarTare = 23.85;
            CarId = "鲁A12345";
            GoodsPrice = 50;
            GoodsRealPrice = 40;
            GoodsName = "1-2";
            CarGrossWeight = 53.25;
            CarNetWeight = 29.4;
            CustomerName = "矩子信息";
            CustomerBalance = 3000;
            OrderMoney = 1176;
            DiscountMoney = 6;
            RealMoney = 1170;
            EmptyCar = "载重出厂";
            Type = "正常过磅";
            CustomerType = "现金零售";
            AdvanceWeight = 30;
            EnterPonderation = "1号地磅";
            ExitPonderation = "2号地磅";
            EnterTime = "2018-9-23 16:34";
            ExitTime = "2018-9-23 15:02";
            EditTime = "2018-9-24 8:58";
            EnterUser = "司磅";
            ExitUser = "司磅";
            Status = "未结账";
            CustomerShowMoney = true;
        }

        public void ByShipOrder(ShipOrder order)
        {
            if (Config.AdjustPrintEnabled)
            {
                order.CarGrossWeight += Config.SHIP_EXIT_CONFIG.AdjustWeight;
                order.Calculate();
            }
            ID = order.ID;
            OrderNo = order.OrderNo;
            EnterOrderNo = order.EnterOrderNo;
            CarTare = order.CarTare;
            CarId = order.CarId;
            GoodsPrice = order.GoodsPrice;
            GoodsRealPrice = order.GoodsRealPrice;
            GoodsName = order.Goods.Name;
            CarGrossWeight = order.CarGrossWeight;
            CarNetWeight = order.CarNetWeight;
            if (order.Customer != null)
            {
                CustomerName = order.Customer.Name;
                CustomerBalance = order.Customer.Balance;
                CustomerShowMoney = order.Customer.ShowMoney == 0;
            }
            else
            {
                CustomerName = "";
                CustomerBalance = 0.0;
                CustomerShowMoney = false;
            }
            OrderMoney = order.OrderMoney;
            DiscountMoney = order.DiscountMoney;
            RealMoney = order.RealMoney;
            EmptyCar = order.EmptyCar == 0 ? "空车出厂" : "载重出厂";
            Type = order.Type == 0 ? "正常过磅" : "补单";
            CustomerType = order.CustomerType == 0 ? "现金流水" : order.Customer?.Name ?? "";
            AdvanceWeight = order.AdvanceWeight;
            EnterPonderation = order.EnterPonderation;
            ExitPonderation = order.ExitPonderation;
            EnterTime = order.Type == 0 ? Common.TimeStamp2DateTime(order.EnterTime).ToString("yyyy-MM-dd HH:mm") : "#" ;
            ExitTime = Common.TimeStamp2DateTime(order.ExitTime).ToString("yyyy-MM-dd HH:mm");
            EnterUser = order.EnterUser == null? "" : order.EnterUser.NickName;
            ExitUser = order.ExitUser == null ? "" : order.ExitUser.NickName;
            Status = order.Status == 0 ?"未结账": "已结账";

            
        }
    }
}
