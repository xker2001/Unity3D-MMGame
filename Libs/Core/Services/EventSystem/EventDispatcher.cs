using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Event
{
    public delegate void EventCallback(EventData e);

    /// <summary>
    /// 事件分发中心。
    /// </summary>
    public static class EventDispatcher
    {
        private struct SenderParameters
        {
            public readonly int Type;
            public readonly GameObject Listener;
            public readonly EventCallback Callback;
            public readonly bool IsAdd; // this listener is to be added or removed

            public SenderParameters(int type, GameObject listener, EventCallback callback, bool isAdd)
            {
                Type = type;
                Listener = listener;
                Callback = callback;
                IsAdd = isAdd;
            }

            public static bool operator ==(SenderParameters a, SenderParameters b)
            {
                return a.Type == b.Type && a.Listener == b.Listener && a.Callback == b.Callback;
            }

            public static bool operator !=(SenderParameters a, SenderParameters b)
            {
                return !(a == b);
            }

            public bool Equals(SenderParameters b)
            {
                return this == b;
            }

            public override bool Equals(System.Object obj)
            {
                if (!(obj is SenderParameters))
                {
                    return false;
                }

                return this == (SenderParameters) obj;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 29 + Type.GetHashCode();
                    hash = hash * 29 + Listener.GetHashCode();
                    hash = hash * 29 + Callback.GetHashCode();
                    return hash;
                }
            }
        }

        /// <summary>
        /// 用于存放所有事件注册信息的字典。
        /// Dictionary<type, Dictionary<sender, List<callback>>>
        /// </summary>
        private static readonly Dictionary<int, Dictionary<GameObject, List<EventCallback>>> eventTable =
            new Dictionary<int, Dictionary<GameObject, List<EventCallback>>>();

        /// <summary>
        /// 待添加或删除的侦听者队列。
        /// </summary>
        private static readonly List<SenderParameters> addRemoveQueue = new List<SenderParameters>();

        /// <summary>
        /// 发送者的数量。
        /// </summary>
        private static int senderCount;

        /// <summary>
        /// 当发送者数量被设置为 0 时可以安全执行添加移除侦听者的操作。
        /// </summary>
        private static int SenderCount
        {
            get { return senderCount; }
            set
            {
                senderCount = value;

                if (senderCount == 0)
                {
                    ApplyAddRemoveListeners();
                }
            }
        }

        /// <summary>
        /// 获取侦听者的总数。
        /// </summary>
        /// <returns>侦听者总数。</returns>
        public static int GetListernerNumber()
        {
            var allListeners = new List<GameObject>();

            foreach (Dictionary<GameObject, List<EventCallback>> listeners in eventTable.Values)
            {
                foreach (GameObject listener in listeners.Keys)
                {
                    if (!allListeners.Contains(listener))
                    {
                        allListeners.Add(listener);
                    }
                }
            }

            return allListeners.Count;
        }

        /// <summary>
        /// 注册一个新的事件侦听。
        /// 在事件流中添加或移除事件侦听不会立即生效，会在本次事件流结束之后进行。
        /// </summary>
        /// <param name="listener">侦听者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="callback">事件回调。</param>
        public static void AddEventListener(GameObject listener, int type, EventCallback callback)
        {
            if (SenderCount == 0)
            {
                DoAddEventListener(listener, type, callback);
            }
            else
            {
                addRemoveQueue.Add(new SenderParameters(type, listener, callback, true));
            }
        }

        /// <summary>
        /// 注销一个事件侦听。
        /// </summary>
        /// <param name="listener">侦听者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="callback">事件回调。</param>
        public static void RemoveEventListener(GameObject listener, int type, EventCallback callback)
        {
            if (SenderCount == 0)
            {
                DoRemoveEventListener(listener, type, callback);
            }
            else
            {
                addRemoveQueue.Add(new SenderParameters(type, listener, callback, false));
            }
        }

        /// <summary>
        /// 检查指定组件是否侦听了指定的事件类型。
        /// </summary>
        /// <param name="component">组件实例。</param>
        /// <param name="type">事件类型。</param>
        /// <returns>返回 true 或 false。</returns>
        public static bool HasEventListener(Component component, int type)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;
            List<EventCallback> callbacks;

            if (!eventTable.TryGetValue(type, out listeners) ||
                !listeners.TryGetValue(component.gameObject, out callbacks))
            {
                return false;
            }

            foreach (EventCallback callback in callbacks)
            {
                if (callback.Target.Equals(component))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 发送一个事件给所有侦听者。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="e">事件数据。</param>
        public static void SendEvent(object sender,
                                     int type,
                                     EventData e)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                EventPool.Delete(e);
                return;
            }

            SenderCount += 1;

            e.Sender = sender;

            foreach (GameObject receiver in listeners.Keys)
            {
                if (e.IsStopped)
                {
                    break;
                }

                SendEventToGameObject(sender, e, receiver, listeners, false);
            }

            EventPool.Delete(e);
            SenderCount -= 1;
        }

        /// <summary>
        /// 发送一个事件给 GameObject 接收者列表。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="e">事件数据。</param>
        /// <param name="receivers">接收者列表。</param>
        /// <param name="includeChildren">是否发送给接收者的子物体。</param>
        public static void SendEvent(object sender,
                                     int type,
                                     EventData e,
                                     List<GameObject> receivers,
                                     bool includeChildren = false)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                EventPool.Delete(e);
                return;
            }

            SenderCount += 1;
            e.Sender = sender;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (e.IsStopped)
                {
                    break;
                }

                SendEventToGameObject(sender, e, receivers[i], listeners, includeChildren);
            }

            EventPool.Delete(e);
            SenderCount -= 1;
        }

        /// <summary>
        /// 发送一个事件给 Component 接收者列表。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="e">事件数据。</param>
        /// <param name="receivers">接收者列表。</param>
        /// <param name="includeChildren">是否发送给接收者的子物体。</param>
        public static void SendEvent(object sender,
                                     int type,
                                     EventData e,
                                     List<Component> receivers,
                                     bool includeChildren = false)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                EventPool.Delete(e);
                return;
            }

            SenderCount += 1;
            e.Sender = sender;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (e.IsStopped)
                {
                    break;
                }

                SendEventToGameObject(sender, e, receivers[i].gameObject, listeners, includeChildren);
            }

            EventPool.Delete(e);
            SenderCount -= 1;
        }

        /// <summary>
        /// 发送一个事件给指定的 GameObject 接收者。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="e">事件数据。</param>
        /// <param name="receiver">接收者。</param>
        /// <param name="includeChildren">是否发送给接收者的子物体。</param>
        public static void SendEvent(object sender,
                                     int type,
                                     EventData e,
                                     GameObject receiver,
                                     bool includeChildren = false)
        {
            if (receiver == null)
            {
                throw new ApplicationException("SendEvent: receiver == null!");
            }

            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                EventPool.Delete(e);
                return;
            }

            SenderCount += 1;
            e.Sender = sender;

            SendEventToGameObject(sender, e, receiver, listeners, includeChildren);
            EventPool.Delete(e);
            SenderCount -= 1;
        }

        /// <summary>
        /// 发送事件给接收者的所有子物体（不包含接收者）。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="e">事件数据。</param>
        /// <param name="receiver">事件接收者。</param>
        public static void SendEventToChildren(object sender,
                                               int type,
                                               EventData e,
                                               GameObject receiver)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                EventPool.Delete(e);
                return;
            }

            SenderCount += 1;
            e.Sender = sender;

            foreach (Transform child in receiver.transform)
            {
                if (e.IsStopped)
                {
                    break;
                }

                SendEventToGameObject(sender, e, child.gameObject, listeners, true);
            }

            EventPool.Delete(e);
            SenderCount -= 1;
        }

        // ------------------------------------------------------
        // private methods

        /// <summary>
        /// 发送事件给指定接收者（及其子物体）。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="e">事件数据。</param>
        /// <param name="receiver">事件接收者。</param>
        /// <param name="listeners">事件所有侦听者列表。</param>
        /// <param name="includeChildren">是否包含接收者子物体。</param>
        private static void SendEventToGameObject(object sender,
                                                  EventData e,
                                                  GameObject receiver,
                                                  Dictionary<GameObject, List<EventCallback>> listeners,
                                                  bool includeChildren)
        {
            if (e.IsStopped || receiver == null)
            {
                return;
            }

            List<EventCallback> callbacks;

            if (listeners.TryGetValue(receiver, out callbacks) && !e.CheckTriggered(receiver))
            {
                foreach (EventCallback callback in callbacks)
                {
                    if (e.IsStopped)
                    {
                        return;
                    }

                    if (callback.Target != sender)
                    {
                        callback(e);
                    }
                }

                e.Trigger(receiver); // don't send to receiver again when sending to receiver's parent
            }

            if (includeChildren)
            {
                foreach (Transform child in receiver.transform)
                {
                    SendEventToGameObject(sender, e, child.gameObject, listeners, true);
                }
            }
        }

        /// <summary>
        /// 处理待添加和待移除的事件侦听队列。
        /// </summary>
        private static void ApplyAddRemoveListeners()
        {
            for (int i = 0; i < addRemoveQueue.Count; i++)
            {
                if (addRemoveQueue[i].IsAdd)
                {
                    DoAddEventListener(addRemoveQueue[i].Listener, addRemoveQueue[i].Type, addRemoveQueue[i].Callback);
                }
                else
                {
                    DoRemoveEventListener(addRemoveQueue[i].Listener, addRemoveQueue[i].Type,
                                          addRemoveQueue[i].Callback);
                }
            }

            addRemoveQueue.Clear();
        }

        /// <summary>
        /// 执行添加一个事件侦听。
        /// </summary>
        /// <param name="listener">事件侦听者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="callback">事件回调。</param>
        private static void DoAddEventListener(GameObject listener, int type, EventCallback callback)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;

            if (!eventTable.TryGetValue(type, out listeners))
            {
                listeners = new Dictionary<GameObject, List<EventCallback>>();
                eventTable.Add(type, listeners);
            }

            List<EventCallback> callbacks;

            if (!listeners.TryGetValue(listener, out callbacks))
            {
                callbacks = new List<EventCallback>();
                listeners.Add(listener, callbacks);
            }

            if (callbacks.Contains(callback))
            {
                Debug.LogError("Callback function has been registerd to this event type already!");
            }
            else
            {
                callbacks.Add(callback);
            }
        }

        /// <summary>
        /// 执行注销一个事件侦听。
        /// 有时候我们不可避免会试图移除不存在的侦听者。比如我们在 OnEnable 中添加侦听后立即又在 OnDisable 中
        /// 移除了该侦听（如 PoolManager 执行了连续的 enable 和 disable 操作），因此这种情况不抛出错误。
        ///  </summary>
        /// <param name="listener">事件侦听者。</param>
        /// <param name="type">事件类型。</param>
        /// <param name="callback">事件回调。</param>
        private static void DoRemoveEventListener(GameObject listener, int type, EventCallback callback)
        {
            Dictionary<GameObject, List<EventCallback>> listeners;
            List<EventCallback> callbacks;

            if (eventTable.TryGetValue(type, out listeners) &&
                listeners.TryGetValue(listener, out callbacks) &&
                callbacks.Contains(callback))
            {
                callbacks.Remove(callback);

                if (callbacks.Count == 0)
                {
                    listeners.Remove(listener);
                }
            }

            // UnityEngine.Debug.LogError ("Callback function does not exsit!");
        }
    }
}