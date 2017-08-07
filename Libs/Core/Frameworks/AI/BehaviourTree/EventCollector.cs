namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 负责侦听事件、将事件及事件数据包缓存到事件缓存中，供下一次行为树执行时使用。
    /// </summary>
    abstract public class EventCollector : ServiceComponent
    {
    }
}