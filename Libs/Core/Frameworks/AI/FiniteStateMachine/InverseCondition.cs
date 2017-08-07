using UnityEngine.Assertions;

namespace MMGame.AI.FiniteStateMachine
{
    public class InverseCondition : ICondition
    {
        private ICondition condition;

        public InverseCondition(ICondition condition)
        {
            Assert.IsNotNull(condition);
            this.condition = condition;
        }

        public bool IsInitialized
        {
            get { return condition.IsInitialized; }
        }

        public void Init()
        {
            condition.Init();
        }

        public bool Check()
        {
            return !condition.Check();
        }
    }
}