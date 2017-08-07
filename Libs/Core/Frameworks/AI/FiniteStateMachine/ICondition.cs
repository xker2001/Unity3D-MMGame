namespace MMGame.AI.FiniteStateMachine
{
    public interface ICondition
    {
        bool IsInitialized { get; }
        void Init();
        bool Check();
    }
}