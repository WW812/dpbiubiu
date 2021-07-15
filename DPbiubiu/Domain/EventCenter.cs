using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    public class EventCenter
    {
        private static Dictionary<EventType, Delegate> m_EventTable = new Dictionary<EventType, Delegate>();

        //注册监听事件
        public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
        {
            if (!m_EventTable.ContainsKey(eventType))
            {
                m_EventTable.Add(eventType, null);
            }

            Delegate d = m_EventTable[eventType];
            if (d != null && d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("添加监听错误：当前尝试为事件类型{0}添加不同的委托，原本的委托是{1}，现要添加的委托是{2}", eventType,
                    d.GetType(),
                    callBack.GetType()));
            }
            m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
        }

        //移除监听事件
        public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
        {
            if (m_EventTable.ContainsKey(eventType))
            {
                Delegate d = m_EventTable[eventType];
                if (d == null)
                {
                    throw new Exception(string.Format("移除监听错误：事件{0}不存在委托", eventType));
                }
                else if (d.GetType() != callBack.GetType())
                {
                    throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同的委托，原先的委托为{1}，现在要移除的委托为{2}", eventType, d.GetType(), callBack.GetType()));
                }
            }
            else
            {
                throw new Exception(string.Format("移除监听错误：不存在事件{0}", eventType));
            }
            m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
            if (m_EventTable[eventType] == null)
            {
                m_EventTable.Remove(eventType);
            }
        }

        //广播事件
        public static void Broadcast<T>(EventType eventType, T arg)
        {
            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is CallBack<T> callBack)
                {
                    callBack(arg);
                }
                else
                {
                    throw new Exception(string.Format("事件广播错误：事件{0}存在不同的委托类型", eventType));
                }
            }
        }
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum EventType
    {
        Ponder,
        LJYZN_RFID
    }

    /// <summary>
    /// 回调
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    public delegate void CallBack<T>(T arg);

    public class PonderEventInfomation{
        public string Name;
        public string Weight;
        public string Error;
        public bool Reset = false; // 重置显示
    }

    public class LJYZN_RFIDEventInfomation
    {
        public int Code; // -2: 未知异常； -1:连接异常（未找到发卡器）; 0: 通讯正常; 1: 读取数据; 2: 其他; 
        public string Data;
        public string Error;
    }
}
