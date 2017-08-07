using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MMGame
{
    public class UnitTestEventHandle : MonoBehaviour
    {
        public int Triggered { get; private set; }
        private string eventName = "EvtUnitTestEventSystem";

        public void OnEvent(Event e)
        {
            Triggered += 1;
        }

        public void OnEventAddListeners(Event e)
        {
            List<UnitTestEventHandle> listeners = (e as ListenerEvent).Listeners;

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].AddEventListener(eventName, listeners[i].OnEvent);
            }

            Triggered += 1;
        }

        public void OnEventRemoveListeners(Event e)
        {
            List<UnitTestEventHandle> listeners = (e as ListenerEvent).Listeners;

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].RemoveEventListener(eventName, listeners[i].OnEvent);
            }

            this.RemoveEventListener(eventName, OnEventRemoveListeners);

            Triggered += 1;
        }

        public void Reset()
        {
            Triggered = 0;
        }
    }
}