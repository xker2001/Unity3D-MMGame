
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("DisableEventObserver")]
    [Description("禁用一个 EventObserver 组件。")]
    public class DisableEventObserver : DisableServiceComponent<EventObserver>
    {
    }
}
#endif