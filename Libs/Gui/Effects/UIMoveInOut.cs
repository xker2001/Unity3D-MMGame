using DG.Tweening;
using UnityEngine;

namespace MMGame.UI
{
    public class UIMoveInOut : AUIMove
    {
        [SerializeField]
        private OutScreenAnchor fromAnchor;

        [SerializeField]
        private Ease inEaseType = Ease.Linear;

        [SerializeField]
        private float inDuration = 1;

        [Tooltip("飞入后飞出前的停留时间。")]
        [SerializeField]
        private float showDuration;

        [SerializeField]
        private OutScreenAnchor toAnchor;

        [SerializeField]
        private Ease outEaseType = Ease.Linear;

        [SerializeField]
        private float outDuration = 1;

        private Vector3 from;
        private Vector3 to;
        private Vector3 centre;
        private Sequence seq;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (seq != null)
            {
                seq.Kill();
                seq = null;
            }
        }

        protected override void PreparePlaying()
        {
            @from = GetOutScreenPosition(fromAnchor, rectTransform.position);
            to = GetOutScreenPosition(toAnchor, rectTransform.position);
            centre = GetOriginalPosition();
            rectTransform.position = @from;
        }

        protected override void PlayEffect()
        {
            if (seq != null)
            {
                seq.Kill();
            }

            seq = DOTween.Sequence()
                         .Append(rectTransform.DOMove(centre, inDuration).SetEase(inEaseType))
                         .AppendInterval(showDuration)
                         .Append(rectTransform.DOMove(to, outDuration).SetEase(outEaseType))
                         .OnComplete(SetSelfComplete)
                         .SetAutoKill(false)
                         .SetUpdate(UpdateType.Normal, true);
            seq.Play();
        }

        protected override void StopEffect()
        {
            if (seq != null)
            {
                seq.Pause();
            }
        }
    }
}