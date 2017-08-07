namespace MMGame.AI.BehaviourTree
{
    public interface IActionTask
    {
        TaskStatus Status { get; }

        /// <summary>
        /// 仅在第一次执行时执行一次
        /// </summary>
        void OnInit();

        /// <summary>
        /// 每次执行时执行一次
        /// </summary>
        void OnExecute();

        /// <summary>
        /// 每个 Tick 执行一次
        /// </summary>
        /// <param name="deltaTime"></param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// 因为任何原因停止时执行
        /// </summary>
        void OnStop();

        /// <summary>
        /// 暂停时执行
        /// </summary>
        void OnPause();
    }
}