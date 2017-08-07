
#if PROCAMERA2D
using UnityEngine.Assertions;

namespace MMGame.Scene2D.ProCamera2D
{
    public class Pro2DLayerCamera : ALayerCamera
    {
        private Com.LuisPedroFonseca.ProCamera2D.ProCamera2D proCamera;

        protected override void Awake()
        {
            base.Awake();
            proCamera = Com.LuisPedroFonseca.ProCamera2D.ProCamera2D.Instance;
            Assert.IsNotNull(proCamera);
        }

        protected override void InitCamera()
        {
            StartPosition = cameraXform.position;
        }

        protected override void RegisterUpdater()
        {
            UpdateManager.RegisterLateUpdate(UpdateCamera);
        }

        protected override void UnregisterUpdater()
        {
            UpdateManager.UnregisterLateUpdate(UpdateCamera);
        }

        private void UpdateCamera(float deltaTime)
        {
            OnFirstRun();
            proCamera.Move(deltaTime);
            InvokeCallbacks();
        }
    }
}
#endif