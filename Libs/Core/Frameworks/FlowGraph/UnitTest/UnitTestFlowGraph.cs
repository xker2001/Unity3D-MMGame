using UnityEngine;

namespace MMGame.FlowGraph.UnitTest
{
    public class UnitTestFlowGraph : MMGame.UnitTest
    {
        // 暂停播放
        // 恢复播放
        // 停止播放
        [SerializeField]
        private AFlowGraph graph;

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

        [SerializeField]
        private ActionPlayAndComplete actionPlayAndComplete;

        [SerializeField]
        private ActionPlayAndStop actionPlayAndStop;

        [TestRun(0)]
        public void StartGraph()
        {
            graph.Play();
        }

        [TestMethod(0)]
        public void TestCompleteAndStopImmediately()
        {
            actionPlayAndComplete.Play();
            IsFalse(actionPlayAndComplete.IsPlaying);
            IsTrue(actionPlayAndComplete.IsSelfCompleted);

            actionPlayAndStop.Play();
            actionPlayAndStop.Stop();
            IsFalse(actionPlayAndStop.IsPlaying);
            IsFalse(actionPlayAndStop.IsSelfCompleted);
        }

        [TimeTestMethod(0.99f)]
        public void TestBeforDelayPlay()
        {
            IsFalse(actionSelfComplete.ExecutePlayIsCalled);

            IsFalse(actionSelfComplete.IsPlaying);
            IsFalse(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 1s (delay)
        [TimeTestMethod(1.11f)] // 0.11 冗余
        public void TestDelayPlay()
        {
            IsTrue(actionSelfComplete.ExecutePlayIsCalled);

            IsTrue(actionSelfComplete.IsPlaying);
            IsFalse(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 0.9s (self complete)
        [TimeTestMethod(1.9f)]
        public void TestBeforeActionSelfComplete()
        {
            IsFalse(actionSelfComplete.IsSelfCompleted);

            IsTrue(actionSelfComplete.IsPlaying);
            IsFalse(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 1s (self complete)
        [TimeTestMethod(2.21f)] // 0.21 冗余
        public void TestActionSelfComplete()
        {
            IsTrue(actionSelfComplete.IsSelfCompleted);

            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);

            AreEqual(actionFlag.Flags, 0);
        }

        // after 1.9s (seg flags)
        [TimeTestMethod(3.9f)]
        public void TestBeforeActonSetFlag()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 2s (set flags)
        [TimeTestMethod(4.31f)] // 0.31 冗余
        public void TestActonSetFlag()
        {
            AreEqual(actionFlag.Flags, 32 | 128);

            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 1.4s (elapse 1.5)
        [TimeTestMethod(5.5f)]
        public void TestBeforeActonElapseTriggerPlay()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 1.5s (elapse 1.5)
        [TimeTestMethod(5.91f)] // 0.41 冗余
        public void TestActonElapseTriggerPlay()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsTrue(actionLast.IsPlaying);
        }

        // after 0.4s (elapse 2)
        [TimeTestMethod(6.0f)]
        public void TestBeforeActonElapseTriggerStop()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsTrue(actionLast.IsPlaying);
        }

        // after 0.5s (elapse 2)
        [TimeTestMethod(6.41f)] // 0.41 冗余
        public void TestActonElapseTriggerStop()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 3.5s (elapse 4.5 + delay 1)
        [TimeTestMethod(9.5f)]
        public void TestActonElapseTriggerDelayPlay()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);
        }

        // after 3.5s (elapse 4.5 + delay 1)
        [TimeTestMethod(9.91f)] // 0.41 冗余
        public void TestActonElapseTriggerDelayPlayDone()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsTrue(actionLast.IsPlaying);
        }

        // after 1s (elapse 5.5)
        [TimeTestMethod(11.91f)] // 0.41 冗余
        public void TestActonElapseTriggerStopAgain()
        {
            IsFalse(actionSelfComplete.IsPlaying);
            IsTrue(actionFlag.IsPlaying);
            IsTrue(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);

            IsFalse(actionElapse.ExecuteStopIsCalled);
        }

        [TimeTestMethod(12.41f)] // 0.41 冗余
        public void TestStopAllWhenCoroutineIsRunning()
        {
            graph.Stop();
            IsFalse(actionSelfComplete.IsPlaying);
            IsFalse(actionFlag.IsPlaying);
            IsFalse(actionElapse.IsPlaying);
            IsFalse(actionLast.IsPlaying);

            IsTrue(actionElapse.ExecuteStopIsCalled);
        }

        [TimeTestMethod(14.41f)] // 1.41 冗余
        public void TestCoroutineIsStopped()
        {
            IsFalse(actionLast.IsPlaying);
        }

        [TimeTestMethod(15f)]
        public void TestPlayTimes()
        {
            AreEqual(actionSelfComplete3Times.PlayTimes, 3);
            IsTrue(actionSelfCompleteLoop.PlayTimes > 10);
        }
    }
}