using System.Collections.Generic;

namespace MMGame.UI
{
    /// <summary>
    /// 使用对象池管理内容的 UI 容器接口。
    /// 对象池可以根据任何规则管理内容，如根据可见性来处理 item prefab 的回收和创建。
    /// </summary>
    public interface IUIPoolableItemLayout
    {
        /// <summary>
        /// 设置所有 item 数据。
        /// </summary>
        /// <param name="data">Item 数据列表。</param>
        void SetItemDatas(IList<UIPoolableItemData> data);

        /// <summary>
        /// 添加一个 item 数据。
        /// </summary>
        /// <param name="data">Item 数据。</param>
        void AddItemData(UIPoolableItemData data);

        /// <summary>
        /// 根据 item 数据在列表中的索引获取 item 数据。
        /// </summary>
        /// <param name="index">Item 数据索引。</param>
        /// <returns>Item 数据。</returns>
        UIPoolableItemData GetItemData(int index);

        /// <summary>
        /// 刷新视口显示内容。
        /// </summary>
        void Refresh();

        /// <summary>
        /// 重置容器，清除所有数据及元素。
        /// </summary>
        void Reset();
    }
}