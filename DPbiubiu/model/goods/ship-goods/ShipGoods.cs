using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model.ship_goods
{
    public class ShipGoods : BaseModel
    {
        //料品名称
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        //标准价格
        private double _price;
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyPropertyChanged("Price");
            }
        }
        //执行价格
        private double _realPrice;
        public double RealPrice
        {
            get { return _realPrice; }
            set
            {
                _realPrice = value;
                NotifyPropertyChanged("RealPrice");
            }
        }
        //是否启用 0禁用 1启用
        private int _valid;
        public int Valid {
            get { return _valid; }
            set
            {
                _valid = value;
                NotifyPropertyChanged("Valid");
            }
        }

        #region 界面使用
        // 是否在编辑
        private bool _isEditing = false;
        public bool IsEditing
        {
            get { return _isEditing; }
            set { _isEditing = value;
                NotifyPropertyChanged("IsEditing");
            }
        }

        // 编辑标准价格
        private double _editPrice;
        public double EditPrice
        {
            get { return _editPrice; }
            set
            {
                _editPrice = value;
                NotifyPropertyChanged("EditPrice");
            }
        }
        //编辑执行价格
        private double _editRealPrice;
        public double EditRealPrice
        {
            get { return _editRealPrice; }
            set
            {
                _editRealPrice = value;
                NotifyPropertyChanged("EditRealPrice");
            }
        }

        //编辑备注
        private string _editNote;
        public string EditNote
        {
            get { return _editNote; }
            set
            {
                _editNote = value;
                NotifyPropertyChanged("EditNote");
            }
        }


        #endregion
    }
}
