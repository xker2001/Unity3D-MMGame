namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestState : FsmState
    {
        public bool OnInitCalled { get; set; }
        public bool OnEnterCalled { get; set; }
        public int TimesOfEnter { get; set; }
        public int Ticks { get; private set; }

        public void SetExitCode(int code)
        {
            Exit(code);
        }

        public string Info { get; private set; }

        public TestState SetInfo(string info)
        {
            Info = info;
            return this;
        }

        protected override void OnInit()
        {
            OnInitCalled = true;
        }

        protected override void OnEnter()
        {
            OnEnterCalled = true;
            TimesOfEnter += 1;

            foreach (TestCondition con in GetComponents<TestCondition>())
            {
                con.Set(false);
            }

            foreach (TestInverseCondition con in GetComponents<TestInverseCondition>())
            {
                con.Set(true);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            Ticks += 1;
        }

        protected override void OnPause()
        {
        }

        protected override void OnResume()
        {
        }

        protected override void OnStop()
        {
            OnEnterCalled = false;
        }
    }
}