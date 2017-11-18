namespace MMGame.FlowGraph.UnitTest
{
    public class ActionSelfComplete : AFlowAction
    {
        public bool ExecutePlayIsCalled { get; private set; }

        protected override void ExecutePlay()
        {
            ExecutePlayIsCalled = true;
            Invoke("Complete", 1f);
        }

        protected override void ExecuteStop() {}

        private void Complete()
        {
            SetSelfCompleted();
        }
    }
}