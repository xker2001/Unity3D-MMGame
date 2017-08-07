using UnityEngine;

namespace MMGame.AI.FiniteStateMachine
{
    /// <summary>
    /// 负责进行条件检查的类。
    /// </summary>
    abstract public class Condition : MonoBehaviour, ICondition
    {
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 初始化。
        /// </summary>
        public void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            OnInit();
            IsInitialized = true;
        }

        /// <summary>
        /// 获取条件检查的结果。
        /// </summary>
        /// <returns>true or false。</returns>
        public bool Check()
        {
            return OnCheck();
        }

        /// <summary>
        /// 在这里编写初始化逻辑。
        /// </summary>
        virtual protected void OnInit()
        {
        }

        /// <summary>
        /// 在这里编写条件检查的逻辑。
        /// </summary>
        /// <returns>true or false。</returns>
        abstract protected bool OnCheck();
    }
}