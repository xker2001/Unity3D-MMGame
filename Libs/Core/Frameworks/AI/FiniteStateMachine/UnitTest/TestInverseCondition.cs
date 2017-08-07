namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestInverseCondition : Condition
    {
        private bool value = true;

        public bool Value
        {
            get { return this.value; }
            private set { this.value = value; }
        }

        public void Set(bool value)
        {
            Value = value;
        }

        public string Info { get; private set; }

        public TestInverseCondition SetInfo(string info)
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