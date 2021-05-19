using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.FactoryService
{
    public class StoneTotalModel : BaseModel
    {
        public int MonthSize { get; set; }
        public double MonthWeight { get; set; }
        public double TotalWeight { get; set; }
    }
}
