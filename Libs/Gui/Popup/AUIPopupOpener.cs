using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 用于打开弹出面板的组件。
    /// 将组件添加到打开按钮，并将 OpenPopup 或 OpenPopupPrefab 方法挂接到 onClick。
    /// </summary>
    abstract public class AUIPopupOpener : MonoBehaviour
    {
        [Tooltip("预置在场景中的 popup 面板或 popup 面板 prefab。")]
        [InfoBox("预置在场景中的 popup 面板或 popup 面板 prefab。")]
        [SerializeField, Required]
        private AUIPopup popup;

        /// <summary>
        /// 是否使用 UIPopupManager 设置的默认遮罩颜色。
        /// </summary>
        [SerializeField]
        private bool useDefaultOverlayColor = true;

        /// <summary>
        /// 自定义的遮罩颜色。
        /// </summary>
        [HideIf("useDefaultOverlayColor")]
        [SerializeField]
        private Color overlayColor = Color.black;

        /// <summary>
        /// 是否可以点击屏幕任意位置关闭弹出窗口。
        /// </summary>
        [SerializeField]
        private bool tapToClose;

        /// <summary>
        /// 打开场景中预置的弹出面板，用于挂接到打开按钮的 onClick。
        /// </summary>
        public void OpenPopup()
        {
            PrepareData();
            Color color = useDefaultOverlayColor ? UIPopupManager.DefaultOverlayColor : overlayColor;
            UIPopupManager.OpenPopup(popup, PrepareData(), color, tapToClose);
        }

        /// <summary>
        /// 打开弹出面板 prefab，用于挂接到打开按钮的 onClick。
        /// </summary>
        public void OpenPopupPrefab()
        {
            PrepareData();
            Color color = useDefaultOverlayColor ? UIPopupManager.DefaultOverlayColor : overlayColor;
            UIPopupManager.OpenPopupPrefab(popup, PrepareData(), color, tapToClose);
        }

        /// <summary>
        /// 准备弹出面板需要的数据。
        /// </summary>
        virtual protected object PrepareData()
        {
            return null;
        }
    }
}