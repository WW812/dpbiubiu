using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace biubiu.model
{
    public class NetTest
    {

        // 定义一个静态变量来保存类的实例
        private static NetTest _uniqueInstance;
        //public readonly static IApi ApiClient = HttpApiFactory.Create<IApi>();

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        private static readonly Dictionary<string, CancellationTokenSource> _requestPool = new Dictionary<string, CancellationTokenSource>();


        private NetTest()
        {

        }


        /// <summary>
        /// 访问成功回调
        /// </summary>
        public delegate void ApiSuccess(PageModel pm);
        /// <summary>
        /// 访问失败回调
        /// </summary>
        public delegate void ApiFaild(object obj);

        // 无参委托
        public delegate ITask<DataInfo<T>> ApiDelegate<T>();
        // 有参委托
        public delegate ITask<DataInfo<T>> ApiDelegateArg<T>([JsonContent] object param, CancellationToken token);
        //无参 
        public async Task<DataInfo<T>> GetApiData<T>(ApiDelegate<T> apiDel, ApiSuccess apiSuccess = null, ApiFaild apiFaild = null)
        {
            //T TResult = default;
            DataInfo<T> TResult = System.Activator.CreateInstance<DataInfo<T>>();
            try
            {
                var Result = await apiDel();
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    apiSuccess?.Invoke(Result.Page);
                    return Result;
                }
            }
            catch (Exception er)
            {
                apiFaild?.Invoke(er);
                var msg = "";
                if (er.InnerException != null)
                    msg += er.InnerException.Message + "\n";
                msg += er.Message;
                if (er is TaskCanceledException || er.InnerException is TaskCanceledException) msg = "服务器请求超时，请稍后重试！";
                BiuMessageBoxWindows.BiuShow(msg, image: BiuMessageBoxImage.Error);
                return TResult;
                //return null;
            }
        }
        //有参 
        public async Task<DataInfo<T>> GetApiDataArg<T>(ApiDelegateArg<T> apiDel, object param, ApiSuccess apiSuccess = null, ApiFaild apiFaild = null)
        {
            //T TResult = default(T);
            DataInfo<T> TResult = System.Activator.CreateInstance<DataInfo<T>>();
            try
            {
                var c = new CancellationTokenSource();
                if (_requestPool.ContainsKey(apiDel.Method.Name))
                {
                    _requestPool[apiDel.Method.Name].Cancel(false);
                    _requestPool[apiDel.Method.Name] = c;
                }
                else
                {
                    _requestPool.Add(apiDel.Method.Name, c);
                }

                var Result = await apiDel(param, c.Token);

                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else //增加回调
                {
                    apiSuccess?.Invoke(Result.Page);
                    return Result;
                }
            }
            catch (ObjectDisposedException er)
            { //CancellationTokenSource被释放时调用Cancel方法抛出异常
                apiFaild?.Invoke(er);
                Console.WriteLine("ObjectDisposedException异常");
                return TResult;
            }
            catch (Exception er)
            {
                apiFaild?.Invoke(er);
                var msg = "";
                if (er.InnerException != null)
                    msg += er.InnerException.Message + "\n";
                msg += er.Message;
                if (er is TaskCanceledException || er.InnerException is TaskCanceledException) msg = "服务器请求超时，请稍后重试！";
                if(!(er.InnerException is TaskCanceledException && 
                    true == _requestPool[apiDel.Method.Name].IsCancellationRequested))
                    BiuMessageBoxWindows.BiuShow(msg, image: BiuMessageBoxImage.Error);
                return TResult;
                //return null;
            }
            finally
            {
                /*
                if (_requestPool[apiDel.Method.Name] != null)
                    _requestPool[apiDel.Method.Name].Dispose();
                _requestPool.Remove(apiDel.Method.Name);
                */
            }
        }

        public static NetTest GetInstance()
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
                        _uniqueInstance = new NetTest();
                    }
                }
            }
            return _uniqueInstance;
        }

    }
}
