using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.FactoryService
{
    public class StoneCarModel : BaseModel
    {
        //队伍id
        public string TeamID { get; set; }
        //车主
        public string User { get; set; }
        //车号
        public string CarID { get; set; }
        //编号
        public string CarNumber { get; set; }
        //皮重
        public double CarTare { get; set; }
        //RFID
        public string Rfid { get; set; }
        //车型
        public string CarType{get;set;}
        //联系方式
        public string Contact { get; set; }
        //公司ID
        public string CompanyId { get; set; }
    }
}
