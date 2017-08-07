
#if PROCAMERA2D
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D.ProCamera2D
{
    public class ProCamera2DTarget : ACameraTarget
    {
        [SerializeField]
        private Transform target;

        private Com.LuisPedroFonseca.ProCamera2D.ProCamera2D proCamera;

        public override Transform Target
        {
            get { return target; }
        }

        void Awake()
        {
            proCamera = Com.LuisPedroFonseca.ProCamera2D.ProCamera2D.Instance;
            Assert.IsNotNull(target);
            Assert.IsNotNull(proCamera);
        }

        void OnEnable()
        {
            proCamera.AddCameraTarget(target);
        }

        void OnDisable()
        {
            proCamera.RemoveCameraTarget(target);
        }
    }
}
#endif