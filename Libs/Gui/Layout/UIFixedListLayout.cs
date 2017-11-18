using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 预置子控件的垂直循环布局器，子控件数量和大小固定，运行后不可增删。
    /// 
    /// 说明：
    /// - Layout 高度和宽度可以适配内容（配合 UIFitLayoutSize 组件）。
    /// - 所有 item 必须事先放置，在运行时通过 enable 和 disable 来控制。
    /// - 某个 item 被隐藏后，其后的 item 按序补进一格。
    /// - 如果设置 reorder，item 被重新激活后会移动到列表的末尾。
    /// - Item 的 pivot 任意。
    /// - Item 的大小不变。
    ///
    /// 使用场景范例：Buffer 显示区。
    /// </summary>
    public class UIFixedListLayout : AUILayout, IUISizeFitableLayout
    {
        private enum Direction
        {
            Vertical,
            Horizontal
        }

        private enum HorizontalAlign
        {
            Left,
            Center, // 此时 leftPadding 和 rightPadding 无效
            Right
        }

        private enum VerticalAlign
        {
            Top,
            Center, // 此时 topPadding 和 bottomPadding 无效
            Bottom
        }

        [Tooltip("重新激活的元素是否移至列表末尾。")]
        [SerializeField]
        private bool reorder;

        [SerializeField]
        private Direction direction = Direction.Vertical;

        [SerializeField]
        private HorizontalAlign horizontalAlign = HorizontalAlign.Left;

        [SerializeField]
        private VerticalAlign verticalAlign = VerticalAlign.Top;

        [SerializeField]
        private float topPadding;

        [SerializeField]
        private float bottomPadding;

        [SerializeField]
        private float leftPadding;

        [SerializeField]
        private float rightPadding;

        [SerializeField]
        private float itemSpace;

        private RectTransform[] elements;
        private readonly Dictionary<RectTransform, bool> elementStats = new Dictionary<RectTransform, bool>();
        private int activeCount; // 激活的子控件数量

        protected override void Awake()
        {
            base.Awake();
            elements = new RectTransform[rectTransform.childCount];

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                elements[i] = rectTransform.GetChild(i) as RectTransform;
                Assert.IsNotNull(elements[i]);
                ChangeAnchors(elements[i]);
                elementStats.Add(elements[i], false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AutoLayout();
        }

        private void Update()
        {
            Layout();
        }

        public override void Layout()
        {
            bool isDirty = RearrangeElements();

            if (!isDirty)
            {
                return;
            }

            activeCount = GetActiveCount();
            SetElementPositions();
            CalculateFitableSize();
        }

        public event Action<float, float> SizeChanged;
        public Vector2 Size { get; private set; }

        private bool RearrangeElements()
        {
            bool isDirty = false;

            // 在层级中重排子控件顺序
            for (int i = 0; i < elements.Length; i++)
            {
                // 新激活的子控件（放到末尾）
                if (elements[i].gameObject.activeSelf && elementStats[elements[i]] == false)
                {
                    if (reorder)
                    {
                        elements[i].SetAsLastSibling();
                    }

                    elementStats[elements[i]] = true;
                    isDirty = true;
                }
                // 新禁用的子控件
                else if (elements[i].gameObject.activeSelf == false && elementStats[elements[i]])
                {
                    elementStats[elements[i]] = false;
                    isDirty = true;
                }
            }

            return isDirty;
        }

        private int GetActiveCount()
        {
            int count = 0;

            for (var i = 0; i < elements.Length; i++)
            {
                if (elements[i].gameObject.activeSelf)
                {
                    count += 1;
                }
            }

            return count;
        }

        private void SetElementPositions()
        {
            int index = 0;

            for (var i = 0; i < elements.Length; i++)
            {
                if (elements[i].gameObject.activeSelf)
                {
                    Rect elRect = elements[i].rect;
                    Vector2 elPivot = elements[i].pivot;
                    SetElementAnchoredPosition(elements[i], elPivot, elRect, index);
                    index += 1;
                }
            }
        }

        private void CalculateFitableSize()
        {
            float elWidth = 0;
            float elHeight = 0;

            if (activeCount > 0)
            {
                elWidth = elements[0].rect.width;
                elHeight = elements[0].rect.height;
            }

            float sizeX = direction == Direction.Vertical
                              ? elWidth + leftPadding + rightPadding
                              : leftPadding + rightPadding + elWidth * activeCount
                                + Mathf.Max(0, activeCount - 1) * itemSpace;

            float sizeY = direction == Direction.Vertical
                              ? topPadding + bottomPadding + elHeight * activeCount
                                + Mathf.Max(0, activeCount - 1) * itemSpace
                              : elHeight + topPadding + bottomPadding;

            Size = new Vector2(sizeX, sizeY);

            if (SizeChanged != null)
            {
                SizeChanged(Size.x, Size.y);
            }
        }

        private void ChangeAnchors(RectTransform rectTransform)
        {
            float anchorX = horizontalAlign == HorizontalAlign.Left
                                ? 0
                                : horizontalAlign == HorizontalAlign.Right
                                    ? 1
                                    : 0.5f;

            float anchorY = verticalAlign == VerticalAlign.Top
                                ? 1
                                : verticalAlign == VerticalAlign.Bottom
                                    ? 0
                                    : 0.5f;

            UIUtility.FixedlyChangeAnchors(rectTransform, new Vector2(anchorX, anchorY), new Vector2(anchorX, anchorY));
        }

        private void SetElementAnchoredPosition(RectTransform element, Vector2 elPivot, Rect elRect, int index)
        {
            float w = elRect.width;
            float h = elRect.height;

            float posX = direction == Direction.Vertical
                             ? horizontalAlign == HorizontalAlign.Left
                                   ? elPivot.x * w + leftPadding
                                   : horizontalAlign == HorizontalAlign.Right
                                       ? -(1 - elPivot.x) * w - rightPadding
                                       : (elPivot.x - 0.5f) * w
                             : horizontalAlign == HorizontalAlign.Left
                                 ? elPivot.x * w + leftPadding + (itemSpace + w) * index
                                 : horizontalAlign == HorizontalAlign.Right
                                     ? -(1 - elPivot.x) * w - rightPadding - (itemSpace + w) * (activeCount - index - 1)
                                     : (elPivot.x - 0.5f) * w
                                       - (itemSpace + w) * (activeCount * 0.5f + 0.5f - index - 1);

            float posY = direction == Direction.Vertical
                             ? verticalAlign == VerticalAlign.Top
                                   ? -(1 - elPivot.y) * h - topPadding - (itemSpace + h) * index
                                   : verticalAlign == VerticalAlign.Bottom
                                       ? elPivot.y * h + bottomPadding + (itemSpace + h) * (activeCount - index - 1)
                                       : (elPivot.y - 0.5f) * h
                                         + (itemSpace + h) * (activeCount * 0.5f + 0.5f - index - 1)
                             : verticalAlign == VerticalAlign.Top
                                 ? -(1 - elPivot.y) * h - topPadding
                                 : verticalAlign == VerticalAlign.Bottom
                                     ? elPivot.y * h + bottomPadding
                                     : (elPivot.y - 0.5f) * h;

            element.anchoredPosition = new Vector2(posX, posY);
        }
    }
}