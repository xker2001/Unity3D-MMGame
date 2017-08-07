namespace MMGame.AI.FiniteStateMachine.Example
{
    public class ComponentShowHud : ServiceProvider
    {
        // ----------------------------------------------------
        // 说明：
        // ServiceProvider 继承了 MonoBehaviour，状态机直接修改其 enabled 来控制其启用和停用。
        //
        // 因此：
        // - 可以通过 OnEnable() 和 OnDisable() 中执行起停时需要的逻辑。
        // - 可以在 Awake() 或 Start() 中执行初始化逻辑。
        // - 可以使用 Update()、StartCorouting()、Invoke() 等方法控制逻辑流程。
        // - 推荐使用 UpdateManager 服务获得高性能的 Update() 调用。
        // ----------------------------------------------------

        void OnEnable()
        {
            AllocateResources();
            UpdateManager.RegisterUpdate(FastUpdate); // 将 FastUpdate() 注册到 UpdateManager。
        }

        void OnDisable()
        {
            ReleaseResources();
            UpdateManager.UnregisterUpdate(FastUpdate); // 从 UpdateManager 注销 FastUpdate()
        }

        // 每帧调用的函数。
        // 也可以使用 Unity 内建支持的 Update()、StartCoroutine() 等方法来实现。
        private void FastUpdate(float deltaTime)
        {
            UpdateHud();
        }

        // 每次服务暂停时被调用。注意需要自行管理暂停逻辑。
        // 注意，如果使用了 UpdateManager 服务，在全局暂停时 FastUpdate 也被强制暂停，无需在这里处理。
        protected override void OnPause()
        {
            // do something
        }

        // 每次服务恢复运行时被调用
        protected override void OnResume()
        {
            // do something
        }

        // 从对象池获取资源
        private void AllocateResources()
        {
            // do something
        }

        // 返还资源给对象池
        private void ReleaseResources()
        {
            // do something
        }

        // 更新 hud 的位置和内容
        private void UpdateHud()
        {
            // do something
        }

        public override void ResetForSpawn()
        {
        }

        public override void ReleaseForDespawn()
        {
        }
    }
}