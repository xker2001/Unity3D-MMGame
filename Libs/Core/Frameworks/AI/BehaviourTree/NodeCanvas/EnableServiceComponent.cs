
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    abstract public class EnableServiceComponent<T> : ServiceComponentController<T>
        where T : ServiceComponent
    {
        protected sealed override void OnExecute()
        {
            Component.Enable();
        }

        protected sealed override string Operation
        {
            get { return "Enable"; }
        }
    }
}
#endif