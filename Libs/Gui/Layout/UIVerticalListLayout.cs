using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 垂直排列布局器。
    /// 
    /// 说明：
    /// - Layout 高度可以适配内容，宽度固定。
    /// - Layout 锚定方式任意，会被设置为左上角。
    /// - Layout pivot 任意，根据缩放时的锚定点需要进行选择。
    /// - Item 可以被拉伸到布局器宽度，此时 Layout 宽度不变。
    /// - Item 的高度不变。
    /// - Item 锚定方式任意，会被 Row 设置为左上角。
    /// - Item 的 pivot 任意。
    ///
    /// 使用场景范例：好友名单列表。
    /// </summary>
    public class UIVerticalListLayout : AUILayout, IUIScrolltoableLayout, IUISizeFitableLayout
    {
        [SerializeField]
        private float topPadding;

        [SerializeField]
        private float bottomPadding;

        [SerializeField]
        private float leftPadding;

        [SerializeField]
        private float rightPadding;

        [SerializeField]
        private float rowSpace;

        [SerializeField]
        private bool expandWidth = true;

        /// <summary>
        /// 子控件的缩放值。
        /// 设置此参数的原始原因是因为即使使用 SetParent(false)，
        /// 控件的 localScale 也会被改变。可能是 Unity 的 bug。
        /// </summary>
        [Tooltip("子控件的 localScale 值。")]
        [SerializeField]
        private Vector3 itemScale = new Vector3(1, 1, 1);

        private readonly IList<RectTransform> items = new List<RectTransform>();

        protected override void Start()
        {
            base.Start();
            AutoLayout();
        }

        void OnTransformChildrenChanged()
        {
            AutoLayout();
        }

        public override void Layout()
        {
            UIHelper.FixedlyChangeAnchors(rectTransform, new Vector2(0, 1), new Vector2(0, 1));
            items.Clear();
            float startPos = -topPadding;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                var item = rectTransform.GetChild(i) as RectTransform;
                Assert.IsNotNull(item);
                // 设置缩放（确认 Unity bug 已修复后可移除）
                item.localScale = itemScale;

                // 设置锚定方式
                UIHelper.FixedlyChangeAnchors(item, new Vector2(0, 1), new Vector2(0, 1));

                // 设置大小
                if (expandWidth)
                {
                    item.sizeDelta = new Vector2(rectTransform.rect.width - leftPadding - rightPadding,
                                                 item.rect.height);
                }

                // 设置位置
                item.anchoredPosition = new Vector2(item.pivot.x * item.rect.width + leftPadding,
                                                    startPos - item.rect.height * (1 - item.pivot.y));

                startPos = startPos - item.rect.height - rowSpace;
                items.Add(item);
            }

            CalculateFitableSize();
        }

        //--------------------------------------------------
        // IUIScrolltoableLayout
        //--------------------------------------------------

        public Vector2 GetItemCenterPosition(int index)
        {
            Assert.IsTrue(index >= 0 && index < items.Count,
                          string.Format("index: {0} (>=0 && < {1})", index, items.Count));

            Vector2 localPos = items[index].localPosition;
            Vector2 pivot = items[index].pivot;
            Rect rect = items[index].rect;

            return new Vector2(
                localPos.x + (1f - 2 * pivot.x) * rect.width * 0.5f - rectTransform.rect.xMin,
                localPos.y + (1f - 2 * pivot.y) * rect.height * 0.5f - rectTransform.rect.yMax);
        }

        public Vector2 GetItemSize(int index)
        {
            return items[index].rect.size;
        }

        public Vector2 GetLayoutSize()
        {
            return rectTransform.rect.size;
        }

        public int ItemCount
        {
            get { return items.Count; }
        }

        //--------------------------------------------------
        // IUISizeFitableLayout
        //--------------------------------------------------

        public event Action<Vector2> SizeChanged;
        public Vector2 SizeDelta { get; private set; }

        /// <summary>
        /// 计算适合内容的大小。
        /// </summary>
        private void CalculateFitableSize()
        {
            float sumItemHeight = 0;

            foreach (RectTransform item in items)
            {
                sumItemHeight += item.rect.size.y;
            }

            SizeDelta = new Vector2(
                rectTransform.sizeDelta.x,
                topPadding + bottomPadding + sumItemHeight + Mathf.Max(0, (items.Count - 1)) * rowSpace);

            if (SizeChanged != null)
            {
                SizeChanged(SizeDelta);
            }
        }
    }
}