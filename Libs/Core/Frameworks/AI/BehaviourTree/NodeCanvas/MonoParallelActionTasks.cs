
#if NODE_CANVAS
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace MMGame.AI.BehaviourTree.NodeCanvas
{
    /// <summary>
    /// "用并行的方式批量调用 ActionTask 组件。
    /// </summary>
    [Category("MMGame")]
    [Name("MonoParallelActionTasks")]
    [Description("用并行的方式批量调用 MMGame.BehaviourTree.ActionTask。")]
    public class MonoParallelActionTasks : global::NodeCanvas.Framework.ActionTask
    {
        public enum ParallelPolicy
        {
            FirstFailure,
            FirstSuccess,
            FirstSuccessOrFailure
        }

        [Serializable]
        public class ActionTaskItem
        {
            public bool Enabled = true;
            public ActionTask Task;
        }

        public ParallelPolicy Policy = ParallelPolicy.FirstFailure;

        public List<ActionTaskItem> Components;


        protected override
            string info
        {
            get
            {
                string output = "<b>Parallel Run</b>";

                if (Components == null)
                {
                    return output;
                }

                for (int i = 0; i < Components.Count; i++)
                {
                    ActionTaskItem item = Components[i];
                    ActionTask comp = item.Task;

                    output += "\n" + (comp
                                  ? (item.Enabled ? "" : "<color=grey>") +
                                    (comp.name == agent.name ? "" : comp.name + " : ") + comp.GetType().Name +
                                    (item.Enabled ? "" : "</color>")
                                  : "?")
                        ;
                }

                return output;
            }
        }

        protected override
            void OnExecute()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                ActionTaskItem item = Components[i];

                if (!item.Enabled)
                {
                    continue;
                }

                item.Task.OnExecute();

                if (IsFinished(item.Task))
                {
                    break;
                }
            }
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                ActionTaskItem item = Components[i];

                if (!item.Enabled)
                {
                    continue;
                }

                item.Task.OnUpdate(Time.deltaTime);
                if (IsFinished(item.Task))
                {
                    break;
                }
            }
        }

        protected override void OnStop()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                ActionTaskItem item = Components[i];

                if (!item.Enabled)
                {
                    continue;
                }

                item.Task.OnStop();
            }
        }

        protected override void OnPause()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                ActionTaskItem item = Components[i];

                if (!item.Enabled)
                {
                    continue;
                }

                item.Task.OnPause();
            }
        }

        protected override string OnInit()
        {
            if (Components == null)
            {
                return "ActionTasks not assigned!";
            }

            for (int i = 0; i < Components.Count; i++)
            {
                ActionTaskItem item = Components[i];

                if (!item.Enabled)
                {
                    continue;
                }

                if (item.Task == null)
                {
                    return "Enabled ActionTask not assigned!";
                }

                item.Task.OnInit();
            }

            return null;
        }

        private bool IsFinished(ActionTask component)
        {
            if (component.Status == TaskStatus.Failure &&
                (Policy == ParallelPolicy.FirstFailure || Policy == ParallelPolicy.FirstSuccessOrFailure))
            {
                EndAction(false);
                return true;
            }
            else if (component.Status == TaskStatus.Success &&
                     (Policy == ParallelPolicy.FirstSuccess || Policy == ParallelPolicy.FirstSuccessOrFailure))
            {
                EndAction(true);
                return true;
            }

            return false;
        }
    }
}
#endif