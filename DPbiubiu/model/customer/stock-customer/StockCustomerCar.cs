using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.customer.stock_customer
{
    public class StockCustomerCar : BaseModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        private string _carId = "";
        public string CarId
        {
            get { return _carId; }
            set
            {
                _carId = value;
                NotifyPropertyChanged("CarId");
            }
        }

        /// <summary>
        /// 车主
        /// </summary>
        private string _owner = "";
        public string Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                NotifyPropertyChanged("Owner");
            }
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        private string _contact = "";
        public string Contact
        {
            get { return _contact; }
            set
            {
                _contact = value;
                NotifyPropertyChanged("Contact");
            }
        }

        private StockCustomer _customer;
        public StockCustomer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        public override string ToString()
        {
            return CarId + " (客户:" + Customer?.Name + ")";
        }
    }
}
