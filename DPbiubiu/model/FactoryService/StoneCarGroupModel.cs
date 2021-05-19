using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.FactoryService
{
    /// <summary>
    /// 车队
    /// </summary>
    public class StoneCarGroupModel : BaseModel
    {
        //名称
        public string Name { get; set; } = null;
        //打卡间隔
        public double? SignInterval { get; set; } = null;
        //公司ID
        public string CompanyId { get; set; } = null;
    }
}
