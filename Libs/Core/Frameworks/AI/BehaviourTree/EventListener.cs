namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 负责侦听事件、解包事件数据并调用使用处理方法的组件。
    /// 事件处理方法通常从其他组件中取得。
    /// </summary>
    abstract public class EventListener : ServiceComponent
    {
    }
}