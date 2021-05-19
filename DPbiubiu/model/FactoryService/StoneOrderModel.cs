using biubiu.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.FactoryService
{
    public class StoneOrderModel :BaseModel
    {
        public string OrderID { get; set; }
        public string CarID { get; set; }
        public string User { get; set; }
        public double CarTare { get; set; }
        public double CarGrossWeight { get; set; }
        public double CarNetWeight { get; set; }
        private double _intervals;
        public double Intervals { 
            get {
                return _intervals;
            } set {
                _intervals = Common.Double2DecimalCalculate(value / 60000);
            } }
        /// <summary>
        /// 出厂时间
        /// </summary>
        public long? UpdateTime { get; set; }

        #region 传参用
        public string TeamID { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
        #endregion
    }
}
