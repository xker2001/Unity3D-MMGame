using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Event.UnitTest
{
    public class ListenerEventData : EventData
    {
        public List<UnitTestEventHandle> Listeners { get; set; }
    }

    public class AEventData : EventData {}

    public class UnitTestEventSystem : MMGame.UnitTest
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

        private void AddAllListeners()
        {
            A.AddEventListener(UnitTestEventType.TestEventType, A.OnEvent);
            A1.AddEventListener(UnitTestEventType.TestEventType, A1.OnEvent);
            A11a.AddEventListener(UnitTestEventType.TestEventType, A11a.OnEvent);
            A11b.AddEventListener(UnitTestEventType.TestEventType, A11b.OnEvent);
            A12.AddEventListener(UnitTestEventType.TestEventType, A12.OnEvent);
            A2.AddEventListener(UnitTestEventType.TestEventType, A2.OnEvent);
            A21.AddEventListener(UnitTestEventType.TestEventType, A21.OnEvent);
            A22.AddEventListener(UnitTestEventType.TestEventType, A22.OnEvent);
            B.AddEventListener(UnitTestEventType.TestEventType, B.OnEvent);
            B1.AddEventListener(UnitTestEventType.TestEventType, B1.OnEvent);
            B11.AddEventListener(UnitTestEventType.TestEventType, B11.OnEvent);
            B12.AddEventListener(UnitTestEventType.TestEventType, B12.OnEvent);
            B2.AddEventListener(UnitTestEventType.TestEventType, B2.OnEvent);
            B21.AddEventListener(UnitTestEventType.TestEventType, B21.OnEvent);
            B22.AddEventListener(UnitTestEventType.TestEventType, B22.OnEvent);
        }

        private void RemoveAllListeners()
        {
            A.RemoveEventListener(UnitTestEventType.TestEventType, A.OnEvent);
            A1.RemoveEventListener(UnitTestEventType.TestEventType, A1.OnEvent);
            A11a.RemoveEventListener(UnitTestEventType.TestEventType, A11a.OnEvent);
            A11b.RemoveEventListener(UnitTestEventType.TestEventType, A11b.OnEvent);
            A12.RemoveEventListener(UnitTestEventType.TestEventType, A12.OnEvent);
            A2.RemoveEventListener(UnitTestEventType.TestEventType, A2.OnEvent);
            A21.RemoveEventListener(UnitTestEventType.TestEventType, A21.OnEvent);
            A22.RemoveEventListener(UnitTestEventType.TestEventType, A22.OnEvent);
            B.RemoveEventListener(UnitTestEventType.TestEventType, B.OnEvent);
            B1.RemoveEventListener(UnitTestEventType.TestEventType, B1.OnEvent);
            B11.RemoveEventListener(UnitTestEventType.TestEventType, B11.OnEvent);
            B12.RemoveEventListener(UnitTestEventType.TestEventType, B12.OnEvent);
            B2.RemoveEventListener(UnitTestEventType.TestEventType, B2.OnEvent);
            B21.RemoveEventListener(UnitTestEventType.TestEventType, B21.OnEvent);
            B22.RemoveEventListener(UnitTestEventType.TestEventType, B22.OnEvent);
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
            ListenerEventData le = null;

            for (int i = 0; i < 3; i++)
            {
                le = EventPool.New<ListenerEventData>();
            }

            AreEqual(EventPool.TotalCreated, 3);

            EventPool.New<ListenerEventData>();
            AreEqual(EventPool.TotalCreated, 4);

            EventPool.Delete(le);
            EventPool.New<ListenerEventData>();
            AreEqual(EventPool.TotalCreated, 4);

            AEventData ae = null;

            for (int i = 0; i < 3; i++)
            {
                ae = EventPool.New<AEventData>();
            }

            AreEqual(EventPool.TotalCreated, 7);

            EventPool.New<AEventData>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEventData>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEventData>();
            AreEqual(EventPool.TotalCreated, 8);

            EventPool.Delete(ae);
            EventPool.New<AEventData>();
            AreEqual(EventPool.TotalCreated, 8);
        }

        [TestMethod]
        public void AddRemoveListener()
        {
            IsFalse(A11a.HasEventListener(UnitTestEventType.TestEventType));

            A11a.AddEventListener(UnitTestEventType.TestEventType, A11a.OnEvent);
            IsTrue(A11a.HasEventListener(UnitTestEventType.TestEventType));

            A11a.RemoveEventListener(UnitTestEventType.TestEventType, A11a.OnEvent);
            IsFalse(A11a.HasEventListener(UnitTestEventType.TestEventType));

            Reset();
        }

        [TestMethod]
        public void SendToAll()
        {
            AddAllListeners();
            this.SendEvent(UnitTestEventType.TestEventType, EventPool.New<AEventData>());

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

            var targets = new List<GameObject> {A1.gameObject, A12.gameObject, B.gameObject, B22.gameObject};
            this.SendEvent(UnitTestEventType.TestEventType, EventPool.New<AEventData>(), targets);

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

            var targets = new List<GameObject> {A1.gameObject, A12.gameObject, B.gameObject, B22.gameObject};
            this.SendEventWithChildren(UnitTestEventType.TestEventType, EventPool.New<AEventData>(), targets);

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
            this.SendEvent(UnitTestEventType.TestEventType, EventPool.New<AEventData>(), A.gameObject);

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
            this.SendEventWithChildren(UnitTestEventType.TestEventType, EventPool.New<AEventData>(), A.gameObject);

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

            A.SendEventToChildren(UnitTestEventType.TestEventType, EventPool.New<AEventData>());

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
            A11a.SendEvent(UnitTestEventType.TestEventType, EventPool.New<AEventData>());

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

            var targets = new List<GameObject> {A.gameObject, A1.gameObject, A11a.gameObject, A12.gameObject};
            this.SendEventWithChildren(UnitTestEventType.TestEventType, EventPool.New<AEventData>(), targets);

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
            A11a.AddEventListener(UnitTestEventType.TestEventType, A11a.OnEventAddListeners);

            var listeners = new List<UnitTestEventHandle> {A, B, B11, B22};

            var targets = new List<GameObject> {A.gameObject, B.gameObject};
            var e = EventPool.New<ListenerEventData>();
            e.Listeners = listeners;
            this.SendEventWithChildren(UnitTestEventType.TestEventType, e, targets);

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

            A11a.RemoveEventListener(UnitTestEventType.TestEventType, A11a.OnEventAddListeners);
            A.RemoveEventListener(UnitTestEventType.TestEventType, A.OnEvent);
            B.RemoveEventListener(UnitTestEventType.TestEventType, B.OnEvent);
            B11.RemoveEventListener(UnitTestEventType.TestEventType, B11.OnEvent);
            B22.RemoveEventListener(UnitTestEventType.TestEventType, B22.OnEvent);

            Reset();
        }

        [TestMethod]
        public void RemoveListenerDuringSending()
        {
            A11a.AddEventListener(UnitTestEventType.TestEventType, A11a.OnEventRemoveListeners);
            A.AddEventListener(UnitTestEventType.TestEventType, A.OnEvent);
            B.AddEventListener(UnitTestEventType.TestEventType, B.OnEvent);
            B11.AddEventListener(UnitTestEventType.TestEventType, B11.OnEvent);
            B22.AddEventListener(UnitTestEventType.TestEventType, B22.OnEvent);

            var listeners = new List<UnitTestEventHandle> {A, B, B11, B22};

            var targets = new List<GameObject> {A.gameObject, B.gameObject};

            var e = EventPool.New<ListenerEventData>();
            e.Listeners = listeners;
            this.SendEventWithChildren(UnitTestEventType.TestEventType, e, targets);
            A11a.RemoveEventListener(UnitTestEventType.TestEventType, A11a.OnEventRemoveListeners);

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