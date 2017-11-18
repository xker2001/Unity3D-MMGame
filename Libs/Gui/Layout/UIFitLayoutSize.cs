using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 根据子控件调整控件大小，必须与 IUISizeFitableLayout 组件同时使用。
    /// 注意，在需要调整的方向上，IUISizeFitableLayout 的锚定方式不得为拉伸。
    /// </summary>
    public class UIFitLayoutSize : EasyUIBehaviour
    {
        [Message(text = "未实现 IUISizeFitableLayout", messageType = MessageType.Error,
            method = "IsNotIUISizeFitableLayout")]
        [SerializeField]
        private MonoBehaviour layout;

        private bool IsNotIUISizeFitableLayout()
        {
            // 必须注入组件/Prefab
            return !this.CheckMustHaveInjection(layout, typeof(IUISizeFitableLayout));
        }

        [Tooltip("忽略调整宽度。")]
        [SerializeField]
        private bool ignoreWidth;

        [Tooltip("忽略调整高度。")]
        [SerializeField]
        private bool ignoreHeight;

        private IUISizeFitableLayout fitableLayout;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(layout);

            fitableLayout = layout as IUISizeFitableLayout;
            Assert.IsNotNull(fitableLayout);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            fitableLayout.SizeChanged += OnFitableSizeChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            fitableLayout.SizeChanged -= OnFitableSizeChanged;
        }

        private void OnFitableSizeChanged(float sizeX, float sizeY)
        {
            if (!ignoreWidth)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeX);
            }

            if (!ignoreHeight)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeY);
            }
        }
    }
}