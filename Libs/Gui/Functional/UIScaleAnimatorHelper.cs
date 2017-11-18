using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 在 enable 时设置 localScale 为 0，在 disable 时设置 localScale 为 1。
    /// 实际值可以在 Inspector 中设置。
    /// 该组件用于辅助通过 Animator + Animation 实现激活时缩放的情形。
    /// 
    /// Disable 设置的来源场合：
    /// Layout 中的 item 会被缩放以调整大小，此时播放基于绝对值缩放的动画会出现问题，
    /// 解决方法为在 item 顶层控件和内容控件之间加一个 localScale 为 1 的容器控件，
    /// Layout 对顶层控件进行缩放以调整大小，Animator 对容器控件执行动画缩放。使用
    /// 这种方法，如果容器控件在 scale 非 1 时被 disable，再次 enable 时会保持这种
    /// 错误的缩放，因此我们需要有一个组件将其重设为 1。
    /// 
    /// Enable 设置的来源场合：
    /// 在上述情形下，一个实践中当容器控件被激活时始终会闪现一次再从 0 开始放大，似乎
    /// Animator 会延迟一帧才开始播放。因此在 OnEnalbe() 中将 localScale 设置为 0，
    /// 用来消除闪现的问题。
    /// 注意，无论该组件位置在 Animator 之前还是之后，Animator 总是先抢下最初的缩放值，
    /// 也可能是跟组件的添加顺序有关而不是组件在 Inspector 中的顺序，有待测试。
    /// 
    /// 一个范例 item：
    /// - AlbumThumb (scale 被 Layout 调整为 0.53，带 UIAnimatedButton 组件)
    ///   - RelativeAnimatior (scale 为 1 的容器控件，带 Animator 组件)
    ///     - Background (scale 为 0.1 的九宫sprite)
    ///     - Icon
    /// 
    /// </summary>
    public class UIScaleAnimatorHelper : EasyUIBehaviour
    {
        [SerializeField]
        private Vector3 onEnableScale = Vector3.zero;

        [SerializeField]
        private Vector3 onDisableScale = Vector3.one;

        protected override void OnEnable()
        {
            base.OnEnable();
            rectTransform.localScale = onEnableScale;
        }

        // 不可以放在 OnEnable 里，因为 Animatior 可能抢先记录下 localScale，
        // 然后执行缩放动画，然后将缩放重置回记录下来的值。
        protected override void OnDisable()
        {
            base.OnDisable();
            rectTransform.localScale = onDisableScale;
        }
    }
}