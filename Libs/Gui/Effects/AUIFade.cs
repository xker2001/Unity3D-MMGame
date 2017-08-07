using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    abstract public class AUIFade : AUIEffect
    {
        protected CanvasGroup canvasGroup;
        protected Sequence seq;

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            Assert.IsNotNull(canvasGroup);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (seq != null)
            {
                seq.Kill();
                seq = null;
            }
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