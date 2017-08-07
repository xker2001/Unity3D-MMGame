using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 预置子控件的垂直循环布局器。
    /// 
    /// 说明：
    /// - Layout 高度和宽度可以适配内容。
    /// - Layout 锚定方式任意，会被设置为左上角。
    /// - Layout pivot 任意，根据缩放时的锚定点需要进行选择。
    /// - 所有 item 必须事先放置，在运行时通过 enable 和 disable 来控制。
    /// - 某个 item 被隐藏后，其后的 item 按序补进一格。
    /// - 如果设置 reorder，item 被重新激活后会移动到列表的末尾。
    /// - Item 的锚定方式将被设置为左上角。
    /// - Item 的 pivot 任意。
    /// - Item 的大小不变。
    /// - 目前要求 item 高度相同。
    ///
    /// 使用场景范例：Buffer 显示区。
    /// </summary>
    public class UIVerticalLoopLayout : AUILayout, IUISizeFitableLayout
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

        [Tooltip("重新激活的元素是否移至列表末尾。")]
        [SerializeField]
        private bool reorder;

        private RectTransform[] elements;
        private readonly Dictionary<RectTransform, bool> elementStats = new Dictionary<RectTransform, bool>();

        protected override void Awake()
        {
            base.Awake();
            elements = new RectTransform[rectTransform.childCount];

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                elements[i] = rectTransform.GetChild(i) as RectTransform;
                Assert.IsNotNull(elements[i]);
                UIHelper.FixedlyChangeAnchors(elements[i], new Vector2(0, 1), new Vector2(0, 1));
                elementStats.Add(elements[i], false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AutoLayout();
        }

        void Update()
        {
            Layout();
        }

        public override void Layout()
        {
            UIHelper.FixedlyChangeAnchors(rectTransform, new Vector2(0, 1), new Vector2(0, 1));
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

            if (!isDirty)
            {
                return;
            }

            // 设置子控件大小和位置
            int index = 0;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                var element = rectTransform.GetChild(i) as RectTransform;
                Assert.IsNotNull(element);

                if (element.gameObject.activeSelf)
                {
                    Rect elRect = element.rect;
                    Vector2 elPivot = element.pivot;

                    element.anchoredPosition =
                        new Vector2(elPivot.x * elRect.width + leftPadding,
                                    -topPadding - elRect.height * (1 - elPivot.y) - (rowSpace + elRect.height) * index);

                    index += 1;
                }
            }

            CalculateFitableSize();
        }

        //--------------------------------------------------
        // IUISizeFitableLayout
        //--------------------------------------------------

        public event Action<Vector2> SizeChanged;
        public Vector2 SizeDelta { get; private set; }

        private void CalculateFitableSize()
        {
            int count = 0;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                var element = rectTransform.GetChild(i) as RectTransform;
                Assert.IsNotNull(element);

                if (element.gameObject.activeSelf)
                {
                    count += 1;
                }
            }

            float elWidth = 0;
            float elHeight = 0;

            if (count > 0)
            {
                elWidth = elements[0].rect.width;
                elHeight = elements[0].rect.height;
            }

            SizeDelta = new Vector2(
                elWidth + leftPadding + rightPadding,
                topPadding + bottomPadding + elHeight * count + Mathf.Max(0, (count - 1)) * rowSpace);

            if (SizeChanged != null)
            {
                SizeChanged(SizeDelta);
            }
        }
    }
}