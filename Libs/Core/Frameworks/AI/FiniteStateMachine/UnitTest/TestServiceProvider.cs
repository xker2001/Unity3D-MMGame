namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestServiceProvider : ServiceProvider
    {
        public bool Value { get; set; }
        public int EnableTimes { get; private set; }
        public int DisableTimes { get; private set; }

        public string Info { get; private set; }

        public TestServiceProvider SetInfo(string info)
        {
            Info = info;
            return this;
        }

        void OnEnable()
        {
            Value = true;
            EnableTimes += 1;
        }

        void OnDisable()
        {
            Value = false;
            DisableTimes += 1;
        }

        public override void ResetForSpawn()
        {
        }

        public override void ReleaseForDespawn()
        {
        }
    }
}