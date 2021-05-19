using biubiu.Domain;
using biubiu.model.customer.stock_customer;
using biubiu.model.goods.stock_goods;
using biubiu.model.stock_order;
using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.print
{
    public class StockOrderDataSet
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
        public string CustomerName { get; set; }

        public double CustomerNonpaymentAmount { get; set; }

        //实付金额
        public double RealMoney
        {
            get;
            set;

        }

        //状态码 0进厂订单，1出厂订单，2已交帐
        public string Status
        {
            get;
            set;
        }
        //单子类型：0正常过磅  1，补单
        public string Type
        {
            get;
            set;
        }
        //客户和非客户 0散户 1客户
        public string CustomerType
        {
            get;
            set;
        }

        // 扣吨
        public double DeductWeight
        {
            get;
            set;
        }

        // 扣吨方式 扣吨方式 1 为百分比 ， 0为直接扣除相应吨数
        public int DeductWeightType
        {
            get;
            set;
        }

        public string DeductWeightTypeText
        {
            get;
            set;
        }

        //每吨运费
        public double FreightOfTon
        {
            get;
            set;
        }

        //是否已结账 0未结 1已结
        public int Paid
        {
            get;
            set;
        }

        //结账展示
        public string PaidText
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
        public User EditUser
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

        public string ID { get; set; }


        /// <summary>
        /// 扣吨展示
        /// </summary>
        public string DeductWeightText
        {
            get;
            set;
        }

        public StockOrderDataSet()
        {
            OrderNo = "JL00001781065461";
            EnterOrderNo = "ZC00000953267982";
            CarTare = 15.35;
            CarId = "鲁A12345";
            GoodsRealPrice = 30;
            GoodsName = "石头";
            CarGrossWeight = 39.3;
            CarNetWeight = 23.95;
            CustomerName = "矩子信息";
            CustomerNonpaymentAmount = 3000;
            RealMoney = 718.5;
            Status = "出厂";
            Type = "正常过磅";
            CustomerType = "散户";
            DeductWeight = 0;
            DeductWeightType = 0;
            DeductWeightTypeText = "扣吨";
            FreightOfTon = 0;
            Paid = 0;
            PaidText = "未支付";
            EnterPonderation = "1号地磅";
            ExitPonderation = "1号地磅";
            EnterTime = "2018-9-23 16:34";
            ExitTime = "2018-9-23 15:02";
            EditTime = "2018-9-24 8:58";
            EnterUser = "司磅";
            ExitUser = "司磅";
            DeductWeightText = "0吨";
        }

        public void ByStockOrder(StockOrder order)
        {
            if (Config.AdjustPrintEnabled)
            {
                order.CarGrossWeight += Config.STOCK_EXIT_CONFIG.AdjustWeight;
                order.Calculate();
            }
            ID = order.ID;
            OrderNo = order.OrderNo;
            EnterOrderNo = order.EnterOrderNo;
            CarTare = order.CarTare;
            CarId = order.CarId;
            GoodsRealPrice = order.GoodsRealPrice;
            GoodsName = order.Goods?.Name ?? "";
            CarGrossWeight = order.CarGrossWeight;
            CarNetWeight = order.CarNetWeight;
            CustomerName = order.Customer?.Name ?? "";
            CustomerNonpaymentAmount = order.Customer?.NonpaymentAmount ?? 0;
            RealMoney = order.RealMoney;
            Status = order.Status == 0 ? "进厂" : "出厂";
            Type = order.Type == 0 ? "正常过磅" : "补单";
            CustomerType = order.CustomerType == 0 ? "散户" : order.Customer?.Name ?? "";
            DeductWeight = order.DeductWeight;
            DeductWeightType = order.DeductWeightType;
            DeductWeightTypeText = order.DeductWeightTypeText;
            FreightOfTon = order.FreightOfTon;
            Paid = order.Paid;
            PaidText = order.PaidText;
            EnterPonderation = order.EnterPonderation;
            ExitPonderation = order.ExitPonderation;
            EnterTime = order.Type == 0 ?Common.TimeStamp2DateTime(order.EnterTime).ToString("yyyy-MM-dd HH:mm"):"#";
            ExitTime = Common.TimeStamp2DateTime(order.ExitTime).ToString("yyyy-MM-dd HH:mm");
            EditTime = Common.TimeStamp2DateTime(order.EditTime).ToString("yyyy-MM-dd HH:mm");
            EnterUser = order.EnterUser?.NickName ?? "";
            ExitUser = order.ExitUser?.NickName ?? "";
            DeductWeightText = order.DeductWeightText;
        }
    }
}
