
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("EnableEventListener")]
    [Description("启用一个 EventListener 组件。")]
    public class EnableEventListener : EnableServiceComponent<EventListener>
    {
    }
}
#endif