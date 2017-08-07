namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestEventListener : EventListener
    {
        private string eventName = "UnitTest";

        public bool OnEventCalled { get; private set; }
        public bool OnPauseCalled { get; private set; }
        public bool OnResumeCalled { get; private set; }
        public int EnableTimes { get; private set; }
        public int DisableTimes { get; private set; }

        public string EventName
        {
            get { return eventName; }
            private set { eventName = value; }
        }

        public string Info { get; private set; }

        public TestEventListener SetInfo(string info)
        {
            Info = info;
            return this;
        }

        public TestEventListener SetEventName(string name)
        {
            EventName = name;
            return this;
        }

        void OnEnable()
        {
            this.AddEventListener(EventName, OnEvent);
            EnableTimes += 1;
        }

        void OnDisable()
        {
            this.RemoveEventListener(EventName, OnEvent);
            DisableTimes += 1;
        }

        protected override void OnPause()
        {
            OnPauseCalled = true;
        }

        protected override void OnResume()
        {
            OnResumeCalled = true;
        }

        private void OnEvent(Event e)
        {
            OnResumeCalled = true;
            IsTriggered = true;
        }

        public override void ResetForSpawn()
        {
        }

        public override void ReleaseForDespawn()
        {
        }
    }
}