
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("EnableEventObserver")]
    [Description("启用一个 EventObserver 组件。")]
    public class EnableEventObserver : EnableServiceComponent<EventObserver>
    {
    }
}
#endif