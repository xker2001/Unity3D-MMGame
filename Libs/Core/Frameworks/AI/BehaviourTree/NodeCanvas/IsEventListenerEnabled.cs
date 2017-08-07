
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("IsEventListenerEnabled")]
    [Description("检查一个 EventListener 组件是否处于启用状态。")]
    public class IsEventListenerEnabled : IsServiceComponentEnabled<EventListener>
    {
    }
}
#endif