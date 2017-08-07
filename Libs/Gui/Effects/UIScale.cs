using UnityEngine;
using DG.Tweening;
using EasyEditor;

namespace MMGame.UI
{
    public class UIScale : AUIEffect
    {
        [SerializeField]
        private Vector2 @from = new Vector2(1, 1);

        [SerializeField]
        private Vector2 to = new Vector2(2, 2);

        [SerializeField]
        private Ease easeType = Ease.Linear;

        [SerializeField]
        private float duration = 1;

        [SerializeField]
        private bool loop;

        [Visibility("loop", true)]
        [SerializeField]
        private LoopType loopType = LoopType.Yoyo;

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
            transform.localScale = new Vector3(@from.x, @from.y, 1);

            if (tw == null)
            {
                tw = transform.DOScale(new Vector3(to.x, to.y, 1), duration)
                              .SetEase(easeType)
                              .OnComplete(OnComplete)
                              .SetAutoKill(false)
                              .SetUpdate(UpdateType.Normal, true);

                if (loop)
                {
                    tw.SetLoops(loopTimes * 2, loopType);
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