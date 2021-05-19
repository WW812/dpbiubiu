using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.model.stock_order
{
    /// <summary>
    /// 用来控制进料面板列表显示
    /// </summary>
    public class StockOrderColumnsVisibility : INotifyPropertyChanged
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
        private Visibility _deductWeightVisibility; //扣吨
        public Visibility DeductWeightVisibility
        {
            get { return _deductWeightVisibility; }
            set { _deductWeightVisibility = value;
                NotifyPropertyChanged("DeductWeightVisibility");
            }
        }
        private Visibility _freightOfTonVisibility; //运费
        public Visibility FreightOfTonVisibility
        {
            get { return _freightOfTonVisibility; }
            set { _freightOfTonVisibility = value;
                NotifyPropertyChanged("FreightOfTonVisibility");
            }
        }
        private Visibility _paidVisibility; //是否结算
        public Visibility PaidVisibility
        {
            get { return _paidVisibility; }
            set
            {
                _paidVisibility = value;
                NotifyPropertyChanged("PaidVisibility");
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
        public Visibility CarNetWeightVisibility
        {
            get
            {
                return _carNetWeightVisibility;
            }
            set
            {
                _carNetWeightVisibility = value;
                NotifyPropertyChanged("CarNetWeightVisibility");
            }
        }
        private Visibility _realMoneyVisibility;
        public Visibility RealMoneyVisibility
        {
            get { return _realMoneyVisibility; }
            set
            {
                _realMoneyVisibility = value;
                NotifyPropertyChanged("RealMoneyVisibility");
            }
        }
        private Visibility _exitTimeVisibility;
        public Visibility ExitTimeVisibility
        {
            get
            {
                return _exitTimeVisibility;
            }
            set
            {
                _exitTimeVisibility = value;
                NotifyPropertyChanged("ExitTimeVisibility");
            }
        }
        private Visibility _exitUserVisibility;
        public Visibility ExitUserVisibility
        {
            get
            {
                return _exitUserVisibility;
            }
            set
            {
                _exitUserVisibility = value;
                NotifyPropertyChanged("ExitUserVisibility");
            }
        }
        private Visibility _exitPonderationVisibility;
        public Visibility ExitPonderationVisibility
        {
            get { return _exitPonderationVisibility; }
            set
            {
                _exitPonderationVisibility = value;
                NotifyPropertyChanged("ExitPonderationVisibility");
            }
        }
        private Visibility _editUserVisibility;
        public Visibility EditUserVisibility
        {
            get
            {
                return _editUserVisibility;
            }
            set
            {
                _editUserVisibility = value;
                NotifyPropertyChanged("EditUserVisibility");
            }
        }
        private Visibility _editTimeVisibility;
        public Visibility EditTimeVisibility
        {
            get { return _editTimeVisibility; }
            set
            {
                _editTimeVisibility = value;
                NotifyPropertyChanged("EditTimeVisibility");
            }
        }
        private Visibility _editReasonVisibility;
        public Visibility EditReasonVisibility
        {
            get { return _editReasonVisibility; }
            set
            {
                _editReasonVisibility = value;
                NotifyPropertyChanged("EditReasonVisibility");
            }
        }
        private Visibility _editNoteVisibility;
        public Visibility EditNoteVisibility
        {
            get { return _editNoteVisibility; }
            set
            {
                _editNoteVisibility = value;
                NotifyPropertyChanged("EditNoteVisibility");
            }
        }

        public StockOrderColumnsVisibility()
        {
            Enter();
        }

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
            }
        }

        // 进厂单据
        private void Enter()
        {
            //基本
            CustomerNameVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Hidden;
            GoodsNameVisibility = Visibility.Visible;
            GoodsRealPriceVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Visible;
            EnterUserVisibility = Visibility.Visible;
            EnterPonderationVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Hidden;
            DeductWeightVisibility = Visibility.Hidden;
            FreightOfTonVisibility = Visibility.Visible; //运费
            PaidVisibility = Visibility.Hidden;
            //进
            EnterOrderNoVisibility = Visibility.Visible;
            //出
            OrderNoVisibility = Visibility.Hidden;
            CarGrossWeightVisibility = Visibility.Visible;
            CarNetWeightVisibility = Visibility.Hidden;
            RealMoneyVisibility = Visibility.Hidden;
            ExitTimeVisibility = Visibility.Hidden;
            ExitUserVisibility = Visibility.Hidden;
            ExitPonderationVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
        }
        // 出厂单据
        private void Exit()
        {
            //基本
            CustomerNameVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Hidden;
            GoodsNameVisibility = Visibility.Visible;
            GoodsRealPriceVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Hidden;
            EnterUserVisibility = Visibility.Hidden;
            EnterPonderationVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Hidden;
            DeductWeightVisibility = Visibility.Visible;
            FreightOfTonVisibility = Visibility.Visible; //运费
            PaidVisibility = Visibility.Visible;
            //进
            EnterOrderNoVisibility = Visibility.Hidden;
            //出
            OrderNoVisibility = Visibility.Visible;
            CarGrossWeightVisibility = Visibility.Hidden;
            CarNetWeightVisibility = Visibility.Visible;
            RealMoneyVisibility = Visibility.Visible;
            ExitTimeVisibility = Visibility.Visible;
            ExitUserVisibility = Visibility.Visible;
            ExitPonderationVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
        }

        private void Mend()
        {
            //基本
            CustomerNameVisibility = Visibility.Visible;
            CarIdVisibility = Visibility.Visible;
            CarTareVisibility = Visibility.Visible;
            GoodsNameVisibility = Visibility.Visible;
            GoodsRealPriceVisibility = Visibility.Visible;
            EnterTimeVisibility = Visibility.Hidden;
            EnterUserVisibility = Visibility.Hidden;
            EnterPonderationVisibility = Visibility.Hidden;
            NoteVisibility = Visibility.Visible;
            //进
            EnterOrderNoVisibility = Visibility.Hidden;
            //出
            OrderNoVisibility = Visibility.Visible;
            CarGrossWeightVisibility = Visibility.Visible;
            CarNetWeightVisibility = Visibility.Visible;
            RealMoneyVisibility = Visibility.Visible;
            ExitTimeVisibility = Visibility.Hidden;
            ExitUserVisibility = Visibility.Visible;
            ExitPonderationVisibility = Visibility.Hidden;
            EditUserVisibility = Visibility.Hidden;
            EditTimeVisibility = Visibility.Hidden;
            EditReasonVisibility = Visibility.Hidden;
            EditNoteVisibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
