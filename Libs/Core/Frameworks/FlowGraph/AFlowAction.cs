using UnityEngine;

namespace MMGame.FlowGraph
{
    /// <summary>
    /// 支持逻辑流的行为类，适用于需要序列控制或条件控制的逻辑中。
    /// </summary>
    abstract public class AFlowAction : PoolBehaviour
    {
        private float elapse;
        private float startTime;

        public bool IsPlaying { get; private set; }

        /// <summary>
        /// 行为是否已经自行结束运行，在派生类中维护。
        /// </summary>
        public bool IsSelfCompleted { get; protected set; }

        /// <summary>
        /// 行为触发的标志，在派生类中维护。
        /// 注意，行为自行结束或者被中断后依旧保留标志，直到下一次重新运行。
        /// </summary>
        public int Flags { get; protected set; }

        /// <summary>
        /// 行为已经运行的时间。受 timeScale 影响，暂停的时间不计入其中。
        /// 注意，行为自行结束或者被中断后依旧保留时间消耗直到下一次重新运行。
        /// </summary>
        public float Elapse
        {
            get
            {
                if (IsPlaying)
                {
                    return elapse + Time.time - startTime;
                }
                else
                {
                    return elapse;
                }
            }
        }

        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            elapse = 0;
            startTime = Time.time;
            IsPlaying = true;
            IsSelfCompleted = false;
            Flags = 0;
            ExecutePlay();
        }

        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            elapse += Time.time - startTime;
            IsPlaying = false;
            IsSelfCompleted = false;
            ExecuteStop();
        }

        public void Reset()
        {
            elapse = 0;
            startTime = 0;
            IsPlaying = false;
            IsSelfCompleted = false;
            Flags = 0;
            ExecuteReset();
        }

        virtual protected void ExecutePlay() {}
        virtual protected void ExecuteStop() {}
        virtual protected void ExecuteReset() {}

        /// <summary>
        /// 派生类自行结束行为时调用，用于设置正确状态。
        /// </summary>
        protected void SetSelfCompleted()
        {
            if (!IsPlaying)
            {
                return;
            }

            elapse += Time.time - startTime;
            IsPlaying = false;
            IsSelfCompleted = true;
        }
    }
}