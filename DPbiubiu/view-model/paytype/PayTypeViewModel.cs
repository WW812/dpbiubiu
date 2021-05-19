using biubiu.Domain;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.paytype;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace biubiu.view_model.paytype
{
    public class PayTypeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 账户集合
        /// </summary>
        private ObservableCollection<PayType> _payTypeItems;
        public ObservableCollection<PayType> PayTypeItems
        {
            get { return _payTypeItems; }
            set
            {
                _payTypeItems = value;
                NotifyPropertyChanged("PayTypeItems");
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        private PageModel _currentPage = new PageModel(17);
        public PageModel CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }

        /// <summary>
        /// 跳转到页面
        /// </summary>
        private int _jumpNum = 1;
        public int JumpNum
        {
            get { return _jumpNum; }
            set
            {
                _jumpNum = value;
                NotifyPropertyChanged("JumpNum");
            }
        }

        public PayTypeViewModel()
        {
            GetPayType();
        }

        public ICommand JumpPageCommand => new AnotherCommandImplementation(ExecuteJumpPageCommand);
        public ICommand PrevPageCommand => new AnotherCommandImplementation(ExecutePrevPageCommand);
        public ICommand NextPageCommand => new AnotherCommandImplementation(ExecuteNextPageCommand);

        private void ExecuteJumpPageCommand(object o)
        {
            CurrentPage.Page = JumpNum;
            GetPayType();
        }
        private void ExecutePrevPageCommand(object o)
        {
            if (!CurrentPage.First)
            {
                CurrentPage.Page -= 1;
                GetPayType();
            }
        }
        private void ExecuteNextPageCommand(object o)
        {
            if (!CurrentPage.Last)
            {
                CurrentPage.Page += 1;
                GetPayType();
            }
        }

        /// <summary>
        /// 获取支付类型
        /// </summary>
        public void GetPayType()
        {
            Task.Run(() =>
            {
                PayTypeItems = new ObservableCollection<PayType>(
                    ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.GetPayTypeAsync,
                    new { Page = CurrentPage.Page - 1, Size = CurrentPage.Size },
                    delegate (DataInfo<List<PayType>> result) { CurrentPage = result.Page; }
                    ).Result.Data);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
