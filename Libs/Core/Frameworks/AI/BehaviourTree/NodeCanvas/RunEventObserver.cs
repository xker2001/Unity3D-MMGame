
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("RunEventObserver")]
    [Description("在开始执行时启用一个 EventObserver 组件，在结束执行时关闭它。")]
    public class RunEventObserver : RunServiceComponent<EventObserver>
    {
    }
}
#endif