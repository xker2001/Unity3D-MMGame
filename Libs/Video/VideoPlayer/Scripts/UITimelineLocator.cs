using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MMGame.VideoPlayer
{
    /// <summary>
    /// 时间条控制器。
    /// 提供对在时间条上进行点击、拖动操作的响应。
    /// - 添加到 Slider 的 Background 控件。
    /// - 关闭 Fill 和 Handle 对输入事件的响应。
    /// </summary>
    public class UITimelineLocator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        /// <summary>
        /// 时间条左端的本地 x 坐标。
        /// </summary>
        private float startPosition;

        /// <summary>
        /// 时间条右端的本地 x 坐标。
        /// </summary>
        private float endPosition;

        private RectTransform rectTransform;

        public event Action<float> PointerDown;
        public event Action<float> Dragging;
        public event Action<float> PointerUp;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform);
        }

        private void Start()
        {
            startPosition = rectTransform.localPosition.x + rectTransform.rect.xMin;
            endPosition = rectTransform.localPosition.x + rectTransform.rect.xMax;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            float value = GetTimelineValue(eventData);

            if (PointerDown != null)
            {
                PointerDown(value);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float value = GetTimelineValue(eventData);

            if (PointerUp != null)
            {
                PointerUp(value);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            float value = GetTimelineValue(eventData);

            if (Dragging != null)
            {
                Dragging(value);
            }
        }

        /// <summary>
        /// 获取鼠标在时间条上的位置。
        /// </summary>
        /// <param name="eventData">鼠标事件。</param>
        /// <returns>时间条上的位置，0 ~ 1。</returns>
        private float GetTimelineValue(PointerEventData eventData)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out position);
            return (position.x - startPosition) / (endPosition - startPosition);
        }
    }
}