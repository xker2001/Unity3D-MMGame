using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 设置指定索引的 IUIScrolltoableLayout 子控件到视口。如果有可能，设置到中央。
    ///
    /// 组件结构范例：
    /// - panel: UIScrollLayoutTo
    /// ----- scroll view: ScrollRect, Mask (Mask 应该和 ScrollRect 大小相同)
    /// --------- container: UIVerticalListLayout, UIFitLayoutSize
    /// </summary>
    public class UIScrollLayoutTo : EasyUIBehaviour
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        [SerializeField]
        private ScrollRect scrollRect;

        [Message(text = "未实现 IUIScrolltoableLayout", messageType = MessageType.Error,
            method = "IsNotIUIScrolltoableLayout")]
        [SerializeField]
        private MonoBehaviour layout;

        private bool IsNotIUIScrolltoableLayout()
        {
            // 必须注入组件/Prefab
            return !this.CheckMustHaveInjection(layout, typeof(IUIScrolltoableLayout));
        }

        [SerializeField]
        private bool ignoreHorizontal;

        [SerializeField]
        private bool ignoreVertical;

        protected override void Awake()
        {
            Assert.IsNotNull(scrollRect);
            Assert.IsNotNull(layout);
            base.Awake();
        }

        /// <summary>
        /// 设置指定索引的子控件到视口中间。
        ///
        /// 滚动到底部（0）时中间线对应的 index 比例：(scrollHeight * 0.5f) / layoutHeight
        /// 滚动到顶部（1）时中间线对应的 index 比例：(layoutHeight - scrollHeight / 2) / layoutHeight
        /// </summary>
        /// <param name="index">条目所在的行数索引。</param>
        public void SetToCenter(int index)
        {
            var layoutRectTransform = layout.GetComponent<RectTransform>();
            Assert.IsNotNull(layoutRectTransform);

            var scrollToInfo = layout as IUIScrolltoableLayout;
            Assert.IsNotNull(scrollToInfo);

            if (!ignoreHorizontal)
            {
                SetToCenter(index, scrollToInfo, Direction.Horizontal);
            }

            if (!ignoreVertical)
            {
                SetToCenter(index, scrollToInfo, Direction.Vertical);
            }
        }

        /// <summary>
        /// 设置指定索引的子控件到视口可见位置。
        /// 如果指定索引的子控件已经完全可见则不做任何改动，否则设置到视口离当前位置较近的边缘。
        /// </summary>
        /// <param name="index">条目所在的行数索引。</param>
        public void SetToVisible(int index)
        {
            var layoutRectTransform = layout.GetComponent<RectTransform>();
            Assert.IsNotNull(layoutRectTransform);

            var scrollToInfo = layout as IUIScrolltoableLayout;
            Assert.IsNotNull(scrollToInfo);

            if (!ignoreHorizontal)
            {
                SetToVisible(index, scrollToInfo, Direction.Horizontal);
            }

            if (!ignoreVertical)
            {
                SetToVisible(index, scrollToInfo, Direction.Vertical);
            }
        }

        /// <summary>
        /// 执行滚动到视口中间。
        /// </summary>
        /// <param name="index">Item 索引。</param>
        /// <param name="scrollableLayout">Layout。</param>
        /// <param name="direction"> 滚动方向。</param>
        private void SetToCenter(int index, IUIScrolltoableLayout scrollableLayout, Direction direction)
        {
            float scrollRectLen = direction == Direction.Vertical
                                      ? scrollRect.GetComponent<RectTransform>().rect.height
                                      : scrollRect.GetComponent<RectTransform>().rect.width;

            float layoutLen = direction == Direction.Vertical
                                  ? scrollableLayout.GetLayoutSize().y
                                  : scrollableLayout.GetLayoutSize().x;

            // layout 不满一屏
            if (layoutLen <= scrollRectLen)
            {
                return;
            }

            // 计算滚动到顶部和底部时 ScrollRect 位置对应的 layout 位置
            float startNorPos = (scrollRectLen * 0.5f) / layoutLen; //  scroll 0
            float endNorPos = (layoutLen - scrollRectLen * 0.5f) / layoutLen; // scroll 1

            // 当前 item 中心在 layout 中的位置（0~1）
            float itemPos = direction == Direction.Vertical
                                ? scrollableLayout.GetItemCenterPosition(index).y
                                : scrollableLayout.GetItemCenterPosition(index).x;

            float targetNorPos = direction == Direction.Vertical
                                     ? 1 + itemPos / layoutLen
                                     : itemPos / layoutLen;

            // layout 位置换算成 scroll 位置
            float scrollNorPos = (targetNorPos - startNorPos) / (endNorPos - startNorPos);
            scrollNorPos = Mathf.Clamp01(scrollNorPos);

            // 将 layout 设置到 scroll 位置
            if (direction == Direction.Vertical)
            {
                scrollRect.verticalNormalizedPosition = scrollNorPos;
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = scrollNorPos;
            }
        }

        /// <summary>
        /// 执行滚动进视口。
        /// </summary>
        /// <param name="index">Item 索引。</param>
        /// <param name="scrollableLayout">Layout。</param>
        /// <param name="direction">滚动方向。</param>
        private void SetToVisible(int index, IUIScrolltoableLayout scrollableLayout, Direction direction)
        {
            float scrollRectLen = direction == Direction.Vertical
                                      ? scrollRect.GetComponent<RectTransform>().rect.height
                                      : scrollRect.GetComponent<RectTransform>().rect.width;

            float layoutLen = direction == Direction.Vertical
                                  ? scrollableLayout.GetLayoutSize().y
                                  : scrollableLayout.GetLayoutSize().x;
            // layout 不满一屏
            if (layoutLen <= scrollRectLen)
            {
                return;
            }

            // 计算滚动到顶部和底部时各自对应的 layout 位置（viewport 中间位置对应的 layout 的 normalized 坐标）
            float startNorPos = (scrollRectLen * 0.5f) / layoutLen; // 0
            float endNorPos = (layoutLen - scrollRectLen * 0.5f) / layoutLen; // 1

            // viewport 中间对应的 layout 位置
            float currentScrollPos = direction == Direction.Vertical
                                         ? scrollRect.verticalNormalizedPosition
                                         : scrollRect.horizontalNormalizedPosition;
            float currentNorPos = currentScrollPos * (endNorPos - startNorPos) + startNorPos;

            // item 在 viewport 中心、 两侧边缘时 layout 的位置
            float itemCenterPos = direction == Direction.Vertical
                                      ? scrollableLayout.GetItemCenterPosition(index).y
                                      : scrollableLayout.GetItemCenterPosition(index).x;

            float itemLen = direction == Direction.Vertical
                                ? scrollableLayout.GetItemSize(index).y
                                : scrollableLayout.GetItemSize(index).x;

            float itemMaxEdgeNorPos = direction == Direction.Vertical
                                          ? 1 + (itemCenterPos + scrollRectLen * 0.5f - itemLen * 0.5f) / layoutLen
                                          : (itemCenterPos + scrollRectLen * 0.5f - itemLen * 0.5f) / layoutLen;

            float itemMinEdgeNorPos = direction == Direction.Vertical
                                          ? 1 + (itemCenterPos - scrollRectLen * 0.5f + itemLen * 0.5f) / layoutLen
                                          : (itemCenterPos - scrollRectLen * 0.5f + itemLen * 0.5f) / layoutLen;

            // item 是否完全可见
            if (currentNorPos <= itemMaxEdgeNorPos && currentNorPos >= itemMinEdgeNorPos)
            {
                return;
            }

            // 滚动到较近的边缘
            float targetNorPos = currentNorPos < itemMinEdgeNorPos ? itemMinEdgeNorPos : itemMaxEdgeNorPos;

            // layout 位置换算成 scroll 位置
            float scrollNorPos = (targetNorPos - startNorPos) / (endNorPos - startNorPos);
            scrollNorPos = Mathf.Clamp01(scrollNorPos);

            // 将 layout 设置到 scroll 位置
            if (direction == Direction.Vertical)
            {
                scrollRect.verticalNormalizedPosition = scrollNorPos;
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = scrollNorPos;
            }
        }

        //--------------------------------------------------
        // 测试方法
        //--------------------------------------------------

        [SerializeField]
        private int testIndex = 9;

        [Inspector]
        private void TestSetToCenter()
        {
            SetToCenter(testIndex);
        }

        [Inspector]
        private void TestSetToVisible()
        {
            SetToVisible(testIndex);
        }
    }
}