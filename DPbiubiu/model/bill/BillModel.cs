using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.bill
{
    /// <summary>
    /// 交账管理Model类
    /// </summary>
    public class BillModel :BaseModel
    {
        /// <summary>
        /// 零售
        /// </summary>
        public List<BillDataModel> Retail { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public List<BillDataModel> Customer { get; set; }
        public List<CusTotalDataModel> CusTotal { get; set; }
        // 公司ID
        public string CompanyId { get; set; }
        // 总单数
        public int Count { get; set; }
        // 总重量
        public double Weight { get; set; }
        // 总钱数
        public double Money { get; set; }
        // 提单人姓名
        public string CreateUserName { get; set; }

        public void CopyBill(BillModel bm)
        {
            Retail = bm.Retail;
            Customer = bm.Customer;
            CusTotal = bm.CusTotal;
            Count = bm.Count;
            Weight = bm.Weight;
            Money = bm.Money;
        }
    }
}
