namespace MMGame.AI
{
    /// <summary>
    /// 承担独立任务的组件。
    /// 在行为树中作用类似 UE4 中的 Service。
    /// </summary>
    abstract public class ServiceComponent : PoolBehaviour, IServiceComponent
    {
        public bool IsEnabled
        {
            get { return enabled; }
        }

        public bool IsPaused { get; private set; }

        public void Enable()
        {
            if (!IsEnabled)
            {
                enabled = true;
                // OnEnable is called here
            }
        }

        public void Disable()
        {
            if (IsEnabled)
            {
                enabled = false;
                // OnDisable is called here

                IsPaused = false;
            }
        }

        public void Pause()
        {
            OnPause();
            IsPaused = true;
        }

        public void Resume()
        {
            OnResume();
            IsPaused = false;
        }

        virtual protected void OnPause()
        {
        }

        virtual protected void OnResume()
        {
        }
    }
}