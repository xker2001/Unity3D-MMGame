namespace MMGame.Event.UnitTest
{
    public static class UnitTestEventType
    {
        public static readonly int TestEventType;

        static UnitTestEventType()
        {
            TestEventType = EventId.GetId();
        }
    }
}