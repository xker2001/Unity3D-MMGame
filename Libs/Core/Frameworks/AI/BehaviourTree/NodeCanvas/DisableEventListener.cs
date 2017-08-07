
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("DisableEventListener")]
    [Description("禁用一个 EventListener 组件。")]
    public class DisableEventListener : DisableServiceComponent<EventListener>
    {
    }
}
#endif