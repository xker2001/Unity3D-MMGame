
#if NODE_CANVAS
using ParadoxNotion.Design;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    /// <summary>
    /// 专门用于调用 ConditionTask 组件的 NodeCanvas ConditionTask。
    /// </summary>
    [Category("MMGame")]
    [Name("MonoConditionTask")]
    [Description("MMGame.BehaviourTree.ConditionTask 的适配器。")]
    public class MonoConditionTask : global::NodeCanvas.Framework.ConditionTask
    {
        public ConditionTask Component;

        protected override bool OnCheck()
        {
            return Component.OnCheck();
        }

        protected override string OnInit()
        {
            if (Component == null)
            {
                return "ConditionTask not assigned!";
            }

            Component.OnInit();
            return null;
        }

        protected override string info
        {
            get
            {
                string output = "<b>Check ConditionTask</b>\n";

                return output + (Component
                           ? (Component.name == agent.name ? "" : Component.name + " : ") + Component.GetType().Name
                           : "?");
            }
        }
    }
}
#endif