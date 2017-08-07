
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    [Category("MMGame")]
    [Name("RunEventListener")]
    [Description("在开始执行时启用一个 EventListener 组件，在结束执行时关闭它。")]
    public class RunEventListener : RunServiceComponent<EventListener>
    {
    }
}
#endif