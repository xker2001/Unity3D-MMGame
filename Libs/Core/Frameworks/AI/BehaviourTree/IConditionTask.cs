namespace MMGame.AI.BehaviourTree
{
    public interface IConditionTask
    {
        void OnInit();
        bool OnCheck();
    }
}