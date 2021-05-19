using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    /// <summary>
    /// 单例Message 类
    /// </summary>
    public class SnackbarViewModel : INotifyPropertyChanged
    {
        // 定义一个静态变量来保存类的实例
        private static SnackbarViewModel _uniqueInstance;

        // SnackBar 实例
        private static Snackbar _snackBar;

        /*
        private string _message = "";
        private string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private Boolean _isActive = false;
        private Boolean IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                NotifyPropertyChanged("IsActive");
            }
        }
        */

        private readonly Queue<string> MsgPool = new Queue<string>();

        //private readonly int DisplayMilliscond = 3000;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        private SnackbarViewModel()
        {
        }

        public void SetSanckbar(Snackbar snackBar)
        {
            _snackBar = snackBar;
            _snackBar.MessageQueue = new SnackbarMessageQueue();
        }

        public void PoupMessageAsync(string msg, int? Milliscond = null)
        {
            /*
            if (MsgPool.Count > 0 && msg.Equals(MsgPool.Peek())) Milliscond = 800;
            MsgPool.Enqueue(msg);
            var dm = Milliscond ?? DisplayMilliscond;
            while (_snackBar!= null && MsgPool.Count > 0)
            {
                Thread.Sleep(180);
                if (_snackBar.IsActive) continue;
                //_snackBar.Message = MsgPool.Dequeue();
                _snackBar.MessageQueue.Enqueue(MsgPool.Dequeue());
                _snackBar.IsActive = true;
                await Task.Delay(dm);
                _snackBar.IsActive = false;
            }
            */
            if (_snackBar != null)
            {
                _snackBar.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    _snackBar.MessageQueue.Enqueue(msg);
                });
            }
        }

        public static SnackbarViewModel GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (_uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (_uniqueInstance == null)
                    {
                        _uniqueInstance = new SnackbarViewModel();
                    }
                }
            }
            return _uniqueInstance;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
