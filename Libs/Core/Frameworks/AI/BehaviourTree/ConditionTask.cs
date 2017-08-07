namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 作为行为树 ConditionTask 存在的组件。
    /// </summary>
    abstract public class ConditionTask : PoolBehaviour
    {
        virtual public void OnInit()
        {
        }

        abstract public bool OnCheck();
    }
}