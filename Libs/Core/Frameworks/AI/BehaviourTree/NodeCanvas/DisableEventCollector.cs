
#if NODE_CANVAS
using System.Diagnostics;
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("DisableEventCollector")]
    [Description("禁用一个 EventCollector 组件。")]
    public class DisableEventCollector : DisableServiceComponent<EventCollector>
    {
    }
}
#endif