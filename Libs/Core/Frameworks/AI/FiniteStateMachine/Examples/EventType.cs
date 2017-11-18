using MMGame.Event;

namespace MMGame.AI.FiniteStateMachine.Example
{
    public static class EventType
    {
        public static readonly int Die;

        static EventType()
        {
            Die = EventId.GetId();
        }
    }
}