namespace MMGame.AI.FiniteStateMachine.Example
{
    public class StateJump : FsmState
    {
        private bool isFinished;

        // 在第一次进入该状态时被调用。
        // 通常用于初始化变量、申请状态所需资源。
        // 注意，为了与对象池更好配合，应当尽可能在 OnEnter() 中申请对象池资源，在 OnStop() 中释放对象池资源。
        // FsmState 继承了 MonoBehaviour，如果希望提前对状态进行初始化，可以使用 Awake() 或 Start() 方法。
        protected override void OnInit()
        {
            // do something
        }

        // 每次进入状态时被调用
        protected override void OnEnter()
        {
            // do something
        }

        // 如果是当前状态，状态机暂停时调用。
        // 注意当状态机暂停时当前状态的 OnUpdate() 不会被调用，无需特意处理。
        protected override void OnPause()
        {
            // do something
        }

        // 如果是当前状态，状态机恢复运行时调用
        protected override void OnResume()
        {
            // do something
        }

        // 状态因任何原因停止时被调用
        protected override void OnStop()
        {
            // do something
        }

        // 状态激活时每帧被调用
        protected override void OnUpdate(float deltaTime)
        {
            // 更新 jump 过程
            // ...

            // 使用 Exit 指令退出 jump 状态
            if (isFinished)
            {
                Exit(0);
            }

            // ...

            isFinished = true;
        }
    }
}