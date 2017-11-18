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

        private RectTransform rectTransform;
        private Tweener tw;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = Target.GetComponent<RectTransform>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (tw != null)
            {
                tw.Kill();
                tw = null;
            }
        }

        protected override void PreparePlaying() {}

        protected override void PlayEffect()
        {
            rectTransform.localScale = new Vector3(@from.x, @from.y, 1);

            if (tw == null)
            {
                tw = rectTransform.DOScale(new Vector3(to.x, to.y, 1), duration)
                                  .SetEase(easeType)
                                  .OnComplete(SetSelfComplete)
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