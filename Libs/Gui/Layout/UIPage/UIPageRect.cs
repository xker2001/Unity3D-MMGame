using System;
using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MMGame.UI
{
    /// <summary>
    /// 翻页控件。
    /// </summary>
    public class UIPageRect : EasyUIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,
                              IEndDragHandler, IDragHandler
    {
        /// <summary>
        /// 页面滚动方向。
        /// </summary>
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        /// 拉动至边缘后页面行为模式。
        /// </summary>
        public enum MovementType
        {
            Elastic, // 弹性
            Clamped, // 无弹性
        }

        /// <summary>
        /// 翻页方向（横向还是竖向）。
        /// </summary>
        [SerializeField]
        private Direction direction = Direction.Horizontal;

        /// <summary>
        /// 拉动至边缘后页面行为模式。
        /// </summary>
        [SerializeField]
        private MovementType movementType = MovementType.Elastic;

        /// <summary>
        /// Viewport 控件（Mask）。
        /// 页面布局控件的父控件。
        /// </summary>
        [SerializeField]
        private RectTransform viewport;

        /// <summary>
        /// 配合使用的页面布局器，提供 IUIScrolltoableLayout 接口。
        /// </summary>
        [Message(text = "未实现 IUIScrolltoableLayout", messageType = MessageType.Error,
            method = "IsNotIUIScrolltoableLayout")]
        [SerializeField]
        private MonoBehaviour layout;

        private bool IsNotIUIScrolltoableLayout()
        {
            // 必须注入组件/Prefab
            return !this.CheckMustHaveInjection(layout, typeof(IUIScrolltoableLayout));
        }

        /// <summary>
        /// 确认进行翻页的最小指滑速度。
        /// 当手指在滚动方向上滑动速度大于该值时进行翻页。
        /// </summary>
        [SerializeField]
        private float minSwipeSpeed = 500f;

        /// <summary>
        /// 页面最小滚动速度。
        /// </summary>
        [SerializeField]
        private float minScrollSpeed = 2000f;

        /// <summary>
        /// 拉动至边缘时的弹性强度。
        /// 强度越小越容易拉离边缘。
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        private float elasticStrength = 0.9f;

        private RectTransform layoutRectTransform;
        private IUIScrolltoableLayout scrolltoableLayout;
        private bool isDragging;

        /// <summary>
        /// 触摸点在 viewport 上的位置。
        /// </summary>
        private Vector2 startTouchPosition;

        /// <summary>
        /// 开始拖动时 layout 的锚定位置，用于计算 layout 的移动。
        /// </summary>
        private Vector2 startAnchoredPosition;

        /// <summary>
        /// 上一帧 layout 的锚定位置，用于计算拖动速度。
        /// </summary>
        private Vector2 lastAnchoredPosition;

        /// <summary>
        /// 当前的目标页面索引。
        /// </summary>
        private int targetPageIndex = -1;

        /// <summary>
        /// 对齐到目标页面时 Layout 和 viewport 左上角之间的距离（有符号）。
        /// </summary>
        private float targetCornerDistance;

        /// <summary>
        /// 是否开始对指滑速度进行指数平滑。
        /// 只有瞬间指滑速度超过阈值才开始记录并平滑。
        /// </summary>
        private bool startSmooth;

        /// <summary>
        /// 平滑后的指滑速度。
        /// </summary>
        private Vector2 smoothVelocity;

        /// <summary>
        /// 指数平滑因子，越小越平滑。
        /// </summary>
        private float smoothFactor = 0.5f;

        public event Action PageScrolled;
        public event Action<int> SetTargetPage;

        /// <summary>
        /// 当前居中的目标页面索引。
        /// </summary>
        public int TargetPage
        {
            get { return targetPageIndex; }
        }

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(viewport);
            Assert.IsNotNull(layout);

            layoutRectTransform = layout.GetComponent<RectTransform>();
            Assert.IsNotNull(layoutRectTransform);

            scrolltoableLayout = layout as IUIScrolltoableLayout;
            Assert.IsNotNull(scrolltoableLayout);
        }

        /// <summary>
        /// 准备拖动。
        /// </summary>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            lastAnchoredPosition = layoutRectTransform.anchoredPosition;
            startSmooth = false;
            smoothVelocity = Vector2.zero;
        }

        /// <summary>
        /// 开始拖动。
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 触摸点在 viewport 上的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport, eventData.position, eventData.pressEventCamera, out startTouchPosition);

            startAnchoredPosition = layoutRectTransform.anchoredPosition;
            isDragging = true;
        }

        /// <summary>
        /// 结束拖动。
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            float swipeSpeed = direction == Direction.Horizontal ? smoothVelocity.x : smoothVelocity.y;

            // 如果指滑速度够大，则滚动到上/下一页
            if (Mathf.Abs(swipeSpeed) >= minSwipeSpeed)
            {
                if ((direction == Direction.Horizontal && swipeSpeed < 0)
                    || (direction == Direction.Vertical && swipeSpeed > 0))
                {
                    ScrollToNextPage();
                }
                else
                {
                    ScrollToPreviousPage();
                }
            }
            // 如果指滑速度不够大，则滚动到当前屏幕的页面
            else
            {
                ScrollToScreenPage();
            }
        }

        /// <summary>
        /// 拖动中。
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            // 当前触摸点位置
            Vector2 touchPosition;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    viewport, eventData.position, eventData.pressEventCamera, out touchPosition))
            {
                return;
            }

            // 跟随触摸点时 layout 的位置
            Vector2 deltaPosition = touchPosition - startTouchPosition;
            Vector2 anchoredPosition = startAnchoredPosition + deltaPosition;

            // 将 layout 约束回 viewport 内
            Vector2 outOffset = GetOutOffset(anchoredPosition);
            anchoredPosition += outOffset;

            // Elastic 模式下允许 layout 被拉离屏幕边缘
            if (movementType == MovementType.Elastic)
            {
                if (!Mathf.Approximately(outOffset.x, 0))
                {
                    anchoredPosition.x -= GetRubberDelta(outOffset.x);
                }

                if (!Mathf.Approximately(outOffset.y, 0))
                {
                    anchoredPosition.y -= GetRubberDelta(outOffset.y);
                }
            }

            // 设置 layout 位置
            SetLayoutAnchoredPosition(anchoredPosition);
        }

        protected void LateUpdate()
        {
            if (!layoutRectTransform || scrolltoableLayout.ItemCount == 0)
            {
                return;
            }

            float deltaTime = Time.unscaledDeltaTime;

            // 拖动时记录平滑速度
            if (isDragging)
            {
                Vector2 velocity = (layoutRectTransform.anchoredPosition - lastAnchoredPosition) / deltaTime;

                // 指滑速度大于阈值才开始记录
                if (!startSmooth)
                {
                    float speed = Mathf.Abs(direction == Direction.Horizontal ? velocity.x : velocity.y);

                    if (speed > minSwipeSpeed)
                    {
                        startSmooth = true;
                    }
                }

                // 指数平滑
                if (startSmooth)
                {
                    smoothVelocity = smoothFactor * velocity + (1 - smoothFactor) * smoothVelocity;
                }

                lastAnchoredPosition = layoutRectTransform.anchoredPosition;
                return;
            }

            // 将 page layout 创建时的位置设置为初始位置
            if (targetPageIndex == -1)
            {
                SetTargetIndex(GetViewportPage());
            }

            // 滚动到目标位置

            // 当前位置 layout 和 viewport 左上角之间的距离（有方向）
            Vector2 cornerDist = UILayoutHelper.GetLayoutLT2ViewportLT(layoutRectTransform, viewport);

            // 进行横向滚动
            if (direction == Direction.Horizontal && !Mathf.Approximately(cornerDist.x, targetCornerDistance))
            {
                float speed = Mathf.Max(minScrollSpeed, Mathf.Abs(smoothVelocity.x)); // 滚动速度不低于下限
                float delta = cornerDist.x - targetCornerDistance;
                float step = Mathf.Abs(speed) * Time.unscaledDeltaTime * Mathf.Sign(delta);

                if ((delta < 0 && step < delta) || (delta > 0 && step > delta))
                {
                    step = delta;
                }

                SetLayoutAnchoredPosition(layoutRectTransform.anchoredPosition + new Vector2(step, 0));
            }
            // 进行纵向滚动
            else if (direction == Direction.Vertical && !Mathf.Approximately(cornerDist.y, targetCornerDistance))
            {
                float speed = Mathf.Max(minScrollSpeed, Mathf.Abs(smoothVelocity.y));
                float delta = cornerDist.y - targetCornerDistance;
                float step = Mathf.Abs(speed) * Time.unscaledDeltaTime * Mathf.Sign(delta);

                if ((delta < 0 && step < delta) || (delta > 0 && step > delta))
                {
                    step = delta;
                }

                SetLayoutAnchoredPosition(layoutRectTransform.anchoredPosition + new Vector2(0, step));
            }
        }

        /// <summary>
        /// 计算如果 layout 被移动到指定 anchoredPosition 时偏移出 viewport 的向量。
        /// </summary>
        /// <param name="anchoredPosition">指定的 layout 锚定位置。</param>
        /// <returns>偏移量。</returns>
        private Vector2 GetOutOffset(Vector2 anchoredPosition)
        {
            // 计入 viewport 和 page 尺寸差异
            Vector2 deltaSize = (viewport.rect.size - scrolltoableLayout.GetItemSize(0)) * 0.5f;

            // 目标位置时 viewport 和 layout 左上角之间的距离
            Vector2 layoutLT2ViewLT = UILayoutHelper.GetLayoutLT2ViewportLT(layoutRectTransform, viewport);
            layoutLT2ViewLT += layoutRectTransform.anchoredPosition - anchoredPosition;

            if (direction == Direction.Horizontal)
            {
                // 左端超出（右滑）
                if (layoutLT2ViewLT.x + deltaSize.x < 0)
                {
                    return new Vector2(layoutLT2ViewLT.x + deltaSize.x, 0);
                }
                // 右端超出（左滑）
                else if (layoutLT2ViewLT.x + viewport.rect.width - deltaSize.x > layoutRectTransform.rect.width)
                {
                    return new Vector2(
                        layoutLT2ViewLT.x + viewport.rect.width - layoutRectTransform.rect.width - deltaSize.x, 0);
                }
            }
            else
            {
                // 上端超出（下滑）
                if (layoutLT2ViewLT.y - deltaSize.y > 0)
                {
                    return new Vector2(0, layoutLT2ViewLT.y - deltaSize.y);
                }
                // 下端超出（上滑）
                else if (layoutLT2ViewLT.y - viewport.rect.height + deltaSize.y < -layoutRectTransform.rect.height)
                {
                    return new Vector2(
                        0,
                        layoutLT2ViewLT.y - viewport.rect.height + layoutRectTransform.rect.height + deltaSize.y);
                }
            }

            return Vector2.zero;
        }

        /// <summary>
        /// 设置 layout 的 anchoredPosition。
        /// </summary>
        /// <param name="position">位置坐标。</param>
        private void SetLayoutAnchoredPosition(Vector2 position)
        {
            if (direction == Direction.Horizontal)
            {
                position.y = layoutRectTransform.anchoredPosition.y;
            }
            else
            {
                position.x = layoutRectTransform.anchoredPosition.x;
            }

            if (position == layoutRectTransform.anchoredPosition)
            {
                return;
            }

            layoutRectTransform.anchoredPosition = position;

            // 事件回调
            if (PageScrolled != null)
            {
                PageScrolled();
            }
        }

        /// <summary>
        /// 在屏幕边缘 layout 拉出距离计算。
        /// </summary>
        /// <param name="outOffset">滑动时（本应）超出屏幕的偏移。</param>
        /// <returns>Layout 被拉离 viewport 边缘的距离。</returns>
        private float GetRubberDelta(float outOffset)
        {
            float factor = Math.Abs(outOffset / viewport.rect.width); // 拉得越多，移动越艰难
            return Mathf.Sign(outOffset) * Mathf.Pow(factor, 0.75f) * viewport.rect.width * (1 - elasticStrength);
        }

        /// <summary>
        /// 设置目标页。
        /// </summary>
        /// <param name="index"></param>
        private void SetTargetIndex(int index)
        {
            if (index != targetPageIndex)
            {
                targetPageIndex = index;

                // 在目标位置时 layout 和 viewport 左上角之间的距离
                Vector2 itemCenterPos = scrolltoableLayout.GetItemCenterPosition(index);
                targetCornerDistance = direction == Direction.Horizontal
                                           ? itemCenterPos.x - viewport.rect.width * 0.5f
                                           : itemCenterPos.y + viewport.rect.height * 0.5f;

                if (SetTargetPage != null)
                {
                    SetTargetPage(index);
                }
            }
        }

        /// <summary>
        /// 获取占据当前屏幕主要部分的页面的索引。
        /// </summary>
        /// <returns>索引值。</returns>
        private int GetViewportPage()
        {
            int pageCount = scrolltoableLayout.ItemCount;

            if (pageCount == 0 || pageCount == 1)
            {
                return 0;
            }

            // 二分法查找离 viewport 中心最近的 page
            int startIndex = 0;
            int endIndex = scrolltoableLayout.ItemCount - 1;
            int result;

            while (true)
            {
                if (startIndex == endIndex)
                {
                    result = startIndex;
                    break;
                }

                int currentIndex = startIndex + Mathf.FloorToInt((endIndex - startIndex) * 0.5f);
                int seek = GetNextSeekDirection(currentIndex);

                if (seek == 0)
                {
                    result = currentIndex;
                    break;
                }
                else if (seek == -1)
                {
                    endIndex = currentIndex - 1;
                }
                else
                {
                    startIndex = currentIndex + 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定页面与左右/上下侧页面到 viewport 中心距离，得到下一次查找的方向。
        /// </summary>
        /// <param name="index">页面索引。</param>
        /// <returns>-1: 左/上侧更小，1: 右/下侧更小，0: 当前最小。</returns>
        private int GetNextSeekDirection(int index)
        {
            float distance = Mathf.Abs(GetPageCenterToViewportCenter(index));

            // 向前比较
            if (index > 0 && Mathf.Abs(GetPageCenterToViewportCenter(index - 1)) < distance)
            {
                return -1;
            }

            // 向后比较
            if (index < scrolltoableLayout.ItemCount - 1
                && Mathf.Abs(GetPageCenterToViewportCenter(index + 1)) < distance)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 获取页面中心到 viewport 中心在滚动方向上的距离（带符号）。
        /// </summary>
        private float GetPageCenterToViewportCenter(int pageIndex)
        {
            Vector2 delta = UILayoutHelper.GetItemCenterToViewportCenter(layoutRectTransform, viewport,
                                                                         scrolltoableLayout, pageIndex);
            return direction == Direction.Horizontal ? delta.x : delta.y;
        }

        /// <summary>
        /// 滚动到上一页。
        /// </summary>
        private void ScrollToPreviousPage()
        {
            int index;
            int viewPage = GetViewportPage();

            float deltaDistance = GetPageCenterToViewportCenter(viewPage);
            int sign = direction == Direction.Horizontal ? 1 : -1;

            if (sign * deltaDistance <= 0)
            {
                index = viewPage - 1;
            }
            else
            {
                index = viewPage;
            }

            ScrollTo(Mathf.Max(index, 0));
        }

        /// <summary>
        /// 滚动到下一页。
        /// </summary>
        private void ScrollToNextPage()
        {
            int index;
            int viewPage = GetViewportPage();
            int sign = direction == Direction.Horizontal ? 1 : -1;

            float deltaDistance = GetPageCenterToViewportCenter(viewPage);

            if (sign * deltaDistance >= 0)
            {
                index = viewPage + 1;
            }
            else
            {
                index = viewPage;
            }

            ScrollTo(Mathf.Min(index, scrolltoableLayout.ItemCount - 1));
        }

        /// <summary>
        /// 滚动到当前屏幕主要显示的页。
        /// </summary>
        private void ScrollToScreenPage()
        {
            int index = GetViewportPage();
            ScrollTo(index);
        }

        //--------------------------------------------------
        // 公有方法
        //--------------------------------------------------

        // 直接设置指定索引的页面到视口
        public void SetTo(int index)
        {
            SetTargetIndex(index);

            if (isDragging)
            {
                return;
            }

            // 当前位置 layout 和 viewport 左上角之间的距离（有方向）
            Vector2 cornerDist = UILayoutHelper.GetLayoutLT2ViewportLT(layoutRectTransform, viewport);

            if (direction == Direction.Horizontal && !Mathf.Approximately(cornerDist.x, targetCornerDistance))
            {
                SetLayoutAnchoredPosition(layoutRectTransform.anchoredPosition
                                          + new Vector2(cornerDist.x - targetCornerDistance, 0));
            }
            else if (direction == Direction.Vertical && !Mathf.Approximately(cornerDist.y, targetCornerDistance))
            {
                SetLayoutAnchoredPosition(layoutRectTransform.anchoredPosition
                                          + new Vector2(0, cornerDist.y - targetCornerDistance));
            }
        }

        public void ScrollTo(int index)
        {
            SetTargetIndex(index);
        }

        //--------------------------------------------------
        // 测试
        //--------------------------------------------------

        [SerializeField]
        private int testIndex;

        [Inspector]
        private void TestSetTo()
        {
            SetTo(testIndex);
        }

        [Inspector]
        private void TestScrollTo()
        {
            ScrollTo(testIndex);
        }
    }
}