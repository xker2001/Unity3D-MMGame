using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    /// <summary>
    /// OneDDistanceSpawner 专用单步场景元素生成器。
    /// </summary>
    abstract public class AOneDDistanceSpawnExecutor : MonoBehaviour
    {
        /// <summary>
        /// 层摄像机，由外部设置。
        /// </summary>
        public ALayerCamera LayerCamera { get; set; }

        /// <summary>
        /// 水平卷轴还是垂直卷轴，由外部设置。
        /// </summary>
        public ScrollAxis ScrollAxis { get; set; }

        /// <summary>
        /// 沿正方形生成下一个场景元素。
        /// </summary>
        /// <param name="element">场景元素。</param>
        /// <param name="filledDistance">填充距离。</param>
        /// <returns>是否成功生成场景元素。</returns>
        public bool SpawnOnePositiveElement(out ASceneElement element, ref float filledDistance)
        {
            return SpawnOneElement(true, out element, ref filledDistance);
        }

        /// <summary>
        /// 沿反方形生成下一个场景元素。
        /// </summary>
        /// <param name="element">场景元素。</param>
        /// <param name="filledDistance">填充距离。</param>
        /// <returns>是否成功生成场景元素。</returns>
        public bool SpawnOneNegativeElement(out ASceneElement element, ref float filledDistance)
        {
            return SpawnOneElement(false, out element, ref filledDistance);
        }

        /// <summary>
        /// 创建一个场景元素。
        /// </summary>
        /// <param name="isPositive">是否向正方向创建。</param>
        /// <param name="element">创建的场景元素。</param>
        /// <param name="filledDistance">当前填充的距离。</param>
        /// <returns>是否成功创建。</returns>
        private bool SpawnOneElement(bool isPositive, out ASceneElement element, ref float filledDistance)
        {
            element = GetProvider(filledDistance, isPositive).GetNext();

            if (element == null)
            {
                return false;
            }

            Assert.IsNotNull(element.Prefab);
            filledDistance = JoinElement(element, filledDistance, isPositive);
            return true;
        }

        /// <summary>
        /// 将新场景元素添加到卷轴中。
        /// </summary>
        /// <param name="element">新场景元素。</param>
        /// <param name="distance">当前已填充的距离。</param>
        /// <param name="isPositive">是否正向填充。</param>
        abstract protected float JoinElement(ASceneElement element, float distance, bool isPositive);

        /// <summary>
        /// 获取场景元素提供器。
        /// </summary>
        /// <param name="distance">当前已填充的距离。</param>
        /// <param name="isPositive">是否正向填充。</param>
        /// <returns>场景元素提供器。</returns>
        abstract protected AElementProvider GetProvider(float distance, bool isPositive);
    }
}