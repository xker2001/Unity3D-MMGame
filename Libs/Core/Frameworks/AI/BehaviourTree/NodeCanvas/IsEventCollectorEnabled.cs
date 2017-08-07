
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("IsEventCollectorEnabled")]
    [Description("检查一个 EventCollector 组件是否处于启用状态。")]
    public class IsEventCollectorEnabled : IsServiceComponentEnabled<EventCollector>
    {
    }
}
#endif