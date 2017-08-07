
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("RunEventCollector")]
    [Description("在开始执行时启用一个 EventCollector 组件，在结束执行时关闭它。")]
    public class RunEventCollector : RunServiceComponent<EventCollector>
    {
    }
}
#endif