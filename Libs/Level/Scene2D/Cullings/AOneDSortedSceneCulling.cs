using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 对场景元素进行排序的一维场景元素可见性管理器。
    /// </summary>
    abstract public class AOneDSortedSceneCulling : AOneDSceneCulling
    {
        //--------------------------------------------------
        // 不假设外部会按坐标顺序添加场景元素，因此 Culling 需
        // 自行保证元素按一维坐标排序，会牺牲一些性能。
        //--------------------------------------------------

        protected List<ASceneElement> elements = new List<ASceneElement>(); // 有序的场景元素集合
        protected List<float> elementCoordinates = new List<float>(); // 有序的一维坐标集合，方便查找

        virtual protected void OnDestroy()
        {
            Clear();
            elements = null;
            elementCoordinates = null;
        }

        public override void AddElement(ASceneElement element)
        {
            // 按屏幕坐标系方向组织顺序
            int index = elementCoordinates.AddSorted(GetOneDCoordinate(element.Position));
            elements.Insert(index, element);
        }

        public override void AddElements(IList<ASceneElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                AddElement(elements[i]);
            }
        }

        /// <summary>
        /// 回收所有场景元素到对象池并清空集合。
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                SceneElementPool.Delete(elements[i]);
            }

            elements.Clear();
            elementCoordinates.Clear();
        }

        /// <summary>
        /// 获取当前安全可视范围内（及向外扩展一个元素）元素最大索引和最小索引。
        /// </summary>
        /// <param name="layerCamera">层摄像机。</param>
        /// <param name="nearElementIndex">近端场景元素索引。</param>
        /// <param name="farElementIndex">远端场景元素索引。</param>
        protected void GetVisibleElementsRange(ALayerCamera layerCamera,
                                               out int nearElementIndex,
                                               out int farElementIndex)
        {
            float minCoordinate;
            float maxCoordinate;

            GetVisibleRange(layerCamera, out minCoordinate, out maxCoordinate);
            nearElementIndex = GetNearElementIndex(minCoordinate, elementCoordinates);
            farElementIndex = GetFarElementIndex(maxCoordinate, elementCoordinates);
        }

        /// <summary>
        /// 获取在卷轴维度上的坐标。
        /// </summary>
        /// <param name="position">世界坐标。</param>
        /// <returns>一维坐标。</returns>
        private float GetOneDCoordinate(Vector3 position)
        {
            return scrollAxis == ScrollAxis.Horizontal ? position.x : position.y;
        }

        /// <summary>
        /// 获取当前安全可视范围。
        /// </summary>
        /// <param name="layerCamera">层摄像机。</param>
        /// <param name="minCoordinate">安全可视范围的最小坐标。</param>
        /// <param name="maxCoordinate">安全可视范围的最大坐标。</param>
        private void GetVisibleRange(ALayerCamera layerCamera, out float minCoordinate, out float maxCoordinate)
        {
            float halfLength = GetSafeVisibleAreaLength() * 0.5f;
            float camPosition = GetOneDCoordinate(layerCamera.Position);

            minCoordinate = camPosition - halfLength;
            maxCoordinate = camPosition + halfLength;
        }

        /// <summary>
        /// 寻找指定坐标的场景元素的索引，如果没有则取更近的一个元素的索引。
        /// </summary>
        /// <param name="coordinate">一维坐标。</param>
        /// <param name="elementDistances">元素坐标有序列表。</param>
        /// <returns>索引值。</returns>
        private int GetNearElementIndex(float coordinate, List<float> elementDistances)
        {
            int index = elementDistances.BinarySearch(coordinate);

            if (index < 0)
            {
                index = ~index; // 最近的更大数的 index

                if (index > 0)
                {
                    index -= 1;
                }
            }

            return index;
        }

        /// <summary>
        /// 寻找指定坐标的场景元素的索引，如果没有则取更远的一个元素的索引。
        /// </summary>
        /// <param name="coordinate">位置。</param>
        /// <param name="elementDistances">元素位置有序列表。</param>
        /// <returns>索引值。</returns>
        private int GetFarElementIndex(float coordinate, List<float> elementDistances)
        {
            int index = elementDistances.BinarySearch(coordinate);

            if (index < 0)
            {
                index = ~index;

                if (index > elementDistances.Count - 1)
                {
                    index = elementDistances.Count - 1;
                }
            }

            return index;
        }
    }
}