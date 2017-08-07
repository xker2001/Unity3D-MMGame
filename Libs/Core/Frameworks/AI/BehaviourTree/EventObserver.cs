namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 负责检查事件缓存中是否存在关心的事件数据，解包并调用事件处理方法。
    /// 事件处理方法通常从其他组件中取得。
    /// </summary>
    abstract public class EventObserver : ServiceComponent
    {
    }
}