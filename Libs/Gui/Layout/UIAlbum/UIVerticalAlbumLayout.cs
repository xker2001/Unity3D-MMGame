using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 垂直滚动的相册控件，必须和 ScrollRect 配合使用。
    /// 
    /// 说明：
    /// - Layout 高度可以适配内容，宽度固定。
    /// - Layout 锚定方式任意，会被设置为左上角。
    /// - Layout pivot 任意。
    /// - Item 锚定方式任意，会被 Row 设置为左上角。
    /// - Item pivot 任意。
    /// - 所有 item 大小相同。
    /// </summary>
    public class UIVerticalAlbumLayout : AUILayout, IUIPoolableItemLayout, IUISizeFitableLayout, IUIScrolltoableLayout
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
        private readonly List<UIAlbumRow> rows = new List<UIAlbumRow>();
        private Vector2 rowSize;
        private Vector2 itemSize;

        // 当前可见 row 的索引
        private int currentMinShownIndex = -1;
        private int currentMaxShownIndex = -1;

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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            scrollRect.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// 构建元素显示结构（rows）。
        /// </summary>
        public override void Layout()
        {
            // 修改锚点到左上角
            UIHelper.FixedlyChangeAnchors(rectTransform, new Vector2(0, 1), new Vector2(0, 1));
            // 清空当前 row 数据
            ClearRows();
            float rowWidth = rectTransform.rect.width - leftPadding - rightPadding;
            float itemWidth = (rowWidth - columnSpace * (columnNumber - 1)) / columnNumber;
            rowSize = new Vector2(rowWidth, itemWidth / gridAspect);

            // 根据 item 数据重新建立 row 数据
            for (int i = 0; i < RowCount; i++)
            {
                // 创建 Row
                var row = new UIAlbumRow();
                row.AlbumLayout = rectTransform;
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
            // 滚动到最顶部
            scrollRect.verticalNormalizedPosition = 1;
            // 刷新视口显示
            UpdateRowsVisibilities();
        }

        /// <summary>
        /// 清除所有 Row 及其包含的元素。
        /// </summary>
        private void ClearRows()
        {
            foreach (UIAlbumRow row in rows)
            {
                row.Reset();
            }

            rows.Clear();
            currentMinShownIndex = -1;
            currentMaxShownIndex = -1;
        }

        /// <summary>
        /// ScrollRect 滚动回调函数。
        /// </summary>
        private void OnScrollValueChanged(Vector2 value)
        {
            UpdateRowsVisibilities();
        }

        /// <summary>
        /// 更新所有  row 的可见性，显示可见 Rows 的元素，隐藏（回收）不可见 Row 的元素。
        /// </summary>
        private void UpdateRowsVisibilities()
        {
            // 视口上边缘相对 Layout 左上角的 y 坐标（通常负值）
            float layoutViewTop = UILayoutHelper.GetLayoutLT2ViewportLT(rectTransform, viewport).y;
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

            if (currentMinShownIndex == -1)
            {
                for (int i = minShownIndex; i <= maxShownIndex; i++)
                {
                    rows[i].ShowItems();
                }
            }
            else if (currentMinShownIndex < minShownIndex || currentMaxShownIndex < maxShownIndex)
            {
                for (int i = currentMinShownIndex; i <= Mathf.Min(minShownIndex - 1, currentMaxShownIndex); i++)
                {
                    rows[i].HideItems();
                }

                for (int i = Mathf.Max(minShownIndex, currentMaxShownIndex + 1); i <= maxShownIndex; i++)
                {
                    rows[i].ShowItems();
                }
            }
            else if (currentMinShownIndex > minShownIndex || currentMaxShownIndex > maxShownIndex)
            {
                for (int i = Mathf.Max(currentMinShownIndex, maxShownIndex + 1); i <= currentMaxShownIndex; i++)
                {
                    rows[i].HideItems();
                }

                for (int i = minShownIndex; i <= Mathf.Min(maxShownIndex, currentMinShownIndex - 1); i++)
                {
                    rows[i].ShowItems();
                }
            }

            currentMinShownIndex = minShownIndex;
            currentMaxShownIndex = maxShownIndex;
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
            throw new NotImplementedException();
        }

        //--------------------------------------------------
        // IUISizeFitableLayout
        //--------------------------------------------------

        public event Action<Vector2> SizeChanged;
        public Vector2 SizeDelta { get; private set; }

        private void CalculateFitableSize()
        {
            SizeDelta = new Vector2(rectTransform.sizeDelta.x,
                                    topPadding + bottomPadding + rowSize.y * rows.Count
                                    + Mathf.Max(0, (rows.Count - 1)) * rowSpace);

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