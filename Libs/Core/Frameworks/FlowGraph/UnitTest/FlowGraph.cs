using UnityEngine;

namespace MMGame.FlowGraph.UnitTest
{
    public class FlowGraph : AFlowGraph
    {
        [SerializeField]
        private ActionSelfComplete actionSelfComplete;

        [SerializeField]
        private ActionFlag actionFlag;

        [SerializeField]
        private ActionElapse actionElapse;

        [SerializeField]
        private ActionLast actionLast;

        [SerializeField]
        private ActionSelfComplete3Times actionSelfComplete3Times;

        [SerializeField]
        private ActionSelfCompleteLoop actionSelfCompleteLoop;

        protected override void Design()
        {
            // 时间触发播放
            BindPlay(actionSelfComplete, () => true, 1, 1);
            // 测试 3 次循环和无限循环
            BindPlay(actionSelfComplete3Times, () => true, 0, 3);
            BindPlay(actionSelfCompleteLoop, () => true, 0, -1);
            // 自我结束触发播放
            BindPlay(actionFlag, () => actionSelfComplete.IsSelfCompleted, 0, 1);
            // Flags 触发播放
            BindPlay(actionElapse, () => actionFlag.Flags == (32 | 128), 0, 1);
            // 运行时间触发播放
            BindPlay(actionLast, () => actionElapse.Elapse >= 1.5f, 0, 1);
            BindStop(actionLast, () => actionElapse.Elapse >= 2f, 0, 1);
            // 条件 + 延迟
            BindPlay(actionLast, () => actionElapse.Elapse > 4.5f, 1, 1);
            BindStop(actionLast, () => actionElapse.Elapse >= 7.5f, 0, 1);
            // 重新进入延迟 play 状态，用于测试全体 stop
            BindPlay(actionLast, () => actionElapse.Elapse > 8.0f, 1, 1);
        }
    }
}