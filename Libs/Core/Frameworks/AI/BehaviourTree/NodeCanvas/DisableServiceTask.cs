
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("DisableServiceTask")]
    [Description("禁用一个 ServiceTask 组件。")]
    public class DisableServiceTask : DisableServiceComponent<ServiceTask>
    {
    }
}
#endif