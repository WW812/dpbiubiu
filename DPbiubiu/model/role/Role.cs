using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.role
{
    /// <summary>
    /// 角色实体类
    /// </summary>
    [Serializable]
    public class Role : BaseModel
    {
        public string Name { get; set; }
        /// <summary>
        /// 0为启用，1为禁用
        /// </summary>
        public int Status { get; set; }
    }
}
