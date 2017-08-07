using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// 根据子控件调整控件大小，必须与 AUILayout 组件同时使用。
    /// 注意，在需要调整的方向上，AUILayout 的锚定方式不得为拉伸。
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

            fitableLayout.SizeChanged += OnFitableSizeChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            fitableLayout.SizeChanged -= OnFitableSizeChanged;
        }

        private void OnFitableSizeChanged(Vector2 sizeDelta)
        {
            float sizeX = ignoreWidth ? rectTransform.sizeDelta.x : sizeDelta.x;
            float sizeY = ignoreHeight ? rectTransform.sizeDelta.y : sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}