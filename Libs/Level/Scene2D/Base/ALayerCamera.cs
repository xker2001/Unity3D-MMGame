using System;
using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 层摄像机，主要负责：
    /// 1. 摄像机行为如移动等（可通过第三方组件实现）。
    /// 2. 向外部提供摄像机更新回调服务。
    /// 3. 提供摄像机相关数据，如可视范围大小。
    /// </summary>
    abstract public class ALayerCamera : MonoBehaviour
    {
        [SerializeField]
        protected Camera camera;
        protected Transform cameraXform;
        private Vector3 startPosition;
        private bool isFirstUpdate = true;

        public event Action<ALayerCamera> CameraUpdated;
        public event Action<ALayerCamera> CameraLateUpdated;

        /// <summary>
        /// 所用的 Camera
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }

        /// <summary>
        /// 摄像机的起始位置（开始运动前的位置）。
        /// </summary>
        public Vector3 StartPosition
        {
            get { return startPosition; }
            set
            {
                startPosition = value;
                cameraXform.position = value;
            }
        }

        /// <summary>
        /// 摄像机当前位置。
        /// </summary>
        public Vector3 Position
        {
            get { return cameraXform.position; }
        }

        virtual protected void Awake()
        {
            cameraXform = camera.transform;
        }

        virtual protected void OnEnable()
        {
            RegisterUpdater();
        }

        virtual protected void OnDisable()
        {
            UnregisterUpdater();
        }

        /// <summary>
        /// 获取当前屏幕对应的世界坐标大小。
        /// </summary>
        /// <param name="rayLength">透视摄像机计算世界坐标所用的射线长度。</param>
        /// <returns>长宽二维矢量。</returns>
        public Vector2 GetScreenUnitSize(float rayLength = 5)
        {
            if (camera.orthographic)
            {
                return new Vector2(camera.orthographicSize * camera.aspect * 2, camera.orthographicSize * 2);
            }
            else
            {
                Vector3 ltPoint = camera.ScreenToWorldPoint(new Vector3(0, 0, rayLength));
                Vector3 rbPoint = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, rayLength));
                return new Vector2(Mathf.Abs(rbPoint.x - ltPoint.x), Mathf.Abs(rbPoint.y - ltPoint.y));
            }
        }

        /// <summary>
        /// 第一次运行时初始化摄像机位置（及其他）、第一次调用回调函数。
        /// </summary>
        protected void OnFirstRun()
        {
            if (isFirstUpdate)
            {
                InitCamera();
                InvokeCallbacks();
                isFirstUpdate = false;
            }
        }

        /// <summary>
        /// 执行外部注册的回调函数。
        /// </summary>
        protected void InvokeCallbacks()
        {
            if (CameraUpdated != null)
            {
                CameraUpdated(this);
            }

            if (CameraLateUpdated != null)
            {
                CameraLateUpdated(this);
            }
        }

        /// <summary>
        /// 初始化 Camera，如位置、FOV 等。
        /// </summary>
        abstract protected void InitCamera();

        /// <summary>
        /// 注册到更新器，如 UpdateManager。
        /// </summary>
        abstract protected void RegisterUpdater();

        /// <summary>
        /// 从更新器注销。
        /// </summary>
        abstract protected void UnregisterUpdater();
    }
}