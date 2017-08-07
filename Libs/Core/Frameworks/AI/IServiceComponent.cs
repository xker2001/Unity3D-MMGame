namespace MMGame.AI
{
    public interface IServiceComponent
    {
        bool IsEnabled { get; }
        bool IsPaused { get; }

        void Enable();
        void Disable();
        void Pause();
        void Resume();
    }
}