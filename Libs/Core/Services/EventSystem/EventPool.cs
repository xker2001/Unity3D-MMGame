using System.Collections.Generic;
using System;
using UnityEngine;

namespace MMGame
{
    /**
     * Event pool
     */
    public static class EventPool
    {
        private static Dictionary<Type, Stack<Event>> freeEventDic = new Dictionary<Type, Stack<Event>>();
        private static int totalCreated = 0;

        /// <summary>
        /// 从事件对象池获取一个指定类型事件的实例。
        /// </summary>
        /// <typeparam name="T">事件的类型。</typeparam>
        /// <returns>事件的实例。</returns>
        public static T New<T>() where T : Event
        {
            Stack<Event> freeEvents;
            Event e;

            Type type = typeof(T);

            if (!freeEventDic.TryGetValue(type, out freeEvents))
            {
                freeEvents = new Stack<Event>();
                freeEventDic.Add(type, freeEvents);
            }

            if (freeEvents.Count > 0)
            {
                e = freeEvents.Pop();
            }
            else
            {
                e = (Event) Activator.CreateInstance(type);
                totalCreated += 1;

                if (totalCreated > 20)
                {
                    Debug.LogError("EventPool: too many Event instances.");
                }
            }

            return (T) e;
        }


        /// <summary>
        /// 回收一个事件实例到事件对象池。
        /// </summary>
        /// <param name="e">事件实例。</param>
        internal static void Delete(Event e)
        {
            Type type = e.GetType();
            freeEventDic[type].Push(e);
            e.Reset();
        }

        // ------------------------------------------------------
        // 单元测试用方法

        /// <summary>
        /// 获取创建的事件实例的总数量。
        /// </summary>
        public static int TotalCreated
        {
            get { return totalCreated; }
        }


        /// <summary>
        /// 清空对象池。
        /// </summary>
        public static void Clear()
        {
            freeEventDic.Clear();
            totalCreated = 0;
        }
    }
}