using UnityEngine;
using DG.Tweening;

namespace MMGame.UI
{
    abstract public class AUISingleMove : AUIMove
    {
        [SerializeField]
        private Ease easeType = Ease.Linear;

        [SerializeField]
        private float duration = 2;

        protected Vector3 toPosition;
        private Tweener tw;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (tw != null)
            {
                tw.Kill();
                tw = null;
            }
        }

        protected override void PlayEffect()
        {
            if (tw != null)
            {
                tw.Kill();
            }

            tw = Target.transform.DOMove(toPosition, duration)
                       .SetEase(easeType)
                       .OnComplete(SetSelfComplete)
                       .SetAutoKill(false)
                       .SetUpdate(UpdateType.Normal, true);
            tw.Play();
        }

        protected override void StopEffect()
        {
            if (tw != null)
            {
                tw.Pause();
            }
        }
    }
}