using System;
using DG.Tweening;
using MMGame.Level;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.SimpleLevelManager
{
    public class SimpleOutFader : ASceneFadeOutProcessor
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

        void Awake()
        {
            // 隐藏掉黑屏，用来保证没有 asyncer 和 inFader 的情况下黑屏会被关掉。
            // 如果有 inFader，inFader 会及时再次打开黑屏
            LevelTransition.LoadLevelCompleted += ReleaseSources;
        }

        void OnDestroy()
        {
            if (tw != null)
            {
                tw.Kill();
                tw = null;
            }

            LevelTransition.LoadLevelCompleted -= ReleaseSources;
        }

        public override void InitFadeOut(ALevelMap map)
        {
            Assert.IsNotNull(canvasGroup);

            if (!canvasGroup.gameObject.activeSelf)
            {
                canvasGroup.gameObject.SetActive(true);
            }

            canvasGroup.alpha = 0;
        }

        public override void FadeOut(ALevelMap nextMap, Action<ALevelMap> onCompleted)
        {
            this.onCompleted = onCompleted;
            this.map = nextMap;

            if (tw == null)
            {
                tw = canvasGroup.DOFade(1, duration)
                                .SetEase(easyType)
                                .SetAutoKill(false)
                                .SetUpdate(UpdateType.Normal, true)
                                .OnComplete(OnComplete);
            }

            tw.Restart();
        }

        private void OnComplete()
        {
            onCompleted(map);
        }

        // 释放/还原使用的资源
        private void ReleaseSources(ALevelMap map, LoadMode mode)
        {
            canvasGroup.gameObject.SetActive(false);
        }
    }
}