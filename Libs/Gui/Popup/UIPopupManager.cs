using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 模态弹出面板专用辅助 Canvas，提供弹出面板父节点及模态遮罩支持。
    /// 弹出面板的 localScale 应当设置为 1。
    /// </summary>
    public class UIPopupManager : MonoBehaviour
    {
        private static UIPopupManager instance;

        private static UIPopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (UIPopupManager) FindObjectOfType(typeof(UIPopupManager));
                }

                return instance;
            }
        }

        /// <summary>
        /// 遮挡用 Canvas 的渲染顺序，设置为较大值以覆盖其他界面（场景过渡遮罩除外）。
        /// </summary>
        [SerializeField]
        private int sortOrder = 8888;

        /// <summary>
        /// 遮罩默认颜色。
        /// 遮罩是否使用默认颜色在 AUIPopup 组件中设定。如果使用，AUIPopup 会从这里取走
        /// 默认颜色并作为参数传递回 OpenPopup 方法中。
        /// </summary>
        [SerializeField]
        private Color defaultOverlayColor = Color.black;

        /// <summary>
        /// 遮罩淡入时长。
        /// </summary>
        [SerializeField]
        private float fadeInDuration = 0.5f;

        /// <summary>
        /// 遮罩淡出时长。
        /// </summary>
        [SerializeField]
        private float fadeOutDuration = 0.2f;

        /// <summary>
        /// 从开始关闭窗口到销毁（回收）窗口的延时，用于等待关闭动画播放完毕。
        /// </summary>
        [SerializeField]
        private float defaultDelayOfDestoryPopup = 0.6f;

        private static GameObject canvasObj;
        private static GameObject overlayObj;
        private static Image overlayImage;
        private static Button overlayButton;

        private const string popupPoolName = "UIPopups";

        private void Awake()
        {
            // 创建一个专用的 Canvas
            canvasObj = new GameObject("UIPopupCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 1;
            var graphicRaycaster = canvasObj.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            DontDestroyOnLoad(canvasObj);
            // 创建一个模态窗口遮罩
            overlayObj = new GameObject("OverlayImage");
            overlayObj.transform.SetParent(canvasObj.transform, false);
            overlayImage = overlayObj.AddComponent<Image>();
            overlayImage.rectTransform.anchorMin = Vector2.zero;
            overlayImage.rectTransform.anchorMax = Vector2.one;
            overlayImage.rectTransform.sizeDelta = new Vector2(10, 10);
            overlayButton = overlayObj.AddComponent<Button>();
            overlayButton.transition = Selectable.Transition.None;
            overlayObj.SetActive(false);
        }

        public static Color DefaultOverlayColor
        {
            get { return Instance.defaultOverlayColor; }
        }

        public static float DefaultDelayOfDestoryPopup
        {
            get { return Instance.defaultDelayOfDestoryPopup; }
        }

        private static void DeactivateOverlay()
        {
            overlayImage.CrossFadeAlpha(0f, Instance.fadeOutDuration, true);
        }

        private static void ActivateOverlay(Color overlayColor)
        {
            overlayObj.SetActive(true);
            overlayImage.color = overlayColor;
            overlayImage.canvasRenderer.SetAlpha(0);
            overlayObj.transform.SetAsLastSibling();
            overlayImage.CrossFadeAlpha(1f, Instance.fadeInDuration, true);
        }

        /// <summary>
        /// 根据给定的 prefab 打开弹出面板，由 AUIpopupOpener 组件调用，或由代码直接调用。
        /// </summary>
        /// <param name="popupPrefab">弹出面板 prefab。</param>
        /// <param name="data">传递给弹出面板的数据。</param>
        /// <param name="overlayColor">遮罩颜色。</param>
        /// <param name="tapToClose">是否可以随意位置点击关闭弹出面板。</param>
        /// <returns>弹出面板。</returns>
        public static AUIPopup OpenPopupPrefab(AUIPopup popupPrefab, object data,
                                               Color overlayColor, bool tapToClose = false)
        {
            Transform pupupXform = PoolManager.Spawn(popupPoolName, popupPrefab.transform);
            var popup = pupupXform.GetComponent<AUIPopup>();
            Assert.IsNotNull(popup, "Can not find AUIPopup component.");
            popup.IsPoolItem = true;
            OpenPopup(popup, data, overlayColor, tapToClose);
            return popup;
        }

        /// <summary>
        /// 直接打开场景中预置的弹出面板，由 AUIpopupOpener 调用，或由代码直接调用。
        /// </summary>
        /// <param name="popup">场景中预置的弹出面板。</param>
        /// <param name="data">传递给弹出面板的数据。</param>
        /// <param name="overlayColor">遮罩颜色。</param>
        /// <param name="tapToClose">是否可以随意位置点击关闭弹出面板。</param>
        public static void OpenPopup(AUIPopup popup, object data,
                                     Color overlayColor, bool tapToClose = false)
        {
            ActivateOverlay(overlayColor);

            if (tapToClose)
            {
                overlayButton.onClick.AddListener(popup.Close);
            }
            else
            {
                overlayButton.onClick.RemoveAllListeners();
            }

            popup.Parent = popup.transform.parent;
            popup.transform.SetParent(canvasObj.transform, false);
            popup.transform.SetAsLastSibling();
            popup.SetData(data);
            popup.gameObject.SetActive(true);
            popup.OnOpen();
        }

        /// <summary>
        /// 关闭/回收弹出面板，由 Close 按钮调用，或由代码调用，（或由 tap 即关闭的遮罩按钮调用）。
        /// </summary>
        /// <param name="popup"></param>
        public static void ClosePopup(AUIPopup popup, float delayOfDestroyPopup)
        {
            DeactivateOverlay();
            Instance.StartCoroutine(Instance.DeactivatePopup(popup, delayOfDestroyPopup));
        }

        private IEnumerator DeactivatePopup(AUIPopup popup, float delayOfDestroyPopup)
        {
            if (delayOfDestroyPopup > Mathf.Epsilon)
            {
                yield return new WaitForSeconds(delayOfDestroyPopup);
            }

            if (popup.IsPoolItem)
            {
                PoolManager.Despawn(popup.transform);
            }
            else
            {
                // 在 Unity 2017.2 中，以下两句如果交换顺序会导致 crash
                popup.gameObject.SetActive(false);
                popup.transform.SetParent(popup.Parent, false);
            }

            overlayObj.SetActive(false);
        }
    }
}