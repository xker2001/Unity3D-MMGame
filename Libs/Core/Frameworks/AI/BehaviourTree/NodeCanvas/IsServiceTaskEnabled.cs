
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("IsServiceTaskEnabled")]
    [Description("检查一个 ServiceTask 组件是否处于启用状态。")]
    public class IsServiceTaskEnabled : IsServiceComponentEnabled<ServiceTask>
    {
    }
}
#endif