using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// Page layout 的页容器，负责显示（创建）/隐藏（回收）所属的 page prefab。
    /// 假定 anchor 和 pivot 都在左上角。
    /// Prefab 的 anchor 和 pivot 也将被设置为左上角。
    /// </summary>
    public class UIPoolablePage
    {
        private UIPoolableItemData itemData;
        private Transform itemXform;

        /// <summary>
        /// Items 挂接的实际父节点。
        /// </summary>
        public RectTransform PageLayout { get; set; }

        /// <summary>
        /// 假想 anchor 和 pivot 都在左上角时的位置（anchoredPosition）。
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Row 长宽大小。
        /// </summary>
        public Vector2 PageSize { get; set; }

        /// <summary>
        /// 当前是否可见。
        /// </summary>
        public bool IsShowingItem { get; private set; }

        /// <summary>
        /// 重置类中的资源（以备重用或 GC）。
        /// </summary>
        public void Reset()
        {
            itemData = null;
            HideItem();
        }

        /// <summary>
        /// 设置元素数据。
        /// </summary>
        /// <param name="data">元素数据。</param>
        public void SetItemData(UIPoolableItemData data)
        {
            itemData = data;
        }

        /// <summary>
        /// 从对象池创建并显示 item。
        /// </summary>
        public void ShowItem()
        {
            if (IsShowingItem)
            {
                return;
            }

            // 创建 item 并设置数据
            Transform itemPrefab = itemData.Prefab;
            string poolName = itemPrefab.name;
            var item = PoolManager.Spawn(poolName, itemPrefab).GetComponent<AUIPoolableItem>();
            Assert.IsNotNull(item);
            item.SetData(itemData);

            // 设置 item 参数及位置
            var itemRectXform = item.GetComponent<RectTransform>();
            Assert.IsNotNull(itemRectXform);
            itemRectXform.SetParent(PageLayout, true);
            itemRectXform.localScale = itemPrefab.localScale;
            UIUtility.FixedlyChangeAnchors(itemRectXform, new Vector2(0, 1), new Vector2(0, 1));
            itemRectXform.sizeDelta = PageSize;
            Rect itemRect = itemRectXform.rect;
            Vector2 itemPivot = itemRectXform.pivot;

            itemRectXform.anchoredPosition = new Vector2(Position.x + itemRect.width * itemPivot.x,
                                                         Position.y - itemRect.height * (1 - itemPivot.y));

            itemXform = item.transform;
            IsShowingItem = true;
        }

        /// <summary>
        /// 将 items 回收到对象池。
        /// </summary>
        public void HideItem()
        {
            if (!IsShowingItem)
            {
                return;
            }

            PoolManager.Despawn(itemXform);
            itemXform = null;
            IsShowingItem = false;
        }
    }
}