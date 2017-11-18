namespace MMGame.FlowGraph.UnitTest
{
    public class ActionPlayAndComplete : AFlowAction
    {
        protected override void ExecutePlay()
        {
            SetSelfCompleted();
        }

        protected override void ExecuteStop() {}
    }
}