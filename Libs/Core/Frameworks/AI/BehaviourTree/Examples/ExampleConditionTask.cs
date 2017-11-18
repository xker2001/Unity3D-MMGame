namespace MMGame.AI.BehaviourTree.Example
{
    public class ExampleConditionTask : ConditionTask
    {
        public override bool OnCheck()
        {
            return true;
        }

        public override void OnSpawn()
        {
        }

        public override void OnDespawn()
        {
        }
    }
}