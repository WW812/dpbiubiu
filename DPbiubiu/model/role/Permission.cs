using biubiu.view_model.role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.role
{
    public class Permission : BaseModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Weight { get; set; }

        public PermissionViewModel ToPermissionViewModel()
        {
            return new PermissionViewModel
            {
                ID = ID,
                Name = Name
            };
        }
    }
}
