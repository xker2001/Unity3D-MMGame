namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestCondition : Condition
    {
        public bool Value { get; private set; }

        public void Set(bool value)
        {
            Value = value;
        }

        public string Info { get; private set; }

        public TestCondition SetInfo(string info)
        {
            Info = info;
            return this;
        }

        protected override bool OnCheck()
        {
            return Value;
        }
    }
}