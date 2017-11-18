namespace MMGame.FlowGraph.UnitTest
{
    public class ActionFlag : AFlowAction
    {
        protected override void ExecutePlay()
        {
            Invoke("SetFlag", 2f);
        }

        protected override void ExecuteStop() {}

        private void SetFlag()
        {
            Flags = 32 | 128;
        }
    }
}