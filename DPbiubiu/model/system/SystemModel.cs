using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.system
{
    public class SystemModel :BaseModel
    {

        /// <summary>
        /// 零售出料单据优惠
        /// </summary>
        private int _shipOrderDiscount;
        public int ShipOrderDiscount
        {
            get { return _shipOrderDiscount; }
            set
            {
                _shipOrderDiscount = value;
                NotifyPropertyChanged("ShipOrderDiscount");
            }
        }

        /// <summary>
        /// 客户出料单据优惠
        /// </summary>
        private int _customerDiscount;
        public int CustomerDiscount
        {
            get { return _customerDiscount; }
            set
            {
                _customerDiscount = value;
                NotifyPropertyChanged("CustomerDiscount");
            }
        }

        /// <summary>
        /// 短信用磅房电话
        /// </summary>
        private string _weightHomePhone;
        public string WeightHomePhone
        {
            get { return _weightHomePhone; }
            set
            {
                _weightHomePhone = value;
                NotifyPropertyChanged("ShipOrderDiscount");
            }
        }

    }
}
