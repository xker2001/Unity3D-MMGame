
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("EnableServiceTask")]
    [Description("启用一个 ServiceTask 组件。")]
    public class EnableServiceTask : EnableServiceComponent<ServiceTask>
    {
    }
}
#endif