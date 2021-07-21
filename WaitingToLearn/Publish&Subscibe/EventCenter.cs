using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter
{
    private static Dictionary<EventType, Delegate> eventTable = new Dictionary<EventType, Delegate>();


    /// <summary>
    /// 添加Listener前的判定
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="d"></param>
    private static void OnListenerAdding(EventType eventType, Delegate callBack)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }

        Delegate d = eventTable[eventType];

        if(d!=null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("Failed to add listener on event {0}.", eventType));
        }
    }


    /// <summary>
    /// 添加無參監聽
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener(EventType eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack)eventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加單參監聽
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T1>(EventType eventType, CallBack<T1> callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack<T1>)eventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加雙參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T1, T2>(EventType eventType, CallBack<T1, T2> callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2>)eventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加三參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T1, T2, T3>(EventType eventType, CallBack<T1, T2, T3> callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3>)eventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加四參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T1, T2, T3, T4>(EventType eventType, CallBack<T1, T2, T3, T4> callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3, T4>)eventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加五參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T1, T2, T3, T4, T5>(EventType eventType, CallBack<T1, T2, T3, T4, T5> callBack)
    {
        OnListenerAdding(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3, T4, T5>)eventTable[eventType] + callBack;
    }



    /// <summary>
    /// 移除前的判定
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    private static void OnListenerRemoving(EventType eventType, Delegate callBack)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];

            if(d == null)
            {
                throw new Exception("Remove Failed, there are no delegate");
            }
            else if(d.GetType() != callBack.GetType())
            {
                throw new Exception("Remove Failed, wrong type of delegate");
            }
        }
        else
        {
            throw new Exception(string.Format("There are no event {0}", eventType));
        }
    }

    /// <summary>
    /// 移除後清空字典裡的事件碼
    /// </summary>
    /// <param name="eventType"></param>
    private static void OnListenerRemoved(EventType eventType)
    {
        if(eventTable[eventType] == null)
        {
            eventTable.Remove(eventType);
        }
    }


    /// <summary>
    /// 移除無參監聽
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener(EventType eventType, CallBack callBack)
    {

        OnListenerRemoving(eventType, callBack);

        eventTable[eventType] = (CallBack)eventTable[eventType] - callBack;

        OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除單參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T1>(EventType eventType, CallBack<T1> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        eventTable[eventType] = (CallBack<T1>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除雙參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T1, T2>(EventType eventType, CallBack<T1, T2> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除三參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T1, T2, T3>(EventType eventType, CallBack<T1, T2, T3> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除四參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T1, T2, T3, T4>(EventType eventType, CallBack<T1, T2, T3, T4> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3, T4>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除五參監聽
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T1, T2, T3, T4, T5>(EventType eventType, CallBack<T1, T2, T3, T4, T5> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        eventTable[eventType] = (CallBack<T1, T2, T3, T4, T5>)eventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }



    //無參廣播
    public static void BroadCast(EventType eventType)
    {
        Delegate d;
        if(eventTable.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if(callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception("BroadCast Error");
            }
        }
    }

    //單參廣播
    public static void BroadCast<T1>(EventType eventType, T1 arg1)
    {
        Delegate d;
        if(eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T1> callBack = d as CallBack<T1>;
            if(callBack != null)
            {
                callBack(arg1);
            }
            else
            {
                throw new Exception("BroadCast Error");
            }
        }
    }

    //雙參廣播
    public static void BroadCast<T1, T2>(EventType eventType, T1 arg1, T2 arg2)
    {
        Delegate d;
        if(eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T1, T2> callBack = d as CallBack<T1, T2>;
            if(callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception("BroadCast Error");
            }
        }
    }

    //三參賡播
    public static void BroadCast<T1, T2, T3>(EventType eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        Delegate d;
        if(eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T1, T2, T3> callBack = d as CallBack<T1, T2, T3>;
            if(callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception("BroadCast Error");
            }
        }
    }

    //四參廣播
    public static void BroadCast<T1, T2, T3, T4>(EventType eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T1, T2, T3, T4> callBack = d as CallBack<T1, T2, T3, T4>;
            if(callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception("BroadCast Error");
            }
        }
    }

    //五參廣播
    public static void BroadCast<T1, T2, T3, T4, T5>(EventType eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        Delegate d;
        if(eventTable.TryGetValue(eventType, out d))
        {
            CallBack<T1, T2, T3, T4, T5> callBack = d as CallBack<T1, T2, T3, T4, T5>;
            if(callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception("BroadCastError");
            }
        }
    }

}
