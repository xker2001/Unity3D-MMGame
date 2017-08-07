namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 作为行为树 ActionTask 存在的组件。
    /// </summary>
    abstract public class ActionTask : PoolBehaviour
    {
        public TaskStatus Status { get; protected set; }

        virtual public void OnInit()
        {
        }

        virtual public void OnExecute()
        {
        }

        virtual public void OnUpdate(float deltaTime)
        {
        }

        virtual public void OnStop()
        {
        }

        virtual public void OnPause()
        {
        }
    }
}