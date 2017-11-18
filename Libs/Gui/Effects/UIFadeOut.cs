using UnityEngine;
using DG.Tweening;

namespace MMGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFadeOut : AUIFade
    {
        [Range(0, 1)]
        [SerializeField]
        private float startAlpha = 1;

        [Range(0, 1)]
        [SerializeField]
        private float endAlpha = 0;

        [Tooltip("开始淡出前的停留时间。")]
        [SerializeField]
        private float showDuration;

        [Tooltip("淡出动画的时长。")]
        [SerializeField]
        private float fadeOutDuration;

        [SerializeField]
        private Ease easeType = Ease.Linear;


        protected override void PreparePlaying()
        {
            canvasGroup.alpha = startAlpha;
        }

        protected override void PlayEffect()
        {
            if (seq == null)
            {
                seq = DOTween.Sequence()
                             .AppendInterval(showDuration)
                             .Append(canvasGroup.DOFade(endAlpha, fadeOutDuration).SetEase(easeType))
                             .OnComplete(SetSelfComplete)
                             .SetAutoKill(false)
                             .SetUpdate(UpdateType.Normal, true);
            }

            seq.Restart();
        }
    }
}