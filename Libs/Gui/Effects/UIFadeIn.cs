using UnityEngine;
using DG.Tweening;

namespace MMGame.UI
{
    public class UIFadeIn : AUIFade
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

        [Tooltip("淡入完成到开始执行 OnComplete() 的停留时间。")]
        [SerializeField]
        private float showDuration;

        [SerializeField]
        private Ease easeType = Ease.Linear;

        protected override void InitPlaying()
        {
            canvasGroup.alpha = startAlpha;
        }

        protected override void PlayEffect()
        {
            if (seq == null)
            {
                seq = DOTween.Sequence()
                             .Append(canvasGroup.DOFade(endAlpha, fadeInDuration).SetEase(easeType))
                             .AppendInterval(showDuration)
                             .OnComplete(OnComplete)
                             .SetAutoKill(false)
                             .SetUpdate(UpdateType.Normal, true);
            }

            seq.Restart();
        }
    }
}