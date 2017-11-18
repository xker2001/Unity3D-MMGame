using MMGame.Event;

namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public static class EventType
    {
        public static readonly int Type1;
        public static readonly int Type2;
        public static readonly int Type3;
        public static readonly int Type4;
        public static readonly int Type5;
        public static readonly int Type6;
        public static readonly int Type1001;
        public static readonly int Type1002;

        static EventType()
        {
            Type1 = EventId.GetId();
            Type2 = EventId.GetId();
            Type3 = EventId.GetId();
            Type4 = EventId.GetId();
            Type5 = EventId.GetId();
            Type6 = EventId.GetId();
            Type1001 = EventId.GetId();
            Type1002 = EventId.GetId();
        }
    }
}