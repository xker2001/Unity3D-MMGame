using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 虚拟 UI 行控件，
    /// - 虚拟 anchor 和 pivot 都在左上角。
    /// - Item 的 anchor 将被设置为左上角。
    /// - Item 的 pivot 任意。
    /// </summary>
    public class UIAlbumRow
    {
        private readonly List<UIPoolableItemData> itemDatas = new List<UIPoolableItemData>();
        private readonly List<Transform> items = new List<Transform>();

        /// <summary>
        /// Items 挂接的实际父节点。
        /// </summary>
        public RectTransform AlbumLayout { get; set; }

        /// <summary>
        /// 假想 anchor 和 pivot 都在左上角时的位置（anchoredPosition）。
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Row 长宽大小。
        /// </summary>
        public Vector2 RowSize { get; set; }

        /// <summary>
        /// Item 长宽大小。
        /// </summary>
        public Vector2 ItemSize { get; set; }

        /// <summary>
        /// AlbumItem 之间的间隙距离。
        /// </summary>
        public float ColumnSpace { get; set; }

        /// <summary>
        /// 当前是否可见。
        /// </summary>
        public bool IsShowingItems { get; private set; }

        /// <summary>
        /// 重置类中的资源（以备重用或 GC）。
        /// </summary>
        public void Reset()
        {
            itemDatas.Clear();
            HideItems();
        }

        /// <summary>
        /// 添加一个元素数据。
        /// </summary>
        /// <param name="data">元素数据。</param>
        public void AddItemData(UIPoolableItemData data)
        {
            itemDatas.Add(data);
        }

        /// <summary>
        /// 从对象池创建并显示 items。
        /// </summary>
        public void ShowItems()
        {
            if (IsShowingItems)
            {
                return;
            }

            for (int i = 0; i < itemDatas.Count; i++)
            {
                ShowItem(i);
            }

            IsShowingItems = true;
        }

        /// <summary>
        /// 将 items 回收到对象池。
        /// </summary>
        public void HideItems()
        {
            if (!IsShowingItems)
            {
                return;
            }

            foreach (Transform item in items)
            {
                PoolManager.Despawn(item);
            }

            items.Clear();
            IsShowingItems = false;
        }

        /// <summary>
        /// 显示一个 item。
        /// </summary>
        /// <param name="index">Item 索引。</param>
        private void ShowItem(int index)
        {
            // 创建 item 并设置数据
            Transform itemPrefab = itemDatas[index].Prefab;
            string poolName = itemPrefab.name;
            var item = PoolManager.Spawn(poolName, itemPrefab).GetComponent<AUIPoolableItem>();
            Assert.IsNotNull(item);
            item.SetData(itemDatas[index]);

            // 设置 item 参数及位置
            var itemRectXform = item.GetComponent<RectTransform>();
            Assert.IsNotNull(itemRectXform);
            itemRectXform.parent = AlbumLayout;
            itemRectXform.localScale = itemPrefab.localScale;
            UIHelper.FixedlyChangeAnchors(itemRectXform, new Vector2(0, 1), new Vector2(0, 1));
            itemRectXform.sizeDelta = ItemSize;
            Rect itemRect = itemRectXform.rect;
            Vector2 itemPivot = itemRectXform.pivot;

            itemRectXform.anchoredPosition =
                new Vector2(Position.x + itemRect.width * (itemPivot.x + index) + index * ColumnSpace,
                            Position.y - itemRect.height * (1 - itemPivot.y));

            // 添加到元素列表
            items.Add(item.transform);
        }
    }
}