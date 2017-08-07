using UnityEngine;

namespace MMGame.UI
{
    public static class UILayoutHelper
    {
        /// <summary>
        /// 获取 layout 左上角到 viewport 左上角的向量（通常 x 正值，y 负值）
        /// </summary>
        public static Vector2 GetLayoutLT2ViewportLT(RectTransform layout, RectTransform viewport)
        {
            var layoutLocalPos = (Vector2) layout.localPosition;
            return new Vector2(viewport.rect.xMin - layout.rect.xMin - layoutLocalPos.x,
                               viewport.rect.yMax - layout.rect.yMax - layoutLocalPos.y);
        }

        /// <summary>
        /// 获取 layout 左上角到 viewport 中心的向量（通常 x 正值，y 负值）
        /// </summary>
        public static Vector2 GetLayoutLT2ViewportCenter(RectTransform layout, RectTransform viewport)
        {
            var layoutLocalPos = (Vector2) layout.localPosition;
            return new Vector2(
                viewport.rect.xMin - layout.rect.xMin - layoutLocalPos.x + viewport.rect.width * 0.5f,
                viewport.rect.yMax - layout.rect.yMax - layoutLocalPos.y - viewport.rect.height * 0.5f);
        }

        /// <summary>
        /// 获取 layout item 中心到 viewport 中心的向量。
        /// </summary>
        /// <param name="layout">Layout 的 RectTransform。</param>
        /// <param name="viewport">Viewport 的 RectTransform。</param>
        /// <param name="scrolltoableLayout">Layout 的 IUIScrolltoableLayout 接口。</param>
        /// <param name="itemIndex">Item 索引。</param>
        /// <returns>位置差值向量。</returns>
        public static Vector2 GetItemCenterToViewportCenter(RectTransform layout, RectTransform viewport,
                                                            IUIScrolltoableLayout scrolltoableLayout,
                                                            int itemIndex)
        {
            Vector2 pageCenterPosition = scrolltoableLayout.GetItemCenterPosition(itemIndex);
            return GetLayoutLT2ViewportCenter(layout, viewport) - pageCenterPosition;
        }
    }
}