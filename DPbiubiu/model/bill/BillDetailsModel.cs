using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.bill
{
    public class BillDetailsModel
    {
        /// <summary>
        /// 零售
        /// </summary>
        public List<BillDataModel> Retail { get;set;}
        /// <summary>
        /// 客户
        /// </summary>
        public List<BillDataModel> Customer { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public List<CusTotalDataModel> CusTotal { get; set; }
    }

    public class BillDataModel
    {
        /// <summary>
        /// 单数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 钱数
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 料品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 平台金额
        /// </summary>
        public double PlatformMoney { get; set; }
        /// <summary>
        /// 差价
        /// </summary>
        public double PlatDiffMoney { get; set; }
        public BillDataModel()
        {
            Count = 0;
            Money = 0.0;
            GoodsName = "";
            Weight = 0.0;
            PlatformMoney = 0.0;
            PlatDiffMoney = 0.0;
        }
    }

    public class CusTotalDataModel
    {
        /// <summary>
        /// 单数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 钱数
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string CusName { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 料品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 平台金额
        /// </summary>
        public double PlatformMoney { get; set; }
        /// <summary>
        /// 差价
        /// </summary>
        public double PlatDiffMoney { get; set; }
        /// <summary>
        /// 方量
        /// </summary>
        public double Cubic { get; set; }
        public CusTotalDataModel()
        {
            Count = 0;
            Money = 0.0;
            CusName = "";
            Weight = 0.0;
            GoodsName = "";
            Cubic = 0.0;
            PlatformMoney = 0.0;
            PlatDiffMoney = 0.0;
        }
    }
}
