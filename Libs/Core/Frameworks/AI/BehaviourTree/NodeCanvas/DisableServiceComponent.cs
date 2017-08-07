
#if NODE_CANVAS
namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    abstract public class DisableServiceComponent<T> : ServiceComponentController<T>
        where T : ServiceComponent
    {
        protected sealed override void OnExecute()
        {
            Component.Disable();
        }

        protected sealed override string Operation
        {
            get { return "Disable"; }
        }
    }
}
#endif