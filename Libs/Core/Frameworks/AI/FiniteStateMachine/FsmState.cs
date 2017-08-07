using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.AI.FiniteStateMachine
{
    abstract public class FsmState : MonoBehaviour
    {
        private bool isInitialized;

        /// <summary>
        /// 退出码，供状态机决定退出后的线路，>= 0。
        /// </summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// 状态机是否在运行。
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 状态机是否处于暂停状态。
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 进入状态。
        /// </summary>
        public void Enter()
        {
            if (!isInitialized)
            {
                OnInit();
                isInitialized = true;
            }

            ExitCode = -1;
            OnEnter();
            IsRunning = true;
        }

        /// <summary>
        /// 更新状态。
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateState(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        /// <summary>
        /// 暂停状态。
        /// </summary>
        public void Pause()
        {
            OnPause();
            IsPaused = true;
        }

        /// <summary>
        /// 恢复运行状态。
        /// </summary>
        public void Resume()
        {
            OnResume();
            IsPaused = false;
        }

        /// <summary>
        /// 停止状态。
        /// </summary>
        public void Stop()
        {
            OnStop();
            IsRunning = false;
            IsPaused = false;
            ExitCode = -1;
        }

        /// <summary>
        /// 退出当前状态。
        /// </summary>
        /// <param name="code">退出编码，必须 >= 0。</param>
        protected void Exit(int code = 0)
        {
            Assert.IsTrue(code >= 0);
            ExitCode = code;
        }

        /// <summary>
        /// 在这里编写初始化状态的逻辑。
        /// </summary>
        virtual protected void OnInit()
        {
        }

        /// <summary>
        /// 在这里编写进入状态的逻辑。
        /// </summary>
        virtual protected void OnEnter()
        {
        }

        /// <summary>
        /// 在这里编写更新状态的逻辑。
        /// </summary>
        /// <param name="deltaTime">本次更新的时长。</param>
        virtual protected void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 在这里编写暂停状态的逻辑。
        /// </summary>
        virtual protected void OnPause()
        {
        }

        /// <summary>
        /// 在这里编写恢复运行状态的逻辑。
        /// </summary>
        virtual protected void OnResume()
        {
        }

        /// <summary>
        /// 在这里编写停止状态的逻辑。
        /// 更复杂的情况是将停止分为中断 OnStop() 和 退出 OnExit()，并分别传入 Condtion、
        /// EventListener、exitCode 参数。不确定是否会存在这种需求，暂时简单化处理。
        /// </summary>
        virtual protected void OnStop()
        {
        }
    }
}