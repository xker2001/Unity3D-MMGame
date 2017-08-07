using UnityEngine;

namespace MMGame.Scene2D
{
    public class RandomDistanceSpawnExecutor : AOneDDistanceSpawnExecutor
    {
        /// <summary>
        /// 最小间隔距离。
        /// </summary>
        [SerializeField]
        private float minDistance = 5;

        /// <summary>
        /// 最大间隔距离。
        /// </summary>
        [SerializeField]
        private float maxDistance = 10;

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

        protected override float JoinElement(ASceneElement element, float distance, bool isPositive)
        {
            distance += GetNextInterval() * (isPositive ? 1 : -1);

            if (ScrollAxis == ScrollAxis.Horizontal)
            {
                element.SetPosition(new Vector3(LayerCamera.StartPosition.x + distance, 0, 0));
            }
            else
            {
                element.SetPosition(new Vector3(0, LayerCamera.StartPosition.x + distance, 0));
            }

            return distance;
        }

        protected override AElementProvider GetProvider(float distance, bool isPositive)
        {
            return isPositive ? positiveElementProvider : negativeElementProvider;
        }

        /// <summary>
        /// 获取下一个间隔距离。
        /// </summary>
        /// <returns></returns>
        private float GetNextInterval()
        {
            if (Mathf.Approximately(minDistance, maxDistance))
            {
                return minDistance;
            }
            else

            {
                return Random.Range(minDistance, maxDistance);
            }
        }
    }
}