using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 打开弹出面板时
    /// </summary>
    abstract public class AUIPopup : MonoBehaviour
    {
        /// <summary>
        /// 弹出面板动画特效，指定为以下名称：
        /// - Open
        /// - Close
        /// </summary>
        [SerializeField]
        private Animator animator;

        /// <summary>
        /// 基于 MMGame.AUIEffect 的关闭动画效果。
        /// </summary>
        [SerializeField]
        private AUIEffect closeEffect;

        /// <summary>
        /// 是否使用 UIPopupManager 设置的默认遮罩颜色。
        /// </summary>
        [SerializeField]
        private bool useDefaultDelayOfDestoryPopup = true;

        /// <summary>
        /// 自定义从按下按钮到销毁弹出面板的延迟时间。。
        /// </summary>
        [SerializeField]
        [HideIf("useDefaultDelayOfDestoryPopup")]
        private float delayOfDestoryPopup = 0.6f;

        /// <summary>
        /// 弹出面板被激活时调用，主要用于额外的弹出特效逻辑。
        /// 该方法通常留空即可，弹出特效通常由动画或特效组件的 OnEnable 自行激活。
        /// </summary>
        virtual public void OnOpen() {}

        /// <summary>
        /// 弹出面板被关闭时调用，主要用于额外的关闭特效逻辑。
        /// 该方法通常留空即可，关闭特效通常由 Close 方法中处理 Animator 和 CloseEffect 实现。
        /// </summary>
        virtual protected void OnClose() {}

        /// <summary>
        /// 关闭当前弹出面板，用于挂接到关闭按钮的 onClick。
        /// </summary>
        public void Close()
        {
            if (animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                animator.Play("Close");
            }

            if (closeEffect)
            {
                closeEffect.Play();
            }

            // 防止被 close 特效修改了缩放值
            if (!animator && !closeEffect)
            {
                transform.localScale = Vector3.one;
            }

            OnClose();
            UIPopupManager.ClosePopup(this, useDefaultDelayOfDestoryPopup
                                                ? UIPopupManager.DefaultDelayOfDestoryPopup
                                                : delayOfDestoryPopup);
        }

        /// <summary>
        /// 设置本次 po pup 的数据。注意此时弹出面板尚处于未激活状态。
        /// </summary>
        /// <param name="data">数据对象。</param>
        virtual public void SetData(object data) {}

        /// <summary>
        /// 弹出窗口是否需要回收的对象池物体。
        /// </summary>
        public bool IsPoolItem { get; set; }

        /// <summary>
        /// 原始的父节点。当弹出窗口不是对象池物体而是预置在场景中时，打开
        /// 弹出窗口会将弹出窗口移动到 UIPopupCanvas 节点上。当关闭弹出
        /// 窗口时需要重新移回到 Parent 节点。
        /// </summary>
        public Transform Parent { get; set; }
    }
}