using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 只支持单向移动、破坏性的、对场景元素进行排序的一维场景元素可见性管理器。
    /// 沿预期方向移动时后方不可见的场景元素会被对象池破坏性回收。
    /// </summary>
    public class UnidirectionalDestructiveOneDSortedSceneCulling : AOneDSortedSceneCulling
    {
        /// <summary>
        /// 预设的正确移动方向。
        /// </summary>
        [SerializeField]
        private RelativeDirection positiveDirection = RelativeDirection.Positive;

        protected override void UpdateCulling(ALayerCamera layerCamera)
        {
            if (elementCoordinates.Count == 0)
            {
                return;
            }

            // 获取可见的最近和最远场景元素的索引值
            int nearElementIndex;
            int farElementIndex;
            GetVisibleElementsRange(layerCamera, out nearElementIndex, out farElementIndex);

            // 设置可视范围内的元素为可见
            for (int i = nearElementIndex; i <= farElementIndex; i++)
            {
                if (!elements[i].IsVisible)
                {
                    elements[i].OnBecameVisible(layerCamera);
                }
            }

            // 回收身后的场景元素
            if (positiveDirection == RelativeDirection.Positive)
            {
                int index = 0;

                while (index < nearElementIndex)
                {
                    SceneElementPool.Delete(elements[0]);
                    elements.RemoveAt(0);
                    elementCoordinates.RemoveAt(0);
                    index += 1;
                }
            }
            else
            {
                int index = elements.Count - 1;

                while (index > farElementIndex)
                {
                    SceneElementPool.Delete(elements[index]);
                    elements.RemoveAt(index);
                    elementCoordinates.RemoveAt(index);
                    index -= 1;
                }
            }
        }
    }
}