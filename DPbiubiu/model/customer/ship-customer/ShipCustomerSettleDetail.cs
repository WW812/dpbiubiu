using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.ship_customer
{
    public class ShipCustomerSettleDetail
    {
        public StatisticsModel Statistics { get; set; }
    }

    public class StatisticsModel
    {
        public long SettleStartTime { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public double UseMoney { get; set; }
        public double Money { get; set; }
    }
}
