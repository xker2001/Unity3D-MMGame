
#if NODE_CANVAS
namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    abstract public class ServiceComponentController<T> : global::NodeCanvas.Framework.ActionTask
        where T : ServiceComponent
    {
        // T 不约束到 IServiceTask 是因为 Easy Editor 无法支持 NodeCanvas 的 Inspector
        public T Component;

        protected abstract string Operation { get; }

        protected sealed override string info
        {
            get
            {
                string output = "<b>" + Operation + " " + typeof(T).Name + "</b>\n";

                return output + (Component
                                     ? (Component.name == agent.name ? "" : Component.name + " : ") +
                                       Component.GetType().Name
                                     : "?");
            }
        }
    }
}
#endif