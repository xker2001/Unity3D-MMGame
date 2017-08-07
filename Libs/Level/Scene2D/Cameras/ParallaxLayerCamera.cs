using UnityEngine;

namespace MMGame.Scene2D
{
    public class ParallaxLayerCamera : ALayerCamera
    {
        [SerializeField]
        private ALayerCamera mainLayerCamera;

        [SerializeField]
        private Vector2 parallaxPercent = Vector2.one;

        private Vector3 parallaxScale;

        protected override void Awake()
        {
            base.Awake();
            parallaxScale = new Vector3(parallaxPercent.x, parallaxPercent.y, 1);
        }

        protected override void InitCamera()
        {
            StartPosition = mainLayerCamera.StartPosition;
        }

        protected override void RegisterUpdater()
        {
            mainLayerCamera.CameraUpdated += UpdateCamera;
        }

        protected override void UnregisterUpdater()
        {
            mainLayerCamera.CameraUpdated -= UpdateCamera;
        }

        private void UpdateCamera(ALayerCamera layerCamera)
        {
            OnFirstRun();
            Vector3 deltaPos = mainLayerCamera.Position - mainLayerCamera.StartPosition;
            cameraXform.position = StartPosition + Vector3.Scale(deltaPos, parallaxScale);
            InvokeCallbacks();
        }
    }
}