using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    public enum OutScreenAnchor
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
        Parent
    }

    abstract public class AUIMove : AUIEffect
    {
        /// <summary>
        /// 是否使用 Awake() 中记录的位置作为标准位置。
        /// 如果使用，则在 Awake() 中记录标准位置，直到使用 RecordPosition() 方法进行更新。
        /// 如果不使用，则：
        /// 1. 使用第一次播放该特效时所在的位置作为标准位置，直到使用 RecordPosition() 方法进行更新。
        /// 2. 使用脚本调用 RecordPosition()，主动设置标准位置。
        /// </summary>
        [Tooltip("是否在 Start() 中记录控件的初始位置。如果不记录则使用播放时所在的位置，直到外部使用 RecordPosition() 方法进行更新。")]
        [ShowIf("ShowUseAwakePosition")]
        [SerializeField]
        private bool useStartPosition = true;

        [ShowIf("ShowOffsetPercent")]
        [SerializeField]
        private float offsetPercent = 0.1f; // 在屏幕外取位的百分比

        private Vector3? originalPosition; // 控件的原始位置，只计算一次

        private float leftCoord; // 屏幕左侧外 x 坐标
        private float rightCoord; // 屏幕右侧外 x 坐标
        private float upperCoord; // 屏幕上部外 y 坐标
        private float lowerCoord; // 屏幕底部外 y 坐标
        private bool isOutScreenPosCalculated;
        protected RectTransform rectTransform;

        virtual protected bool ShowUseAwakePosition()
        {
            return true;
        }

        virtual protected bool ShowOffsetPercent()
        {
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = Target.GetComponent<RectTransform>();
        }

        protected override void Start()
        {
            if (useStartPosition)
            {
                RecordPosition();
            }

            base.Start();
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
        /// <param name="relativePosition">
        /// 关联坐标。当移向屏幕外时应当传入起点坐标，当移向屏幕内时应传入终点坐标。
        /// </param>
        /// <returns>位置坐标。</returns>
        protected Vector3 GetOutScreenPosition(OutScreenAnchor anchor, Vector3 relativePosition)
        {
            if (anchor == OutScreenAnchor.Parent)
            {
                return rectTransform.parent.position;
            }

            if (!isOutScreenPosCalculated)
            {
                CalculateOutScreenCoordinates();
                isOutScreenPosCalculated = true;
            }

            switch (anchor)
            {
                case OutScreenAnchor.TopLeft:
                    return new Vector3(leftCoord, upperCoord, relativePosition.z);

                case OutScreenAnchor.Top:
                    return new Vector3(relativePosition.x, upperCoord, relativePosition.z);

                case OutScreenAnchor.TopRight:
                    return new Vector3(rightCoord, upperCoord, relativePosition.z);

                case OutScreenAnchor.Left:
                    return new Vector3(leftCoord, relativePosition.y, relativePosition.z);

                case OutScreenAnchor.Right:
                    return new Vector3(rightCoord, relativePosition.y, relativePosition.z);

                case OutScreenAnchor.BottomLeft:
                    return new Vector3(leftCoord, lowerCoord, relativePosition.z);

                case OutScreenAnchor.Bottom:
                    return new Vector3(relativePosition.x, lowerCoord, relativePosition.z);

                case OutScreenAnchor.BottomRight:
                    return new Vector3(rightCoord, lowerCoord, relativePosition.z);

                default:
                    return new Vector3(relativePosition.x, upperCoord, relativePosition.z);
            }
        }

        /// <summary>
        /// 预先计算屏幕四边外的坐标。
        /// </summary>
        private void CalculateOutScreenCoordinates()
        {
            Vector3 scale = rectTransform.lossyScale;

            // 屏幕宽高（世界单位）
            Canvas canvas = Target.GetComponentInParent<Canvas>();
            Assert.IsNotNull(canvas);

            float screenWidth;
            float screenHeight;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Overlay 模式下屏幕大小的 unit 数值和像素数值相等
                screenWidth = Screen.width;
                screenHeight = Screen.height;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                screenHeight = canvas.worldCamera.orthographicSize * 2f;
                screenWidth = screenHeight * canvas.worldCamera.aspect;
            }
            else
            {
                throw new Exception("Do not support RenderMode.WorldSpace.");
            }

            // 控件宽高
            float rectWidth = rectTransform.rect.width * scale.x;
            float rectHeight = rectTransform.rect.height * scale.y;

            // Rect 原点（左上角）相对中心的坐标
            float rectTLPosX = rectTransform.rect.position.x * scale.x;
            float rectTLPosY = rectTransform.rect.position.y * scale.y;

            // 计算屏幕四边外坐标

            // Canvas 左下角在原点
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                leftCoord = -(rectTLPosX + rectWidth) - screenWidth * offsetPercent;
                rightCoord = -rectTLPosX + screenWidth * (1 + offsetPercent);
                upperCoord = -rectTLPosY + screenHeight * (1 + offsetPercent);
                lowerCoord = -(rectHeight + rectTLPosY) - screenHeight * offsetPercent;
            }
            // Canvas 中心点在原点
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                leftCoord = -(rectTLPosX + rectWidth) - screenWidth * (0.5f + offsetPercent);
                rightCoord = -rectTLPosX + screenWidth * (0.5f + offsetPercent);
                upperCoord = -rectTLPosY + screenHeight * (0.5f + offsetPercent);
                lowerCoord = -(rectHeight + rectTLPosY) - screenHeight * (0.5f + offsetPercent);
            }
        }
    }
}