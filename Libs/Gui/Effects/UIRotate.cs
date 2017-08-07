using UnityEngine;
using DG.Tweening;
using EasyEditor;

namespace MMGame.UI
{
    public class UIRotate : AUIEffect
    {
        [SerializeField]
        private Vector3 to = new Vector3(0, 0, 360);

        [SerializeField]
        private Ease easeType = Ease.Linear;

        [SerializeField]
        private float duration = 1;

        [SerializeField]
        private bool loop;

        [Visibility("loop", true)]
        [SerializeField]
        private LoopType loopType = LoopType.Incremental;

        [Visibility("loop", true)]
        [SerializeField]
        private int loopTimes = -1;

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

        protected override void InitPlaying() {}

        protected override void PlayEffect()
        {
            if (tw == null)
            {
                tw = transform.DORotate(to, duration, RotateMode.LocalAxisAdd)
                              .SetEase(easeType)
                              .OnComplete(OnComplete)
                              .SetAutoKill(false)
                              .SetUpdate(UpdateType.Normal, true);

                if (loop)
                {
                    tw.SetLoops(loopTimes, loopType);
                }
            }

            tw.Restart();
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