using UnityEngine;
using UnityEngine.UI;

namespace MMGame.Level
{
    /// <summary>
    /// 轻量级场景淡入淡出切换 Canvas 生成器。
    /// 作为基础服务挂接在场景中。
    /// </summary>
    public class EasyTransitionCanvas : MonoBehaviour
    {
        /// <summary>
        /// 遮挡用 Canvas 的渲染顺序，设置为较大值以覆盖所有其他界面。
        /// </summary>
        [SerializeField]
        private int sortOrder = 9999;

        /// <summary>
        /// 幕布 Canvas GameObject。
        /// </summary>
        private static GameObject canvasObj;

        /// <summary>
        /// 幕布 GameObject。
        /// </summary>
        private static GameObject overlayObj;

        /// <summary>
        /// 幕布 Image 组件。
        /// </summary>
        private static Image overlayImage;

        private void Awake()
        {
            // 创建一个专用的 Canvas
            canvasObj = new GameObject("SceneTransitionCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            DontDestroyOnLoad(canvasObj);
            // 创建一个遮蔽用的 ImageButton
            overlayObj = new GameObject("OverlayImage");
            overlayObj.transform.SetParent(canvasObj.transform, false);
            overlayImage = overlayObj.AddComponent<Image>();
            // 设置为遮蔽整个屏幕。
            overlayImage.rectTransform.anchorMin = Vector2.zero;
            overlayImage.rectTransform.anchorMax = Vector2.one;
            overlayImage.rectTransform.sizeDelta = new Vector2(10, 10);
            canvasObj.SetActive(false);
        }

        public static void DeactivateOverlay()
        {
            canvasObj.SetActive(false);
        }

        public static void ActivateOverlay(Color color)
        {
            canvasObj.SetActive(true);
            overlayImage.color = color;
            overlayImage.canvasRenderer.SetAlpha(0);
        }

        public static void SetOverlayAlpha(float alpha)
        {
            overlayImage.canvasRenderer.SetAlpha(alpha);
        }
    }
}