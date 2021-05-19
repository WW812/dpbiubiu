using biubiu.model.role;
using biubiu.view_model.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.user
{
    public class User :BaseModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string NickName { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        /// <summary>
        /// 0管理员，1为用户
        /// </summary>
        public int Type { get; set; }
        public string RoleID { get; set; }

        public UserViewModel ToUserViewModel()
        {
            return new UserViewModel
            {
                ID = ID,
                UserName = UserName,
                PassWord = PassWord,
                NickName = NickName,
                Type = Type,
                UserOfRole = RoleID,
                Note = Note
            };
        }
    }
}
