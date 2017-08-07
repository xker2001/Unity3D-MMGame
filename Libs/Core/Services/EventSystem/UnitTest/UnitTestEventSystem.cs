using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    public class ListenerEvent : Event
    {
        public List<UnitTestEventHandle> Listeners { get; set; }
    }

    public class AEvent : Event
    {
    }

    public class UnitTestEventSystem : UnitTest
    {
        public UnitTestEventHandle A;
        public UnitTestEventHandle A1;
        public UnitTestEventHandle A11a;
        public UnitTestEventHandle A11b;
        public UnitTestEventHandle A12;
        public UnitTestEventHandle A2;
        public UnitTestEventHandle A21;
        public UnitTestEventHandle A22;
        public UnitTestEventHandle B;
        public UnitTestEventHandle B1;
        public UnitTestEventHandle B11;
        public UnitTestEventHandle B12;
        public UnitTestEventHandle B2;
        public UnitTestEventHandle B21;
        public UnitTestEventHandle B22;

        private string eventName = "EvtUnitTestEventSystem";


        private void AddAllListeners()
        {
            A.AddEventListener(eventName, A.OnEvent);
            A1.AddEventListener(eventName, A1.OnEvent);
            A11a.AddEventListener(eventName, A11a.OnEvent);
            A11b.AddEventListener(eventName, A11b.OnEvent);
            A12.AddEventListener(eventName, A12.OnEvent);
            A2.AddEventListener(eventName, A2.OnEvent);
            A21.AddEventListener(eventName, A21.OnEvent);
            A22.AddEventListener(eventName, A22.OnEvent);
            B.AddEventListener(eventName, B.OnEvent);
            B1.AddEventListener(eventName, B1.OnEvent);
            B11.AddEventListener(eventName, B11.OnEvent);
            B12.AddEventListener(eventName, B12.OnEvent);
            B2.AddEventListener(eventName, B2.OnEvent);
            B21.AddEventListener(eventName, B21.OnEvent);
            B22.AddEventListener(eventName, B22.OnEvent);
        }


        private void RemoveAllListeners()
        {
            A.RemoveEventListener(eventName, A.OnEvent);
            A1.RemoveEventListener(eventName, A1.OnEvent);
            A11a.RemoveEventListener(eventName, A11a.OnEvent);
            A11b.RemoveEventListener(eventName, A11b.OnEvent);
            A12.RemoveEventListener(eventName, A12.OnEvent);
            A2.RemoveEventListener(eventName, A2.OnEvent);
            A21.RemoveEventListener(eventName, A21.OnEvent);
            A22.RemoveEventListener(eventName, A22.OnEvent);
            B.RemoveEventListener(eventName, B.OnEvent);
            B1.RemoveEventListener(eventName, B1.OnEvent);
            B11.RemoveEventListener(eventName, B11.OnEvent);
            B12.RemoveEventListener(eventName, B12.OnEvent);
            B2.RemoveEventListener(eventName, B2.OnEvent);
            B21.RemoveEventListener(eventName, B21.OnEvent);
            B22.RemoveEventListener(eventName, B22.OnEvent);
        }


        private void Reset()
        {
            A.Reset();
            A1.Reset();
            A11a.Reset();
            A11b.Reset();
            A12.Reset();
            A2.Reset();
            A21.Reset();
            A22.Reset();
            B.Reset();
            B1.Reset();
            B11.Reset();
            B12.Reset();
            B2.Reset();
            B21.Reset();
            B22.Reset();
        }

        #region Test methods

        [TestMethod]
        public void TestEventPool()
        {
            ListenerEvent le = null;

            for (int i = 0; i < 3; i++)
            {
                le = EventPool.New<ListenerEvent>();
            }

            AreEqual(EventPool.TotalCreated, 3);

            EventPool.New<ListenerEvent>();
            AreEqual(EventPool.TotalCreated, 4);

            EventPool.Delete(le);
            EventPool.New<ListenerEvent>();
            AreEqual(EventPool.TotalCreated, 4);

            AEvent ae = null;

            for (int i = 0; i < 3; i++)
            {
                ae = EventPool.New<AEvent>();
            }

            AreEqual(EventPool.TotalCreated, 7);

            EventPool.New<AEvent>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEvent>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEvent>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEvent>();
            AreEqual(EventPool.TotalCreated, 8);
        }

        [TestMethod]
        public void AddRemoveListener()
        {
            IsFalse(A11a.HasEventListener(eventName));

            A11a.AddEventListener(eventName, A11a.OnEvent);
            IsTrue(A11a.HasEventListener(eventName));

            A11a.RemoveEventListener(eventName, A11a.OnEvent);
            IsFalse(A11a.HasEventListener(eventName));

            Reset();
        }


        [TestMethod]
        public void SendToAll()
        {
            AddAllListeners();
            this.SendEvent(eventName, EventPool.New<AEvent>());

            AreEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreEqual(A2.Triggered, 1);
            AreEqual(A21.Triggered, 1);
            AreEqual(A22.Triggered, 1);
            AreEqual(B.Triggered, 1);
            AreEqual(B1.Triggered, 1);
            AreEqual(B11.Triggered, 1);
            AreEqual(B12.Triggered, 1);
            AreEqual(B2.Triggered, 1);
            AreEqual(B21.Triggered, 1);
            AreEqual(B22.Triggered, 1);

            RemoveAllListeners();
            AreEqual(EventDispatcher.GetListernerNumber(), 0);
            Reset();
        }


        [TestMethod]
        public void SendToListWhithoutChildren()
        {
            AddAllListeners();

            List<GameObject> targets = new List<GameObject>();
            targets.Add(A1.gameObject);
            targets.Add(A12.gameObject);
            targets.Add(B.gameObject);
            targets.Add(B22.gameObject);
            this.SendEvent(eventName, EventPool.New<AEvent>(), targets);

            AreNotEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreNotEqual(A11a.Triggered, 1);
            AreNotEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreNotEqual(A2.Triggered, 1);
            AreNotEqual(A21.Triggered, 1);
            AreNotEqual(A22.Triggered, 1);
            AreEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void SendToListWhithChildren()
        {
            AddAllListeners();

            List<GameObject> targets = new List<GameObject>();
            targets.Add(A1.gameObject);
            targets.Add(A12.gameObject);
            targets.Add(B.gameObject);
            targets.Add(B22.gameObject);
            this.SendEventWithChildren(eventName, EventPool.New<AEvent>(), targets);

            AreNotEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreNotEqual(A2.Triggered, 1);
            AreNotEqual(A21.Triggered, 1);
            AreNotEqual(A22.Triggered, 1);
            AreEqual(B.Triggered, 1);
            AreEqual(B1.Triggered, 1);
            AreEqual(B11.Triggered, 1);
            AreEqual(B12.Triggered, 1);
            AreEqual(B2.Triggered, 1);
            AreEqual(B21.Triggered, 1);
            AreEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void SendToSingleWhithoutChildren()
        {
            AddAllListeners();

            this.SendEvent(eventName, EventPool.New<AEvent>(), A.gameObject);

            AreEqual(A.Triggered, 1);
            AreNotEqual(A1.Triggered, 1);
            AreNotEqual(A11a.Triggered, 1);
            AreNotEqual(A11b.Triggered, 1);
            AreNotEqual(A12.Triggered, 1);
            AreNotEqual(A2.Triggered, 1);
            AreNotEqual(A21.Triggered, 1);
            AreNotEqual(A22.Triggered, 1);
            AreNotEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreNotEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void SendToSingleWhithChildren()
        {
            AddAllListeners();

            this.SendEventWithChildren(eventName, EventPool.New<AEvent>(), A.gameObject);

            AreEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreEqual(A2.Triggered, 1);
            AreEqual(A21.Triggered, 1);
            AreEqual(A22.Triggered, 1);
            AreNotEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreNotEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void SendToChildren()
        {
            AddAllListeners();

            A.SendEventToChildren(eventName, EventPool.New<AEvent>());

            AreNotEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreEqual(A2.Triggered, 1);
            AreEqual(A21.Triggered, 1);
            AreEqual(A22.Triggered, 1);
            AreNotEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreNotEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void DontSendToSelf()
        {
            AddAllListeners();
            A11a.SendEvent(eventName, EventPool.New<AEvent>());

            AreEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreNotEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreEqual(A2.Triggered, 1);
            AreEqual(A21.Triggered, 1);
            AreEqual(A22.Triggered, 1);
            AreEqual(B.Triggered, 1);
            AreEqual(B1.Triggered, 1);
            AreEqual(B11.Triggered, 1);
            AreEqual(B12.Triggered, 1);
            AreEqual(B2.Triggered, 1);
            AreEqual(B21.Triggered, 1);
            AreEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void DontSendToOneMultipleTimes()
        {
            AddAllListeners();

            List<GameObject> targets = new List<GameObject>();
            targets.Add(A.gameObject);
            targets.Add(A1.gameObject);
            targets.Add(A11a.gameObject);
            targets.Add(A12.gameObject);
            this.SendEventWithChildren(eventName, EventPool.New<AEvent>(), targets);

            AreEqual(A.Triggered, 1);
            AreEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreEqual(A11b.Triggered, 1);
            AreEqual(A12.Triggered, 1);
            AreEqual(A2.Triggered, 1);
            AreEqual(A21.Triggered, 1);
            AreEqual(A22.Triggered, 1);
            AreNotEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreNotEqual(B22.Triggered, 1);

            RemoveAllListeners();
            Reset();
        }


        [TestMethod]
        public void AddListenerDuringSending()
        {
            A11a.AddEventListener(eventName, A11a.OnEventAddListeners);

            List<UnitTestEventHandle> listeners = new List<UnitTestEventHandle>();
            listeners.Add(A);
            listeners.Add(B);
            listeners.Add(B11);
            listeners.Add(B22);

            List<GameObject> targets = new List<GameObject>();
            targets.Add(A.gameObject);
            targets.Add(B.gameObject);
            ListenerEvent e = EventPool.New<ListenerEvent>();
            e.Listeners = listeners;
            this.SendEventWithChildren(eventName, e, targets);

            // will be traversed by order
            AreNotEqual(A.Triggered, 1);
            AreNotEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreNotEqual(A11b.Triggered, 1);
            AreNotEqual(A12.Triggered, 1);
            AreNotEqual(A2.Triggered, 1);
            AreNotEqual(A21.Triggered, 1);
            AreNotEqual(A22.Triggered, 1);
            AreNotEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreNotEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreNotEqual(B22.Triggered, 1);

            A11a.RemoveEventListener(eventName, A11a.OnEventAddListeners);
            A.RemoveEventListener(eventName, A.OnEvent);
            B.RemoveEventListener(eventName, B.OnEvent);
            B11.RemoveEventListener(eventName, B11.OnEvent);
            B22.RemoveEventListener(eventName, B22.OnEvent);

            Reset();
        }


        [TestMethod]
        public void RemoveListenerDuringSending()
        {
            A11a.AddEventListener(eventName, A11a.OnEventRemoveListeners);
            A.AddEventListener(eventName, A.OnEvent);
            B.AddEventListener(eventName, B.OnEvent);
            B11.AddEventListener(eventName, B11.OnEvent);
            B22.AddEventListener(eventName, B22.OnEvent);

            List<UnitTestEventHandle> listeners = new List<UnitTestEventHandle>();
            listeners.Add(A);
            listeners.Add(B);
            listeners.Add(B11);
            listeners.Add(B22);

            List<GameObject> targets = new List<GameObject>();
            targets.Add(A.gameObject);
            targets.Add(B.gameObject);

            ListenerEvent e = EventPool.New<ListenerEvent>();
            e.Listeners = listeners;
            this.SendEventWithChildren(eventName, e, targets);
            A11a.RemoveEventListener(eventName, A11a.OnEventRemoveListeners);

            AreEqual(A.Triggered, 1);
            AreNotEqual(A1.Triggered, 1);
            AreEqual(A11a.Triggered, 1);
            AreNotEqual(A11b.Triggered, 1);
            AreNotEqual(A12.Triggered, 1);
            AreNotEqual(A2.Triggered, 1);
            AreNotEqual(A21.Triggered, 1);
            AreNotEqual(A22.Triggered, 1);
            AreEqual(B.Triggered, 1);
            AreNotEqual(B1.Triggered, 1);
            AreEqual(B11.Triggered, 1);
            AreNotEqual(B12.Triggered, 1);
            AreNotEqual(B2.Triggered, 1);
            AreNotEqual(B21.Triggered, 1);
            AreEqual(B22.Triggered, 1);

            Reset();
        }

        #endregion Test methods
    }
}