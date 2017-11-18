namespace MMGame.FlowGraph.UnitTest
{
    public class ActionSelfComplete3Times : AFlowAction
    {
        public int PlayTimes { get; private set; }

        protected override void ExecutePlay()
        {
            PlayTimes += 1;
            Invoke("Complete", 0.5f);
        }

        protected override void ExecuteStop() {}

        private void Complete()
        {
            SetSelfCompleted();
        }
    }
}