using System.Collections.Generic;
using UnityEngine;

namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 事件缓存类。
    /// 发送到行为树的事件（从外部或从内部）都内缓存到事件缓存中，待下一次行为树
    /// 执行时供行为树检查。
    ///
    /// 注意：
    /// 内部发送到行为树的事件不会在当前帧执行，故事件缓存设置了冷热两个缓存区。
    /// </summary>
    public class EventCache : MonoBehaviour
    {
        private Dictionary<string, List<Event>> coldCache = new Dictionary<string, List<Event>>();
        private Dictionary<string, List<Event>> hotCache = new Dictionary<string, List<Event>>();

        /// <summary>
        /// 将冷备用缓存推出为热备用缓存，在行为树准备执行前调用。
        /// </summary>
        public void PopHotEvents()
        {
            var swap = hotCache;
            hotCache = coldCache;
            coldCache = swap;

            ClearCache(coldCache);
        }

        /// <summary>
        /// 添加一个事件到冷备用缓存。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="e">事件数据。</param>
        public void AddEvent(string eventName, Event e)
        {
            List<Event> events;

            if (!coldCache.TryGetValue(eventName, out events))
            {
                events = new List<Event>();
                coldCache.Add(eventName, events);
            }

            events.Add(e);
        }

        /// <summary>
        /// 检查热备用缓存中是否存在指定名称的事件。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <returns>如果存在事件数据，返回 true，反之返回 false。</returns>
        public bool HasEvent(string eventName)
        {
            return hotCache.ContainsKey(eventName) && hotCache[eventName].Count > 0;
        }

        /// <summary>
        /// 从热备用缓存获取指定名称的事件数据。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <returns>事件数据列表。</returns>
        public List<Event> GetEvents(string eventName)
        {
            List<Event> events;

            if (!hotCache.TryGetValue(eventName, out events))
            {
                events = new List<Event>();
                hotCache.Add(eventName, events);
            }

            return events;
        }

        /// <summary>
        /// 清空冷热备用缓存。
        /// </summary>
        public void Clear()
        {
            ClearCache(hotCache);
            ClearCache(coldCache);
        }

        /// <summary>
        /// 释放事件缓存中的事件数据包并清空事件缓存。
        /// </summary>
        /// <param name="eventDic">事件缓存。</param>
        private void ClearCache(Dictionary<string, List<Event>> eventDic)
        {
            foreach (List<Event> events in eventDic.Values)
            {
                foreach (Event e in events)
                {
                    EventPool.Delete(e);
                }

                events.Clear();
            }
        }
    }
}