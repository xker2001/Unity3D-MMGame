using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 支持滚到动指定 item 的功能的接口。
    /// </summary>
    public interface IUIScrolltoableLayout
    {
        /// <summary>
        /// Item 中心点（非 pivot）相对 Layout 左上角 (rect.xMin, rect.yMax) 的位置。
        /// 注意 y 轴方向的值是负数。
        /// </summary>
        /// <param name="index">Item 索引。</param>
        /// <returns>Item 位置。</returns>
        Vector2 GetItemCenterPosition(int index);

        /// <summary>
        /// 获取 item 的大小，当有需要滚动 item 到边缘的时候使用。
        /// </summary>
        /// <param name="index">Item 索引。</param>
        /// <returns>Item 大小。</returns>
        Vector2 GetItemSize(int index);

        /// <summary>
        /// 获取 item 父容器的大小。
        /// </summary>
        /// <returns>父容器的大小。</returns>
        Vector2 GetLayoutSize();

        /// <summary>
        /// Item 的总数。
        /// </summary>
        int ItemCount { get; }
    }
}