using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Event.UnitTest
{
    public class UnitTestEventHandle : MonoBehaviour
    {
        public int Triggered { get; private set; }

        public void OnEvent(EventData e)
        {
            Triggered += 1;
        }

        public void OnEventAddListeners(EventData e)
        {
            List<UnitTestEventHandle> listeners = (e as ListenerEventData).Listeners;

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].AddEventListener(UnitTestEventType.TestEventType, listeners[i].OnEvent);
            }

            Triggered += 1;
        }

        public void OnEventRemoveListeners(EventData e)
        {
            List<UnitTestEventHandle> listeners = (e as ListenerEventData).Listeners;

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].RemoveEventListener(UnitTestEventType.TestEventType, listeners[i].OnEvent);
            }

            this.RemoveEventListener(UnitTestEventType.TestEventType, OnEventRemoveListeners);

            Triggered += 1;
        }

        public void Reset()
        {
            Triggered = 0;
        }
    }
}