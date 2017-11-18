using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 垂直滚动的 grid 布局控件，必须和 ScrollRect 配合使用。
    /// 
    /// 说明：
    /// - Layout 高度可以适配内容，宽度固定。
    /// - Layout 锚定方式任意，会被设置为左上角。
    /// - Layout pivot 任意。
    /// - Item 锚定方式任意，会被 Row 设置为左上角。
    /// - Item pivot 任意。
    /// - 所有 item 大小相同。
    /// </summary>
    public class UIVerticalPoolableGridLayout : AUILayout, IUIPoolableItemLayout,
                                                IUISizeFitableLayout, IUIScrolltoableLayout
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
        private float columnSpace;

        [SerializeField]
        private int columnNumber = 4;

        /// <summary>
        /// 元素格的长宽比。所有元素格的大小都一样。
        /// </summary>
        [SerializeField]
        private float gridAspect = 1.33333f;

        /// <summary>
        /// ScrollRect 控件物体。
        /// </summary>
        [SerializeField]
        private ScrollRect scrollRect;

        /// <summary>
        /// ScrollRect.viewport 物体。
        /// </summary>
        [SerializeField]
        private RectTransform viewport;

        private List<UIPoolableItemData> itemDatas = new List<UIPoolableItemData>();
        private readonly List<UIPoolableGridRow> rows = new List<UIPoolableGridRow>();
        private Vector2 rowSize;
        private Vector2 itemSize; // 实际 item 的尺寸，不包含间隙

        /// <summary>
        /// 上次更新后可见 row 的最小索引
        /// </summary>
        private int lastMinShownIndex = -1;

        /// <summary>
        /// 上次更新后可见 row 的最大索引
        /// </summary>
        private int lastMaxShownIndex = -1;

        private int RowCount
        {
            get { return Mathf.CeilToInt(1.0f * itemDatas.Count / columnNumber); }
        }

        protected override void Awake()
        {
            Assert.IsNotNull(scrollRect);
            Assert.IsNotNull(viewport);
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            // 防止在 disable 状态下发生滚动但 Row 的显示却没有更新。
            UpdateRowsVisibilities();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            scrollRect.onValueChanged.RemoveAllListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Reset();
        }

        /// <summary>
        /// 构建元素显示结构（rows）。
        /// </summary>
        public override void Layout()
        {
            // 修改锚点到左上角
            UIUtility.FixedlyChangeAnchors(rectTransform, new Vector2(0, 1), new Vector2(0, 1));
            Refresh();
            scrollRect.verticalNormalizedPosition = 1;
            // 当锚点不在顶端时上一行滚动指令会产生实际效果但不会发出 OnScrollValueChanged 事件，
            // 因此这里手动再刷新一次以保证视口 item 显示正确。
            Refresh();
        }

        /// <summary>
        /// 清除所有 Row 及其包含的元素。
        /// </summary>
        private void ClearRows()
        {
            foreach (UIPoolableGridRow row in rows)
            {
                row.Reset();
            }

            rows.Clear();
            lastMinShownIndex = -1;
            lastMaxShownIndex = -1;
        }

        /// <summary>
        /// ScrollRect 滚动回调函数。
        /// </summary>
        private void OnScrollValueChanged(Vector2 value)
        {
            UpdateRowsVisibilities();
        }

        /// <summary>
        /// 更新所有 row 的可见性，显示可见 rows 的元素，隐藏（回收）不可见 row 的元素。
        /// </summary>
        private void UpdateRowsVisibilities()
        {
            // 视口上边缘相对 Layout 左上角的 y 坐标（通常负值）
            float layoutViewTop = UILayoutUtility.GetLayoutLT2ViewportLT(rectTransform, viewport).y;
            float layoutViewBottom = layoutViewTop - viewport.rect.height;

            // 视口上部最近的不可见 row
            // -topPadding - rowSize.y - (rowSpace + rowSize.y) * minIndex < layoutViewTop;
            // 视口内最上排
            int minShownIndex = Mathf.CeilToInt(-(layoutViewTop + topPadding + rowSize.y) / (rowSpace + rowSize.y));
            minShownIndex = Mathf.Max(minShownIndex, 0);

            // 视口下部最近的不可见 row
            // -topPadding - (rowSpace + rowSize.y) * maxIndex > layoutViewBottom;
            // 视口内最下排
            int maxShownIndex = Mathf.FloorToInt(-(layoutViewBottom + topPadding) / (rowSpace + rowSize.y));
            maxShownIndex = Mathf.Min(maxShownIndex, rows.Count - 1);

            if (lastMinShownIndex == -1)
            {
                for (int i = minShownIndex; i <= maxShownIndex; i++)
                {
                    rows[i].ShowItems();
                }
            }
            else if (lastMinShownIndex < minShownIndex || lastMaxShownIndex < maxShownIndex)
            {
                for (int i = lastMinShownIndex; i <= Mathf.Min(minShownIndex - 1, lastMaxShownIndex); i++)
                {
                    rows[i].HideItems();
                }

                for (int i = Mathf.Max(minShownIndex, lastMaxShownIndex + 1); i <= maxShownIndex; i++)
                {
                    rows[i].ShowItems();
                }
            }
            else if (lastMinShownIndex > minShownIndex || lastMaxShownIndex > maxShownIndex)
            {
                for (int i = Mathf.Max(lastMinShownIndex, maxShownIndex + 1); i <= lastMaxShownIndex; i++)
                {
                    rows[i].HideItems();
                }

                for (int i = minShownIndex; i <= Mathf.Min(maxShownIndex, lastMinShownIndex - 1); i++)
                {
                    rows[i].ShowItems();
                }
            }

            lastMinShownIndex = minShownIndex;
            lastMaxShownIndex = maxShownIndex;
        }

        //--------------------------------------------------
        // IUIPoolableItemLayout
        //--------------------------------------------------

        public new void Reset()
        {
            itemDatas.Clear();
            ClearRows();
        }

        public void SetItemDatas(IList<UIPoolableItemData> data)
        {
            itemDatas = new List<UIPoolableItemData>(data);
            AutoLayout();
        }

        public void AddItemData(UIPoolableItemData itemData)
        {
            itemDatas.Add(itemData);
            AutoLayout();
        }

        public UIPoolableItemData GetItemData(int index)
        {
            Assert.IsTrue(index > 0 && index <= itemDatas.Count - 1, string.Format("索引越界: {0}。", index));
            return itemDatas[index];
        }

        public void Refresh()
        {
            float rowWidth = rectTransform.rect.width - leftPadding - rightPadding;
            float itemWidth = (rowWidth - columnSpace * (columnNumber - 1)) / columnNumber;
            rowSize = new Vector2(rowWidth, itemWidth / gridAspect);

            // 清空当前 row 数据
            ClearRows();

            // 根据 item 数据重新建立 row 数据
            for (int i = 0; i < RowCount; i++)
            {
                // 创建 Row
                var row = new UIPoolableGridRow();
                row.GridLayout = rectTransform;
                row.RowSize = rowSize;
                row.ItemSize = new Vector2(itemWidth, rowSize.y);
                row.Position = new Vector2(leftPadding, -topPadding - i * (rowSpace + row.RowSize.y));
                row.ColumnSpace = columnSpace;
                itemSize = row.ItemSize;

                // 添加元素数据
                for (int j = 0; j < columnNumber; j++)
                {
                    int dataIndex = i * columnNumber + j;

                    if (dataIndex >= itemDatas.Count)
                    {
                        break;
                    }

                    row.AddItemData(itemDatas[dataIndex]);
                }

                // 添加 Row 到列表
                rows.Add(row);
            }

            // 计算内容大小
            CalculateFitableSize();
            // 刷新视口显示
            UpdateRowsVisibilities();
        }

        //--------------------------------------------------
        // IUISizeFitableLayout
        //--------------------------------------------------

        public event Action<float, float> SizeChanged;
        public Vector2 Size { get; private set; }

        private void CalculateFitableSize()
        {
            Size = new Vector2(rectTransform.sizeDelta.x,
                               topPadding + bottomPadding + rowSize.y * rows.Count
                               + Mathf.Max(0, (rows.Count - 1)) * rowSpace);

            if (SizeChanged != null)
            {
                SizeChanged(Size.x, Size.y);
            }
        }

        //--------------------------------------------------
        // IUIScrolltoableLayout
        //--------------------------------------------------

        public Vector2 GetItemCenterPosition(int index)
        {
            Assert.IsTrue(index >= 0 && index < itemDatas.Count,
                          string.Format("index: {0} (>=0 && < {1})", index, itemDatas.Count));

            int row = index / columnNumber;
            int col = index % columnNumber;
            var posToLeftTop = new Vector2(rows[row].Position.x + itemSize.x * (0.5f + col) + col * columnSpace,
                                           rows[row].Position.y - itemSize.y * 0.5f);
            return posToLeftTop;
        }

        public Vector2 GetItemSize(int index)
        {
            return itemSize;
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