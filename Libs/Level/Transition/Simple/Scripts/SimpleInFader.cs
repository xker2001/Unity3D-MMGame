using System;
using DG.Tweening;
using MMGame.Level;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.SimpleLevelManager
{
    public class SimpleInFader : ASceneFadeInProcessor
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private float duration = 1;

        [SerializeField]
        private Ease easyType = Ease.Linear;

        private Action<ALevelMap> onCompleted;
        private ALevelMap map;
        private Tweener tw;

        void OnDestroy()
        {
            if (tw != null)
            {
                tw.Kill();
                tw = null;
            }
        }

        public override void InitFadeIn(ALevelMap map)
        {
            Assert.IsNotNull(canvasGroup);

            if (!canvasGroup.gameObject.activeSelf)
            {
                canvasGroup.gameObject.SetActive(true);
            }

            canvasGroup.alpha = 1;
        }

        public override void FadeIn(ALevelMap map, Action<ALevelMap> onCompleted)
        {
            this.onCompleted = onCompleted;
            this.map = map;

            if (tw == null)
            {
                tw = canvasGroup.DOFade(0, duration)
                                .SetEase(easyType)
                                .SetAutoKill(false)
                                .SetUpdate(UpdateType.Normal, true)
                                .OnComplete(OnComplete);
            }

            tw.Restart();
        }

        private void OnComplete()
        {
            canvasGroup.gameObject.SetActive(false);
            onCompleted(map);
        }
    }
}