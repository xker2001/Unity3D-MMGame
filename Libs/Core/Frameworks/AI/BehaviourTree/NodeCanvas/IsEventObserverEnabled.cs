
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("IsEventObserverEnabled")]
    [Description("检查一个 EventObserver 组件是否处于启用状态。")]
    public class IsEventObserverEnabled : IsServiceComponentEnabled<EventObserver>
    {
    }
}
#endif