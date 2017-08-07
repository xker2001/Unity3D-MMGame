using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 一维场景元素可见性管理器。
    /// 仅从一个维度（水平或垂直）检查元素的可见性。
    /// </summary>
    abstract public class AOneDSceneCulling : ASceneCulling
    {
        /// <summary>
        /// 场景是水平滚动还是垂直滚动。
        /// </summary>
        [SerializeField]
        protected ScrollAxis scrollAxis;

        /// <summary>
        /// 透视摄像机计算可视区域时射线的长度。
        /// </summary>
        [SerializeField]
        protected float cameraRayLength = 10;

        /// <summary>
        /// 安全可视区域扩展比例。
        /// 安全可视区域 = 屏幕区域 * safeRangeInPercent
        /// </summary>
        [Tooltip("安全可视区域 = 屏幕区域 * 扩展比例。")]
        [SerializeField]
        private float safeVisibleAreaPercent = 1.01f;

        /// <summary>
        /// 获取当前安全可视范围在卷轴维度上的长度（世界单位）。
        /// </summary>
        /// <returns>长度值。</returns>
        protected float GetSafeVisibleAreaLength()
        {
            Vector2 screenUnitSize = layerCamera.GetScreenUnitSize(cameraRayLength);
            float len = scrollAxis == ScrollAxis.Horizontal ? screenUnitSize.x : screenUnitSize.y;
            return len * safeVisibleAreaPercent;
        }
    }
}