namespace MMGame.Scene2D
{
    public class FixedLayerCamera : ALayerCamera
    {
        protected override void Awake()
        {
            base.Awake();
            InitCamera();
        }

        void Start()
        {
            InvokeCallbacks();
        }

        protected override void InitCamera()
        {
            StartPosition = cameraXform.position;
        }

        protected override void RegisterUpdater()
        {
        }

        protected override void UnregisterUpdater()
        {
        }
    }
}