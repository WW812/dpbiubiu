using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.paytype
{
    public class PayType : BaseModel
    {
        public string Name { get; set; }
        public double Money { get; set; }
    }
}
