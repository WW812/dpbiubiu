using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.model.ship_order
{
    /// <summary>
    /// 用来控制出料面板列表显示
    /// </summary>
    public class ShipOrderColumnsVisibility : INotifyPropertyChanged
    {
        //基本
        private Visibility _customerNameVisibility;
        public Visibility CustomerNameVisibility
        {
            get { return _customerNameVisibility; }
            set
            {
                _customerNameVisibility = value;
                NotifyPropertyChanged("CustomerNameVisibility");
            }
        }
        private Visibility _carIdVisibility;
        public Visibility CarIdVisibility
        {
            get { return _carIdVisibility; }
            set
            {
                _carIdVisibility = value;
                NotifyPropertyChanged("CarIdVisibility");
            }
        }
        private Visibility _carTareVisibility;
        public Visibility CarTareVisibility
        {
            get
            {
                return _carTareVisibility;
            }
            set
            {
                _carTareVisibility = value;
                NotifyPropertyChanged("CarTareVisibility");
            }
        }
        private Visibility _goodsNameVisibility;
        public Visibility GoodsNameVisibility
        {
            get
            {
                return _goodsNameVisibility;
            }
            set
            {
                _goodsNameVisibility = value;
                NotifyPropertyChanged("GoodsNameVisibility");
            }
        }
        private Visibility _goodsPriceVisibility;
        public Visibility GoodsPriceVisibility
        {
            get { return _goodsNameVisibility; }
            set
            {
                _goodsPriceVisibility = value;
                NotifyPropertyChanged("GoodsPriceVisibility");
            }
        }
        private Visibility _goodsRealPriceVisibility;
        public Visibility GoodsRealPriceVisibility
        {
            get { return _goodsRealPriceVisibility; }
            set
            {
                _goodsRealPriceVisibility = value;
                NotifyPropertyChanged("GoodsRealPriceVisibility");
            }
        }
        private Visibility _enterTimeVisibility;
        public Visibility EnterTimeVisibility
        {
            get
            {
                return _enterTimeVisibility;
            }
            set
            {
                _enterTimeVisibility = value;
                NotifyPropertyChanged("EnterTimeVisibility");
            }
        }
        private Visibility _enterUserVisibility;
        public Visibility EnterUserVisibility
        {
            get
            {
                return _enterUserVisibility;
            }
            set
            {
                _enterUserVisibility = value;
                NotifyPropertyChanged("EnterUserVisibility");
            }
        }
        private Visibility _enterPonderationVisibility;
        public Visibility EnterPonderationVisibility
        {
            get { return _enterPonderationVisibility; }
            set
            {
                _enterPonderationVisibility = value;
                NotifyPropertyChanged("EnterPonderationVisibility");
            }
        }
        private Visibility _noteVisibility;
        public Visibility NoteVisibility
        {
            get { return _noteVisibility; }
            set
            {
                _noteVisibility = value;
                NotifyPropertyChanged("NoteVisibility");
            }
        }

        //进
        private Visibility _enterOrderNoVisibility;
        public Visibility EnterOrderNoVisibility
        {
            get
            {
                return _enterOrderNoVisibility;
            }
            set
            {
                _enterOrderNoVisibility = value;
                NotifyPropertyChanged("EnterOrderNoVisibility");
            }
        }

        //出
        private Visibility _orderNoVisibility;
        public Visibility OrderNoVisibility
        {
            get
            {
                return _orderNoVisibility;
            }
            set
            {
                _orderNoVisibility = value;
                NotifyPropertyChanged("OrderNoVisibility");
            }
        }
        private Visibility _carGrossWeightVisibility;
        public Visibility CarGrossWeightVisibility
        {
            get
            {
                return _carGrossWeightVisibility;
            }
            set
            {
                _carGrossWeightVisibility = value;
                NotifyPropertyChanged("CarGrossWeightVisibility");
            }
        }
        private Visibility _carNetWeightVisibility;
        public Visibility CarNetWeightVisibility { get {
                return _carNetWeightVisibility;
            } set { _carNetWeightVisibility = value;
                NotifyPropertyChanged("CarNetWeightVisibility");
            } }
        private Visibility _orderMoneyVisibility;
        public Visibility OrderMoneyVisibility { get {
                return _orderMoneyVisibility;
            } set {
                _orderMoneyVisibility = value;
                NotifyPropertyChanged("OrderMoneyVisibility");
            } }
        private Visibility _discountMoneyVisibility;
        public Visibility DiscountMoneyVisibility { get {
                return _discountMoneyVisibility;
            } set { _discountMoneyVisibility = value;
                NotifyPropertyChanged("DiscountMoneyVisibility");
            } }
        private Visibility _realMoneyVisibility;
        public Visibility RealMoneyVisibility { get { return _realMoneyVisibility; } set {
                _realMoneyVisibility = value;
                NotifyPropertyChanged("RealMoneyVisibility");
            } }
        private Visibility _exitTimeVisibility;
        public Visibility ExitTimeVisibility { get {
                return _exitTimeVisibility;
            } set { _exitTimeVisibility = value;
                NotifyPropertyChanged("ExitTimeVisibility");
            } }
        private Visibility _exitUserVisibility;
        public Visibility ExitUserVisibility { get {
                return _exitUserVisibility;
            } set { _exitUserVisibility = value;
                NotifyPropertyChanged("ExitUserVisibility");
            } }
        private Visibility _exitPonderationVisibility;
        public Visibility ExitPonderationVisibility { get { return _exitPonderationVisibility; } set {
                _exitPonderationVisibility = value;
                NotifyPropertyChanged("ExitPonderationVisibility");
            } }
        private Visibility _editUserVisibility;
        public Visibility EditUserVisibility { get {
                return _editUserVisibility;
            } set { _editUserVisibility = value;
                NotifyPropertyChanged("EditUserVisibility");
            } }
        private Visibility _editTimeVisibility;
        public Visibility EditTimeVisibility { get { return _editTimeVisibility; } set {
                _editTimeVisibility = value;
                NotifyPropertyChanged("EditTimeVisibility");
            } }
        private Visibility _editReasonVisibility;
        public Visibility EditReasonVisibility { get { return _editReasonVisibility; } set {
                _editReasonVisibility = value;
                NotifyPropertyChanged("EditReasonVisibility");
            } }
        private Visibility _editNoteVisibility;
        public Visibility EditNoteVisibility { get { return _editNoteVisibility; } set {
                _editNoteVisibility = value;
                NotifyPropertyChanged("EditNoteVisibility");
            } }

        private Visibility _cubicVisibility;
        public Visibility CubicVisibility
        {
            get { return _cubicVisibility; }
            set
            {
                _cubicVisibility = value;
                NotifyPropertyChanged("CubicVisibility");
            }
        }


        public ShipOrderColumnsVisibility()
        {
            /*
            CustomerNameVisibility = true;
            CarIdVisibility = true;
            CarTareVisibility = true;
            GoodsNameVisibility = true;
            GoodsPriceVisibility = true;
            GoodsRealPriceVisibility = true;
            EnterTimeVisibility = true;
            EnterUserVisibility = true;
            EnterPonderationVisibility = true;
            NoteVisibility = true;
            */
            Enter();
        }

        /// <summary>
        /// 设置列模式
        /// </summary>
        /// <param name=""></param>
        public void ColumnsMode(int socm)
        {
            switch (socm)
            {
                case 0:
                    Enter();
                    break;
                case 1:
                    Exit();
                    break;
                case 2:
                    Empty();
                    break;
            }
        }

        //进厂单据
        private void Enter()
        {
            //基本
            CustomerNameVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Visible;
            GoodsNameVisibility = Visibility.Visible;
            GoodsPriceVisibility = Visibility.Visible;
            GoodsRealPriceVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Visible;
            EnterUserVisibility = Visibility.Visible;
            EnterPonderationVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Hidden;
            //进
            EnterOrderNoVisibility = Visibility.Visible;
            //出
            OrderNoVisibility = Visibility.Hidden;
            CarGrossWeightVisibility = Visibility.Hidden;
            CarNetWeightVisibility = Visibility.Hidden;
            OrderMoneyVisibility = Visibility.Hidden;
            DiscountMoneyVisibility = Visibility.Hidden;
            RealMoneyVisibility = Visibility.Hidden;
            ExitTimeVisibility = Visibility.Hidden;
            ExitUserVisibility = Visibility.Hidden;
            ExitPonderationVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
            CubicVisibility = Visibility.Hidden;
        }

        //出场单据
        private void Exit()
        {
            //基本
            CustomerNameVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Visible;
            GoodsNameVisibility = Visibility.Visible;
            GoodsPriceVisibility = Visibility.Hidden;
            GoodsRealPriceVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Hidden;
            EnterUserVisibility = Visibility.Hidden;
            EnterPonderationVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Hidden;
            //进
            EnterOrderNoVisibility = Visibility.Hidden;
            //出
            OrderNoVisibility = Visibility.Visible;
            CarGrossWeightVisibility = Visibility.Visible;
            CarNetWeightVisibility = Visibility.Visible;
            OrderMoneyVisibility = Visibility.Hidden;
            DiscountMoneyVisibility = Visibility.Hidden;
            RealMoneyVisibility = Visibility.Visible;
            ExitTimeVisibility = Visibility.Visible;
            ExitUserVisibility = Visibility.Visible;
            ExitPonderationVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
            CubicVisibility = Visibility.Hidden;
        }

        //空车单据
        private void Empty()
        {
            OrderNoVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CustomerNameVisibility = Visibility.Visible;
            CarGrossWeightVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Visible;
            EnterUserVisibility = Visibility.Hidden;
            EnterPonderationVisibility = Visibility.Hidden;
            ExitTimeVisibility = Visibility.Visible;
            ExitUserVisibility = Visibility.Visible;
            ExitPonderationVisibility = Visibility.Hidden;
            GoodsNameVisibility = Visibility.Visible;
            GoodsPriceVisibility = Visibility.Hidden;
            CarNetWeightVisibility = Visibility.Hidden;
            GoodsRealPriceVisibility = Visibility.Hidden;
            OrderMoneyVisibility = Visibility.Hidden;
            DiscountMoneyVisibility = Visibility.Hidden;
            RealMoneyVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Hidden;
            EnterOrderNoVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
            CubicVisibility = Visibility.Hidden;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
