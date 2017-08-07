using UnityEngine;

namespace MMGame
{
    public static class SpriteUtility
    {
        /// <summary>
        /// 获取 Sprite 实际大小（unit）。
        /// </summary>
        /// <param name="renderer">SpriteRenderer。</param>
        /// <returns>大小（unit）。</returns>
        public static Vector2 GetRealSize(SpriteRenderer renderer)
        {
            Bounds bounds = renderer.sprite.bounds;
            Vector3 localScale = renderer.transform.localScale;
            return new Vector2(bounds.size.x * localScale.x, bounds.size.y * localScale.y);
        }

        /// <summary>
        /// 设置 Sprite 实际大小（unit）。
        /// </summary>
        /// <param name="renderer">SpriteRenderer。</param>
        /// <param name="width">宽度（unit）。</param>
        /// <param name="height">长度（unit）。</param>
        public static void SetRealSize(SpriteRenderer renderer, float width, float height)
        {
            Bounds bounds = renderer.sprite.bounds;
            Transform xform = renderer.transform;
            Vector3 localScale = xform.localScale;
            float scaleX = width / (bounds.size.x * localScale.x);
            float scaleY = height / (bounds.size.y * localScale.y);
            xform.localScale = new Vector3(localScale.x * scaleX,
                                           localScale.y * scaleY,
                                           localScale.z);
        }
    }
}