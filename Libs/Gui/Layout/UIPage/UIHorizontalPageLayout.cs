using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 水平滚动的页布局器。
    /// 
    /// 说明：
    /// - Layout 宽度可以适配内容，高度固定。
    /// - Layout 锚定方式任意，会被设置为左上角。
    /// - Layout pivot 任意。
    /// - Item 锚定方式任意，会被 Row 设置为左上角。
    /// - Item pivot 任意。
    /// - 所有 item 大小相同。
    /// </summary>
    public class UIHorizontalPageLayout : AUILayout, IUIPoolableItemLayout, IUISizeFitableLayout, IUIScrolltoableLayout
    {
        /// <summary>
        /// PageRect 物体。
        /// </summary>
        [SerializeField]
        private UIPageRect pageRect;

        /// <summary>
        /// ScrollRect.viewport 物体。
        /// </summary>
        [SerializeField]
        private RectTransform viewport;

        /// <summary>
        /// 页面大小的参考物体。
        /// - 页面的大小会设置到参考物体大小，垂直方向自动居中。
        /// - 如果参考物体为 null 则使用 viewport 为参考物体。
        /// </summary>
        [SerializeField]
        private RectTransform sizer;

        /// <summary>
        /// 页面间距。
        /// </summary>
        [SerializeField]
        private float pageSpace;

        private List<UIPoolableItemData> itemDatas = new List<UIPoolableItemData>();
        private readonly List<UIPage> pages = new List<UIPage>();
        private Vector2 pageSize;

        // 当前可见的 page 索引
        private int currentMinShownIndex = -1;
        private int currentMaxShownIndex = -1;

        private RectTransform Sizer
        {
            get { return sizer == null ? viewport : sizer; }
        }

        protected override void Awake()
        {
            Assert.IsNotNull(pageRect);
            Assert.IsNotNull(viewport);
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            pageRect.PageScrolled += UpdatePagesVisibilities;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            pageRect.PageScrolled -= UpdatePagesVisibilities;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Reset();
        }

        public override void Layout()
        {
            // 修改锚点到左上角
            UIHelper.FixedlyChangeAnchors(rectTransform, new Vector2(0, 1), new Vector2(0, 1));
            // 清空当前 page 数据
            ClearPages();
            pageSize = Sizer.rect.size;

            // 根据 item 数据重新建立 page 数据
            for (var i = 0; i < itemDatas.Count; i++)
            {
                var page = new UIPage
                {
                    AlbumLayout = rectTransform,
                    PageSize = pageSize,
                    Position = new Vector2(i * (pageSize.x + pageSpace), (pageSize.y - viewport.rect.height) * 0.5f),
                };

                page.SetItemData(itemDatas[i]);
                pages.Add(page);
            }

            // 计算内容大小
            CalculateFitableSize();
            // 刷新视口显示
            UpdatePagesVisibilities();
        }

        private void ClearPages()
        {
            foreach (UIPage page in pages)
            {
                page.Reset();
            }

            pages.Clear();
            currentMinShownIndex = -1;
            currentMaxShownIndex = -1;
        }

        private void UpdatePagesVisibilities()
        {
            // 视口宽度
            float viewportWidth = viewport.rect.width;
            // 视口左边缘相对 Layout 左上角的 x 坐标（通常正值）
            float layoutViewLeft = UILayoutHelper.GetLayoutLT2ViewportLT(rectTransform, viewport).x;

            // 视口左侧最近的不可见 page
            // pageWidth + (pageWidth + pageSpace) * minIndex <= layoutViewLeft;
            // 0.0001f 的作用：
            //   - 消除浮点误差的影响，如 2.0f 的实际结果可能是 2.0000001f
            //   - 解决 CeilToInt(2.0) == 2.0 的问题，保证 2.0 不被显示 
            // 视口内最左列
            int minShownIndex = Mathf.CeilToInt((layoutViewLeft - pageSize.x) / (pageSize.x + pageSpace) + 0.0001f);
            minShownIndex = Mathf.Max(minShownIndex, 0);

            // 视口右侧最近的不可见 page
            // (pageWidth + pageSpace) * minIndex => layoutViewLeft + viewportWidth;
            // 视口内最右列
            int maxShownIndex = Mathf.FloorToInt((layoutViewLeft + viewportWidth) / (pageSize.x + pageSpace) - 0.0001f);
            maxShownIndex = Mathf.Min(maxShownIndex, pages.Count - 1);

            if (currentMinShownIndex == -1)
            {
                for (int i = minShownIndex; i <= maxShownIndex; i++)
                {
                    pages[i].ShowItem();
                }
            }
            else if (currentMinShownIndex < minShownIndex || currentMaxShownIndex < maxShownIndex)
            {
                for (int i = currentMinShownIndex; i <= Mathf.Min(minShownIndex - 1, currentMaxShownIndex); i++)
                {
                    pages[i].HideItem();
                }

                for (int i = Mathf.Max(minShownIndex, currentMaxShownIndex + 1); i <= maxShownIndex; i++)
                {
                    pages[i].ShowItem();
                }
            }
            else if (currentMinShownIndex > minShownIndex || currentMaxShownIndex > maxShownIndex)
            {
                for (int i = Mathf.Max(currentMinShownIndex, maxShownIndex + 1); i <= currentMaxShownIndex; i++)
                {
                    pages[i].HideItem();
                }

                for (int i = minShownIndex; i <= Mathf.Min(maxShownIndex, currentMinShownIndex - 1); i++)
                {
                    pages[i].ShowItem();
                }
            }

            currentMinShownIndex = minShownIndex;
            currentMaxShownIndex = maxShownIndex;
        }

        //--------------------------------------------------
        // IUIPoolableItemLayout
        //--------------------------------------------------

        public void SetItemDatas(IList<UIPoolableItemData> data)
        {
            itemDatas = new List<UIPoolableItemData>(data);
            AutoLayout();
        }

        public void AddItemData(UIPoolableItemData data)
        {
            itemDatas.Add(data);
            AutoLayout();
        }

        public UIPoolableItemData GetItemData(int index)
        {
            Assert.IsTrue(index >= 0 && index <= itemDatas.Count - 1, string.Format("索引越界: {0}。", index));
            return itemDatas[index];
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public new void Reset()
        {
            itemDatas.Clear();
            ClearPages();
        }

        //--------------------------------------------------
        // IUISizeFitableLayout
        //--------------------------------------------------

        public event Action<Vector2> SizeChanged;
        public Vector2 SizeDelta { get; private set; }

        private void CalculateFitableSize()
        {
            SizeDelta = new Vector2(pageSize.x * itemDatas.Count + pageSpace * (itemDatas.Count - 1),
                                    rectTransform.sizeDelta.y);

            if (SizeChanged != null)
            {
                SizeChanged(SizeDelta);
            }
        }

        //--------------------------------------------------
        // IUIScrolltoableLayout
        //--------------------------------------------------

        public Vector2 GetItemCenterPosition(int index)
        {
            Assert.IsTrue(index >= 0 && index < itemDatas.Count,
                          string.Format("index: {0} (>=0 && < {1})", index, itemDatas.Count));

            return new Vector2((pageSize.x + pageSpace) * index + pageSize.x * 0.5f,
                               (pageSize.y - viewport.rect.size.y) * 0.5f - pageSize.y * 0.5f);
        }

        public Vector2 GetItemSize(int index)
        {
            return pageSize;
        }

        public Vector2 GetLayoutSize()
        {
            return rectTransform.rect.size;
        }

        public int ItemCount
        {
            get { return itemDatas.Count; }
        }
    }
}