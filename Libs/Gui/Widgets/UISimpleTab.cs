using UnityEngine;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 简单的标签控件。
    ///
    /// 使用说明（典型范例）：
    /// 1. 创建整体容器控件。
    /// 2. 创建标签容器控件，可添加布局器。
    /// 3. 创建内容容器控件。
    /// 4. 在内容容器控件中为每个标签创建内容。
    /// 5. 在标签容器中创建 Button 并添加 UISimpleTab 组件。
    /// 6. 在 Button 下创建标签显示子控件（两种状态）。
    /// 7. 将显示子控件、Button 组件、内容控件指到 UISimpleTab 组件的相应字段。
    /// </summary>
    public class UISimpleTab : EasyUIBehaviour
    {
        [Tooltip("标签激活时显示。")]
        [SerializeField]
        private GameObject enableImage;

        [Tooltip("标签禁用时显示。")]
        [SerializeField]
        private GameObject disableImage;

        [Tooltip("标签按钮，可以将其 Image 设为透明或删除。")]
        [SerializeField]
        private Button tabButton;

        [Tooltip("标签内容。")]
        [SerializeField]
        private GameObject content;

        [Tooltip("是否默认激活。")]
        [SerializeField]
        private bool isDefault;

        protected override void Awake()
        {
            base.Awake();
            tabButton.onClick.AddListener(Enable);
        }

        protected override void OnEnable()
        {
            base.Awake();

            if (isDefault)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        /// <summary>
        /// 激活标签。
        /// </summary>
        public void Enable()
        {
            if (enableImage)
            {
                enableImage.SetActive(true);
            }

            if (disableImage)
            {
                disableImage.SetActive(false);
            }

            if (content)
            {
                content.SetActive(true);
            }

            DisableOthers();
        }

        /// <summary>
        /// 禁用标签。
        /// </summary>
        public void Disable()
        {
            if (enableImage)
            {
                enableImage.SetActive(false);
            }

            if (disableImage)
            {
                disableImage.SetActive(true);
            }

            if (content)
            {
                content.SetActive(false);
            }
        }

        /// <summary>
        /// 禁用所有其他标签。
        /// </summary>
        private void DisableOthers()
        {
            UISimpleTab[] tabs = transform.parent.gameObject.GetComponentsInChildren<UISimpleTab>();

            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i] != this)
                {
                    tabs[i].Disable();
                }
            }
        }
    }
}