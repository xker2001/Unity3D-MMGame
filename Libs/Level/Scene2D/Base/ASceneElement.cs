using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 场景元素，封装对 GameObject 的各种操作，如记录生成位置、显示/隐藏等。
    /// </summary>
    abstract public class ASceneElement
    {
        private Transform prefab;
        private string poolName;
        private Vector3 position = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 relativeScale = Vector3.one;

        public Transform Prefab
        {
            get { return prefab; }
            private set { prefab = value; }
        }

        public string PoolName
        {
            get { return poolName; }
            private set { poolName = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            private set { position = value; }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            private set { rotation = value; }
        }

        public Vector3 RelativeScale
        {
            get { return relativeScale; }
            private set { relativeScale = value; }
        }

        /// <summary>
        /// 当前状态时可见还是不可见。
        /// </summary>
        public bool IsVisible { get; protected set; }

        public ASceneElement SetPrefab(Transform prefab, string poolName)
        {
            Prefab = prefab;
            PoolName = string.IsNullOrEmpty(poolName) ? Prefab.name : poolName;
            return this;
        }

        /// <summary>
        /// 设置实体出生的位置。
        /// 注意这里并不直接设置实体位置，实体位置在实体被真正创建时设置。
        /// </summary>
        /// <param name="position">位置矢量。</param>
        /// <returns>实体。</returns>
        public ASceneElement SetPosition(Vector3 position)
        {
            Position = position;
            return this;
        }

        /// <summary>
        /// 设置实体出生的旋转。
        /// 注意这里并不直接设置实体旋转，实体旋转在实体被真正创建时设置。
        /// </summary>
        /// <param name="rotation">旋转四元数。</param>
        /// <returns>实体。</returns>
        public ASceneElement SetRotation(Quaternion rotation)
        {
            Rotation = rotation;
            return this;
        }

        /// <summary>
        /// 设置实体出生的相对缩放。
        /// 注意这里并不直接设置实体缩放，实体缩放在实体被真正创建时设置。
        /// </summary>
        /// <param name="scale">缩放矢量。</param>
        /// <returns>实体。</returns>
        public ASceneElement SetRelativeScale(Vector3 scale)
        {
            RelativeScale = scale;
            return this;
        }

        /// <summary>
        /// 重置当前实例。
        /// </summary>
        public void Reset()
        {
            prefab = null;
            poolName = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            relativeScale = Vector3.one;
            IsVisible = false;

            OnReset();
        }

        /// <summary>
        /// 元素变得可见时回调。
        /// </summary>
        abstract public void OnBecameVisible(ALayerCamera layerCamera);

        /// <summary>
        /// 元素变得不可见时回调。
        /// </summary>
        abstract public void OnBecameInvisible(ALayerCamera layerCamera);

        /// <summary>
        /// 重置与特定派生类相关的成员。
        /// </summary>
        abstract protected void OnReset();

        /// <summary>
        /// 将 GameObject 的 layer 设置为 层摄像机可见的第一个 layer。
        /// 如果层摄像机没有可见层则保留 GameObject 原先设置。
        /// </summary>
        /// <param name="go">GameObject。</param>
        /// <param name="layerCamera">层摄像机。</param>
        protected void SetToCameraLayer(GameObject go, ALayerCamera layerCamera)
        {
            LayerMask layerMask = layerCamera.Camera.cullingMask;
            int layer = layerMask.GetFirstLayer();

            if (layer != -1)
            {
                go.layer = layer;
            }
        }
    }
}