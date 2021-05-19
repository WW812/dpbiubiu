using biubiu.Domain.biuMessageBox;
using biubiu.views.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    public class GetAllDataStatus : INotifyPropertyChanged
    {
        public delegate void Start();
        /// <summary>
        /// 当前请求数目
        /// </summary>
        private int _requestNum = 0;
        public int RequestNum
        {
            get { return _requestNum; }
            set
            {
                _requestNum = value;
                NotifyPropertyChanged("RequestNum");
            }
        }

        /// <summary>
        /// 是否全部请求完成
        /// </summary>
        private bool _isAllComplete = true;
        public bool IsAllComplete
        {
            get { return _isAllComplete; }
            set
            {
                _isAllComplete = value;
                NotifyPropertyChanged("IsAllComplete");
            }
        }

        /// <summary>
        /// 开始一个异步请求
        /// </summary>
        /// <param name="s"></param>
        /// <param name="mes">loading显示文字，为null 不加载Loading, 为""空字符串 为默认显示</param>
        public void StartRequest(Start s, string mes = "")
        {
            var c = new CancellationTokenSource(2000);
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (mes != null)
                            LoadingDialog.Show(mes);
                        AddOneRequest();
                        s?.Invoke();
                        //CompleteOneRequest();
                        //LoadingDialog.Close();
                    }
                    catch (Exception er)
                    {
                        BiuMessageBoxWindows.BiuShow(er.Message);
                    }
                    finally
                    {
                        CompleteOneRequest();
                        LoadingDialog.Close();
                        c.Dispose();
                    }
                }, c.Token);
            }
            catch (Exception)
            {
                //CompleteOneRequest();
                BiuMessageBoxWindows.BiuShow("乖乖");
            }
            finally
            {
                CompleteOneRequest();
                c.Dispose();
            }
        }

        /// <summary>
        /// 新增一个请求
        /// </summary>
        public void AddOneRequest()
        {
            RequestNum++;
            if (RequestNum < 0) RequestNum = 0;
            IsAllComplete = RequestNum < 1;
        }

        /// <summary>
        /// 完成一个请求
        /// </summary>
        public void CompleteOneRequest()
        {
            RequestNum--;
            if (RequestNum < 0) RequestNum = 0;
            IsAllComplete = RequestNum < 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            /*
            switch (propertyName)
            {
                case "RequestNum":
                    if (RequestNum < 0) RequestNum = 0;
                    IsAllComplete = RequestNum < 1;
                    break;
            }
            */
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
