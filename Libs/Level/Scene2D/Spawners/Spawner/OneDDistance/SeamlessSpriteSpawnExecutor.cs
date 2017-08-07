using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 无缝 Sprite 背景生成器。
    /// 1. 只支持 Sprite。
    /// 2. Sprite 的中心点应该在几何中心点。
    /// TODO: Sprite 的中心点可以在任意位置。
    /// </summary>
    public class SeamlessSpriteSpawnExecutor : AOneDDistanceSpawnExecutor
    {
        /// <summary>
        /// 正向场景元素提供器，元素 prefab 必须包含 SpriteRenderer。
        /// </summary>
        [SerializeField]
        private AElementProvider positiveElementProvider;

        /// <summary>
        /// 反向场景元素提供器，元素 prefab 必须包含 SpriteRenderer。
        /// </summary>
        [SerializeField]
        private AElementProvider negativeElementProvider;

        /// <summary>
        /// 字典，存放元素 Prefab 的 Sprite 长度。
        /// 注意，这里没有计入元素的 RelativeScale。
        /// </summary>
        private Dictionary<Transform, float> prefabLengthDic = new Dictionary<Transform, float>();

        protected override float JoinElement(ASceneElement element, float distance, bool isPositive)
        {
            float elementSpriteLen = GetElementSpriteLength(element);
            float sign = isPositive ? 1 : -1;

            Vector3 elementPos;

            if (ScrollAxis == ScrollAxis.Horizontal)
            {
                elementPos =
                    new Vector3(LayerCamera.StartPosition.x + distance + sign * elementSpriteLen * 0.5f,
                                0,
                                0);
            }
            else
            {
                elementPos =
                    new Vector3(0,
                                LayerCamera.StartPosition.y + distance + sign * elementSpriteLen * 0.5f,
                                0);
            }

            element.SetPosition(elementPos);
            return distance + sign * elementSpriteLen;
        }

        protected override AElementProvider GetProvider(float distance, bool isPositive)
        {
            return isPositive ? positiveElementProvider : negativeElementProvider;
        }

        /// <summary>
        /// 获取 prefab 中 Sprite 在卷轴方向上的长度。
        /// </summary>
        /// <param name="element"></param>
        /// <returns>长度，单位 unit。</returns>
        private float GetElementSpriteLength(ASceneElement element)
        {
            float prefabLen;
            Transform prefab = element.Prefab;

            if (!prefabLengthDic.TryGetValue(prefab, out prefabLen))
            {
                SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();

                Assert.IsNotNull(renderer);
                Assert.IsNotNull(renderer.sprite);

                Vector3 spriteSize = renderer.sprite.bounds.size;

                prefabLen = ScrollAxis == ScrollAxis.Horizontal
                                ? spriteSize.x * prefab.localScale.x
                                : spriteSize.y * prefab.localScale.y;

                prefabLengthDic.Add(prefab, prefabLen);
            }

            float relativeScale = ScrollAxis == ScrollAxis.Horizontal
                                      ? element.RelativeScale.x
                                      : element.RelativeScale.y;

            return prefabLen * relativeScale;
        }
    }
}