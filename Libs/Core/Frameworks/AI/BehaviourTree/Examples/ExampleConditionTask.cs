namespace MMGame.AI.BehaviourTree.Example
{
    public class ExampleConditionTask : ConditionTask
    {
        public override bool OnCheck()
        {
            return true;
        }

        public override void ResetForSpawn()
        {
        }

        public override void ReleaseForDespawn()
        {
        }
    }
}