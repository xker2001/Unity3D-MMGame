using System.Collections.Generic;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 支持双向移动、对场景元素进行排序的一维场景元素可见性管理器。
    /// 场景元素不会被销毁。
    /// </summary>
    public class BidirectionalOneDSortedSceneCulling : AOneDSortedSceneCulling
    {
        // 上一帧可见元素集合
        private HashSet<ASceneElement> visibleElements = new HashSet<ASceneElement>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            visibleElements = null;
        }

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
                if (elements[i].IsVisible)
                {
                    visibleElements.Remove(elements[i]);
                }
                else
                {
                    elements[i].OnBecameVisible(layerCamera);
                }
            }

            // 设置之前可见当前不可见的场景元素为不可见
            foreach (ASceneElement element in visibleElements)
            {
                element.OnBecameInvisible(layerCamera);
            }

            // 重新填充当前可见元素集合
            visibleElements.Clear();

            for (int i = nearElementIndex; i <= farElementIndex; i++)
            {
                visibleElements.Add(elements[i]);
            }
        }
    }
}