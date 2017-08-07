
#if NODE_CANVAS
namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    abstract public class IsServiceComponentEnabled<T> : global::NodeCanvas.Framework.ConditionTask
        where T : ServiceComponent
    {
        public T Component;

        protected sealed override bool OnCheck()
        {
            return Component.IsEnabled;
        }

        protected sealed override string info
        {
            get
            {
                string output = "<b>" + typeof(T).Name + " enabled</b>\n";

                return output + (Component
                                     ? (Component.name == agent.name ? "" : Component.name + " : ") +
                                       Component.GetType().Name
                                     : "?");
            }
        }
    }
}
#endif