using MMGame.Event;

namespace MMGame.AI.FiniteStateMachine.Example
{
    public class EventListenerDie : EventListener
    {
        // ----------------------------------------------------
        // 说明：
        // EventListener 继承了 MonoBehaviour，状态机直接修改其 enabled 来控制其启用和停用。
        //
        // 因此：
        // - 可以通过 OnEnable() 和 OnDisable() 中执行起停时需要的逻辑。
        // - 可以在 Awake() 或 Start() 中执行初始化逻辑。
        // - 可以使用 Update()、StartCorouting()、Invoke() 等方法控制逻辑流程。
        // - 推荐使用 UpdateManager 服务获得高性能的 Update() 调用。
        // ----------------------------------------------------

        void OnEnable()
        {
            // 注册侦听「死亡」事件（基于 MMGame 的事件系统支持）
            this.AddEventListener(EventType.Die, OnDie);
        }

        void OnDisable()
        {
            // 注销侦听「死亡事件」
            this.RemoveEventListener(EventType.Die, OnDie);
        }

        // 事件回调方法
        private void OnDie(EventData e)
        {
            // do something

            // 注意！必须设置 IsTriggered = true，状态机通过这个变量判断状态是否被触发
            // （状态机会在需要的时候重置 IsTriggered，无需手动处理）
            IsTriggered = true;
        }

        // 每次服务暂停时被调用。注意需要自行管理暂停逻辑。
        protected override void OnPause()
        {
            // do something
        }

        // 每次服务恢复运行时被调用
        protected override void OnResume()
        {
            // do something
        }

        public override void OnSpawn() {}

        public override void OnDespawn() {}
    }
}