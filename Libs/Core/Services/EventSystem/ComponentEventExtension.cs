using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Event
{
    /// <summary>
    /// 事件扩展方法。
    /// </summary>
    public static class ComponentEventExtension
    {
        /** Start listening to specified event type */
        public static void AddEventListener(this Component self, int type, EventCallback callback)
        {
            EventDispatcher.AddEventListener(self.gameObject, type, callback);
        }

        /** Stop listening to specified event type */
        public static void RemoveEventListener(this Component self, int type, EventCallback callback)
        {
            EventDispatcher.RemoveEventListener(self.gameObject, type, callback);
        }

        /** Check if an event listener exists or not */
        public static bool HasEventListener(this Component self, int type)
        {
            return EventDispatcher.HasEventListener(self, type);
        }

        /** Send an event to all game objects */
        public static void SendEvent(this Component self, int type, EventData e)
        {
            EventDispatcher.SendEvent(self, type, e);
        }

        /** Send an event to game object list whithout childred */
        public static void SendEvent(this Component self, int type, EventData e, List<GameObject> gameObjects)
        {
            EventDispatcher.SendEvent(self, type, e, gameObjects);
        }

        /** Send an event to game object list with children */
        public static void SendEventWithChildren(this Component self, int type,
                                                 EventData e, List<GameObject> gameObjects)
        {
            EventDispatcher.SendEvent(self, type, e, gameObjects, true);
        }

        /** Send an event to component list whithout childred */
        public static void SendEvent(this Component self, int type, EventData e, List<Component> components)
        {
            EventDispatcher.SendEvent(self, type, e, components);
        }

        /** Send an event to component list with children */
        public static void SendEventWithChildren(this Component self, int type,
                                                 EventData e, List<Component> components)
        {
            EventDispatcher.SendEvent(self, type, e, components, true);
        }

        /** Send an event to single game object whithout it's children */
        public static void SendEvent(this Component self, int type, EventData e, GameObject go)
        {
            EventDispatcher.SendEvent(self, type, e, go);
        }

        /** Send an event to single game object with it's children */
        public static void SendEventWithChildren(this Component self, int type, EventData e, GameObject go)
        {
            EventDispatcher.SendEvent(self, type, e, go, true);
        }

        /** Send an event only to child game objects */
        public static void SendEventToChildren(this Component self, int type, EventData e)
        {
            EventDispatcher.SendEventToChildren(self, type, e, self.gameObject);
        }
    }
}