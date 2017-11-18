namespace MMGame.FlowGraph.UnitTest
{
    public class ActionElapse : AFlowAction
    {
        public bool ExecuteStopIsCalled { get; private set; }

        protected override void ExecutePlay() {}

        protected override void ExecuteStop()
        {
            ExecuteStopIsCalled = true;
        }
    }
}