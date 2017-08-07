namespace MMGame.AI.FiniteStateMachine
{
    /// <summary>
    /// 事件侦听及处理类。
    ///
    /// 一般使用方法：
    ///     - 在 OnEnable() 中注册事件侦听
    ///     - 在 OnDisable() 中注销事件侦听
    ///     - 在事件处理函数中设置 IsTriggered = true
    ///
    /// 注意：
    /// 状态机通过检查 IsTriggered 来判定事件是否触发，因此在某些情况下该组件
    /// 可以不侦听外部事件，而是自身检测某些状态变化并设置 IsTriggered，如检测
    /// 按键是否被按下。
    /// </summary>
    abstract public class EventListener : ServiceComponent
    {
        /// <summary>
        /// 事件侦听器是否已经被触发。
        /// </summary>
        public bool IsTriggered { get; protected set; }

        /// <summary>
        /// 重置事件侦听器的触发标记。
        /// 注意外部并不承诺每次触发后立即清除标记，外部有可能只在其需要时清除标记。
        /// </summary>
        public void ResetTrigger()
        {
            IsTriggered = false;
        }
    }
}