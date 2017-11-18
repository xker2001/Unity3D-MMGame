using MMGame.Event;

namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestEventListener : EventListener
    {
        public bool FlagEventCalled { get; private set; }
        public bool FlagPauseCalled { get; private set; }
        public bool FlagResumeCalled { get; private set; }
        public int EnableTimes { get; private set; }
        public int DisableTimes { get; private set; }

        public int EventType { get; private set; }

        public string Info { get; private set; }

        public TestEventListener SetInfo(string info)
        {
            Info = info;
            return this;
        }

        public TestEventListener SetEventType(int id)
        {
            EventType = id;
            return this;
        }

        private void OnEnable()
        {
            this.AddEventListener(EventType, OnEvent);
            EnableTimes += 1;
        }

        private void OnDisable()
        {
            this.RemoveEventListener(EventType, OnEvent);
            DisableTimes += 1;
        }

        protected override void OnPause()
        {
            FlagPauseCalled = true;
        }

        protected override void OnResume()
        {
            FlagResumeCalled = true;
        }

        private void OnEvent(EventData e)
        {
            FlagResumeCalled = true;
            IsTriggered = true;
        }

        public override void OnSpawn() {}

        public override void OnDespawn() {}
    }
}