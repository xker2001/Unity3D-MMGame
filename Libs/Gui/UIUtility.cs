using UnityEngine;

namespace MMGame.UI
{
    public static class UIUtility
    {
        /// <summary>
        /// 修改 RectTransform 的 anchors，并保证 RectTransform 的位置和大小不变。
        /// </summary>
        /// <param name="rectTransform">UI 控件。</param>
        /// <param name="anchorMin">锚定方式 min 值。</param>
        /// <param name="anchorMax">锚定方式 max 值。</param>
        public static void FixedlyChangeAnchors(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
        {
            Vector3 localPos = rectTransform.localPosition;
            Vector2 size = rectTransform.rect.size;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            rectTransform.localPosition = localPos;
        }
    }
}