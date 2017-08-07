
#if NODE_CANVAS
using ParadoxNotion.Design;
using UnityEngine;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    /// <summary>
    /// 专门用于调用 ActionTask 组件的 NodeCanvas ActionTask。
    /// </summary>
    [Category("MMGame")]
    [Name("MonoActionTask")]
    [Description("MMGame.BehaviourTree.ActionTask 的适配器。")]
    public class MonoActionTask : global::NodeCanvas.Framework.ActionTask
    {
        public ActionTask Component;

        protected override string info
        {
            get
            {
                string output = "<b>Run ActionTask</b>\n";

                return output + (Component
                                     ? (Component.name == agent.name ? "" : Component.name + " : ") +
                                       Component.GetType().Name
                                     : "?");
            }
        }

        protected override void OnExecute()
        {
            Component.OnExecute();
            CheckStatus();
        }

        protected override void OnUpdate()
        {
            Component.OnUpdate(Time.deltaTime);
            CheckStatus();
        }

        protected override void OnStop()
        {
            Component.OnStop();
        }

        protected override void OnPause()
        {
            Component.OnPause();
        }

        protected override string OnInit()
        {
            if (Component == null)
            {
                return "ActionTask not assigned!";
            }

            Component.OnInit();
            return null;
        }

        private void CheckStatus()
        {
            if (Component.Status == TaskStatus.Failure)
            {
                EndAction(false);
            }
            else if (Component.Status == TaskStatus.Success)
            {
                EndAction(true);
            }
        }
    }
}
#endif