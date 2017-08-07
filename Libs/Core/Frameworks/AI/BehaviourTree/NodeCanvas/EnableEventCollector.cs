
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("EnableEventCollector")]
    [Description("启用一个 EventCollector 组件。")]
    public class EnableEventCollector : EnableServiceComponent<EventCollector>
    {
    }
}
#endif