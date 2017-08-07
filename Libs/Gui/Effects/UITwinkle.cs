using UnityEngine;
using DG.Tweening;
using EasyEditor;

namespace MMGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITwinkle : AUIFade
    {
        [Range(0, 1)]
        [SerializeField]
        private float startAlpha = 1;

        [Range(0, 1)]
        [SerializeField]
        private float endAlpha = 0;

        [SerializeField]
        private float showDuration = 0.5f;

        [SerializeField]
        private float hideDuration = 0.5f;


        [Range(0, 1)]
        [SerializeField]
        private float fadeInDurationPercent = 0.5f;

        [SerializeField]
        private Ease fadeInEaseType = Ease.Linear;

        [Range(0, 1)]
        [SerializeField]
        private float fadeOutDurationPercent = 0.5f;

        [SerializeField]
        private Ease fadeOutEaseType = Ease.Linear;


        [SerializeField]
        private bool loop;

        [Visibility("loop", true)]
        [SerializeField]
        private LoopType loopType = LoopType.Restart;

        [Visibility("loop", true)]
        [SerializeField]
        private int loopTimes = -1;

        protected override void InitPlaying()
        {
            canvasGroup.alpha = endAlpha;
        }

        protected override void PlayEffect()
        {
            if (seq == null)
            {
                seq = DOTween.Sequence();

                float fadeInDuration = showDuration * fadeInDurationPercent;

                if (fadeInDuration > Mathf.Epsilon)
                {
                    seq.Append(canvasGroup.DOFade(startAlpha, fadeInDuration)
                                          .SetEase(fadeInEaseType));
                    seq.AppendInterval(showDuration - fadeInDuration);
                }

                float fadeOutDuration = hideDuration * fadeOutDurationPercent;

                if (fadeOutDuration > Mathf.Epsilon)
                {
                    seq.Append(canvasGroup.DOFade(endAlpha, fadeOutDuration)
                                          .SetEase(fadeOutEaseType));
                    seq.AppendInterval(hideDuration - fadeOutDuration);
                }

                if (loop)
                {
                    seq.SetLoops(loopTimes, loopType);
                }

                seq.OnComplete(OnComplete)
                   .SetAutoKill(false)
                   .SetUpdate(UpdateType.Normal, true);
            }

            seq.Restart();
        }
    }
}