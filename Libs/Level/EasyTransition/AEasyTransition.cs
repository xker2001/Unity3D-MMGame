using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMGame.Level
{
    /// <summary>
    /// 轻量级场景淡入淡出切换控制器。
    /// 
    /// - 支持通过加载新 scene 切换场景。
    /// - 支持通过 disable/enable 场景节点切换屏幕画面。
    /// 
    /// 为每个/每种场景过渡创建一个 EasyTransition（的派生）并挂接在场景中。
    /// 如果需要侦听场景切换事件并处理，则编写相应的组件配合使用。
    /// 
    /// </summary>
    abstract public class AEasyTransition : MonoBehaviour
    {
        /// <summary>
        /// 淡入淡出总时长。
        /// </summary>
        [SerializeField]
        private float duration = 1f;

        /// <summary>
        /// 淡入后中间停顿时长。
        /// </summary>
        [SerializeField]
        private float overlayDuration = 0.5f;

        /// <summary>
        /// 幕布的颜色。
        /// </summary>
        [SerializeField]
        private Color color = Color.black;

        public event Action StartTransition;
        public event Action StartSwitch;
        public event Action EndSwitch;
        public event Action EndTransition;

        public void Transit()
        {
            StartCoroutine(FadeInOut());
        }

        /// <summary>
        /// 执行场景切换操作。
        /// </summary>
        abstract protected void SwitchScene();

        private IEnumerator FadeInOut()
        {
            if (StartTransition != null)
            {
                StartTransition();
            }

            EasyTransitionCanvas.ActivateOverlay(color);

            float time = 0f;
            float halfDuration = duration * 0.5f;

            while (time < halfDuration)
            {
                time += Time.deltaTime;
                EasyTransitionCanvas.SetOverlayAlpha(Mathf.InverseLerp(0, 1, time / halfDuration));
                yield return new WaitForEndOfFrame();
            }

            EasyTransitionCanvas.SetOverlayAlpha(1);
            yield return new WaitForEndOfFrame();

            if (StartSwitch != null)
            {
                StartSwitch();
            }

            SwitchScene();

            if (EndSwitch != null)
            {
                EndSwitch();
            }

            if (overlayDuration > Mathf.Epsilon)
            {
                yield return new WaitForSeconds(overlayDuration);
            }

            time = 0f;

            while (time < halfDuration)
            {
                time += Time.deltaTime;
                EasyTransitionCanvas.SetOverlayAlpha(Mathf.InverseLerp(1, 0, time / halfDuration));
                yield return new WaitForEndOfFrame();
            }

            EasyTransitionCanvas.SetOverlayAlpha(0);
            yield return new WaitForEndOfFrame();

            EasyTransitionCanvas.DeactivateOverlay();

            if (EndTransition != null)
            {
                EndTransition();
            }
        }
    }
}