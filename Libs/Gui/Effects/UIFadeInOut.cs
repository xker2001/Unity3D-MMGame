using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace MMGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFadeInOut : AUIFade
    {
        [Range(0, 1)]
        [SerializeField]
        private float startAlpha = 0;

        [Range(0, 1)]
        [SerializeField]
        private float endAlpha = 1;

        [Tooltip("淡入动画的时长。")]
        [SerializeField]
        private float fadeInDuration;

        [SerializeField]
        private Ease fadeInEaseType = Ease.Linear;

        [Tooltip("淡入完成到开始淡出的停留时间。")]
        [SerializeField]
        private float showDuration;

        [Tooltip("淡出动画的时长。")]
        [SerializeField]
        private float fadeOutDuration;

        [SerializeField]
        private Ease fadeOutEaseType = Ease.Linear;

        [Tooltip("淡入动画完成后执行的回调函数。")]
        [SerializeField]
        private UnityEvent onFadeInComplete;

        protected override void InitPlaying()
        {
            canvasGroup.alpha = startAlpha;
        }

        protected override void PlayEffect()
        {
            if (seq == null)
            {
                seq = DOTween.Sequence()
                             .Append(canvasGroup.DOFade(endAlpha, fadeInDuration).SetEase(fadeInEaseType))
                             .AppendCallback(OnFadeInComplete)
                             .AppendInterval(showDuration)
                             .Append(canvasGroup.DOFade(startAlpha, fadeOutDuration).SetEase(fadeOutEaseType))
                             .OnComplete(OnComplete)
                             .SetAutoKill(false)
                             .SetUpdate(UpdateType.Normal, true);
            }

            seq.Restart();
        }

        private void OnFadeInComplete()
        {
            onFadeInComplete.Invoke();
        }
    }
}