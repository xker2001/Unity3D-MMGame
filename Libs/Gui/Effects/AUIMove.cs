using UnityEngine;

namespace MMGame.UI
{
    public enum OutScreenAnchor
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight,
        Parent
    }

    abstract public class AUIMove : AUIEffect
    {
        /// <summary>
        /// 是否使用 Start() 中记录的位置作为标准位置。
        /// 如果使用，则则 Start() 中记录标准位置，直到使用 RecordPosition() 方法进行更新。
        /// 如果不使用，则：
        /// 1. 使用第一次播放该特效时所在的位置作为标准位置，直到使用 RecordPosition() 方法进行更新。
        /// 2. 使用脚本调用 RecordPosition()，主动设置标准位置。
        /// </summary>
        [SerializeField]
        private bool useAwakePosition = true;

        private Vector3? originalPosition; // 控件的原始位置，只计算一次

        private const float offsetPercent = 0.1f; // 在屏幕外取位的百分比
        private float leftCoord; // 屏幕左侧外 x 坐标
        private float rightCoord; // 屏幕右侧外 x 坐标
        private float upperCoord; // 屏幕上部外 y 坐标
        private float lowerCoord; // 屏幕底部外 y 坐标
        private bool isOutScreenPosCalculated;

        protected override void Awake()
        {
            base.Awake();

            if (useAwakePosition)
            {
                RecordPosition();
            }
        }

        /// <summary>
        /// 记录控件的初始位置。
        /// </summary>
        public void RecordPosition()
        {
            originalPosition = rectTransform.position;
        }

        /// <summary>
        /// 获取控件原始位置，优先取已存在的值。
        /// </summary>
        /// <returns>位置坐标。</returns>
        protected Vector3 GetOriginalPosition()
        {
            if (!originalPosition.HasValue)
            {
                originalPosition = rectTransform.position;
            }

            return originalPosition.Value;
        }

        /// <summary>
        /// 根据锚点类型计算屏幕外位置的坐标。
        /// </summary>
        /// <param name="anchor">锚点类型。</param>
        /// <returns>位置坐标。</returns>
        protected Vector3 GetOutScreenPosition(OutScreenAnchor anchor)
        {
            if (anchor == OutScreenAnchor.Parent)
            {
                return transform.parent.position;
            }

            if (!isOutScreenPosCalculated)
            {
                CalculateOutScreenCoordinates();
                isOutScreenPosCalculated = true;
            }

            Vector3 position = rectTransform.position;

            switch (anchor)
            {
                case OutScreenAnchor.UpperLeft:
                    return new Vector3(leftCoord, upperCoord, position.z);

                case OutScreenAnchor.UpperCenter:
                    return new Vector3(position.x, upperCoord, position.z);

                case OutScreenAnchor.UpperRight:
                    return new Vector3(rightCoord, upperCoord, position.z);

                case OutScreenAnchor.MiddleLeft:
                    return new Vector3(leftCoord, position.y, position.z);

                case OutScreenAnchor.MiddleRight:
                    return new Vector3(rightCoord, position.y, position.z);

                case OutScreenAnchor.LowerLeft:
                    return new Vector3(leftCoord, lowerCoord, position.z);

                case OutScreenAnchor.LowerCenter:
                    return new Vector3(position.x, lowerCoord, position.z);

                case OutScreenAnchor.LowerRight:
                    return new Vector3(rightCoord, lowerCoord, position.z);

                default:
                    return new Vector3(position.x, upperCoord, position.z);
            }
        }

        /// <summary>
        /// 预先计算屏幕四边外的坐标。
        /// </summary>
        private void CalculateOutScreenCoordinates()
        {
            Vector3 scale = transform.lossyScale;

            // 屏幕宽高
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 控件宽高
            float rectWidth = rectTransform.rect.width * scale.x;
            float rectHeight = rectTransform.rect.height * scale.y;

            // Rect 原点（左上角）相对中心的坐标
            float rectTLPosX = rectTransform.rect.position.x * scale.x;
            float rectTLPosY = rectTransform.rect.position.y * scale.y;

            // 屏幕四边外坐标
            leftCoord = -(rectTLPosX + rectWidth) - screenWidth * offsetPercent;
            rightCoord = -rectTLPosX + screenWidth * (1 + offsetPercent);
            upperCoord = -rectTLPosY + screenHeight * (1 + offsetPercent);
            lowerCoord = -(rectHeight + rectTLPosY) - screenHeight * offsetPercent;
        }
    }
}