namespace MMGame.Event
{
    /// <summary>
    /// 事件类型 id 生成器。
    /// </summary>
    public static class EventId
    {
        private static int id;

        public static int GetId()
        {
            return id++;
        }
    }
}