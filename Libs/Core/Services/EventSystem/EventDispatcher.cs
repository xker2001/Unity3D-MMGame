using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MMGame
{
    public delegate void EventCallback(Event e);

    /**
     * Event Dispatcher
     */

    public static class EventDispatcher
    {
        private struct SenderParameters
        {
            public string Type;
            public GameObject Listener;
            public EventCallback Callback;
            public bool IsAdd; // this listener is to be added or removed

            public SenderParameters(string type, GameObject listener, EventCallback callback, bool isAdd)
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
                if (obj == null || !(obj is SenderParameters))
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

        /**
         * Dic to store all event registers
         * Dictionary<type, Dictionary<sender, List<callback>>>
         */

        private static Dictionary<string, Dictionary<GameObject, List<EventCallback>>> eventTable =
            new Dictionary<string, Dictionary<GameObject, List<EventCallback>>>();

        /**
         * Queue of listeners to be added and removed
         */
        private static List<SenderParameters> addRemoveQueue = new List<SenderParameters>();

        /**
         * sender counter
         */
        private static int senderCount;

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

        // ------------------------------------------------------
        // public methods

        /**
         * Get the total number of listeners
         */

        public static int GetListernerNumber()
        {
            List<GameObject> allListeners = new List<GameObject>();

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

        /**
         * Register a new event listener
         * Add or remove an event listener during another event sending will not takes effect immediately.
         */

        public static void AddEventListener(GameObject listener, string type, EventCallback callback)
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

        /**
         * Remove an event listener
         */

        public static void RemoveEventListener(GameObject listener, string type, EventCallback callback)
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

        /**
         * Check if an event listener exists or not
         */

        public static bool HasEventListener(Component component, string type)
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

        /**
         * Send event to all registered listeners
         */

        public static void SendEvent(object sender,
                                     string type,
                                     Event e)
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

        /**
         * Send event to specified receivers (GameObject)
         */

        public static void SendEvent(object sender,
                                     string type,
                                     Event e,
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

        /**
        * Send event to specified receivers (Component)
        */

        public static void SendEvent(object sender,
                                     string type,
                                     Event e,
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

        /**
         * Send event to specified receiver
         */

        public static void SendEvent(object sender,
                                     string type,
                                     Event e,
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

        /**
         * Send event to all receiver's children (exclude receiver)
         */

        public static void SendEventToChildren(object sender,
                                               string type,
                                               Event e,
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

        /**
         * Send event to a receiver and it's children
         */

        private static void SendEventToGameObject(object sender,
                                                  Event e,
                                                  GameObject receiver,
                                                  Dictionary<GameObject, List<EventCallback>> listeners,
                                                  bool includeChildren = false)
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

        /**
         * Apply add & remove queue
         */

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

        /**
        * Register a new event listener
        */

        private static void DoAddEventListener(GameObject listener, string type, EventCallback callback)
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
                UnityEngine.Debug.LogError("Callback function has been registerd to this event type already!");
            }
            else
            {
                callbacks.Add(callback);
            }
        }

        /**
         * Remove an event listener
         * Some times we have to remove nonexistent listener. For example we add event listener in
         * OnEnable and remove it in OnDisable when pool manager does a continous enable
         * and disable.
         */

        private static void DoRemoveEventListener(GameObject listener, string type, EventCallback callback)
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