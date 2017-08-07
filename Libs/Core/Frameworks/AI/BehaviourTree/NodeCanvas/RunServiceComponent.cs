
#if NODE_CANVAS
namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    abstract public class RunServiceComponent<T> : ServiceComponentController<T>
        where T : ServiceComponent
    {
        protected sealed override void OnExecute()
        {
            Component.Enable();
        }

        protected sealed override void OnStop()
        {
            Component.Disable();
        }

        protected sealed override string Operation
        {
            get { return "Run"; }
        }
    }
}
#endif