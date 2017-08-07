using System.Collections;
using MMGame.Level;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.SimpleLevelManager
{
    /// <summary>
    /// 简单的加载进度控制器。
    ///
    /// 说明：
    /// 1. 加载移动动画控件随黑幕淡入淡出。
    /// 2. 随着加载进行，移动动画控件从起始锚点移动到终点锚点。
    /// 3. 移动动画控件的移动有速度限制（最大速度）。
    /// </summary>
    public class SimpleAsyncer : AAsyncProcessor
    {
        /// <summary>
        /// 过渡黑屏。
        /// </summary>
        [SerializeField]
        private CanvasGroup canvasGroup;

        /// <summary>
        /// 移动动画控件，通常是一个带 Animator 的 Image 组件
        /// </summary>
        [SerializeField]
        private Transform mover;

        /// <summary>
        /// 移动的起始锚点。
        /// </summary>
        [SerializeField]
        private Transform startAnchor;

        /// <summary>
        /// 移动的终点锚点。
        /// </summary>
        [SerializeField]
        private Transform endAnchor;

        /// <summary>
        /// 移动速度。
        /// </summary>
        [SerializeField]
        private float speed = 0.5f;

        private float currentPosition; // 映射到 0 ~ 1 区间的当前位置
        private float currentProgress; // 0 ~ 1

        public override bool AllowLevelActivation { get; set; }

        void Awake()
        {
            // 隐藏掉黑屏，用来保证没有 inFader 的情况下黑屏会被关掉。
            // 如果有 inFader，inFader 会及时再次打开黑屏
            LevelTransition.LoadLevelCompleted += ReleaseSources;
        }

        void OnDestroy()
        {
            LevelTransition.LoadLevelCompleted -= ReleaseSources;
        }

        public override void StartProgress(ALevelMap map)
        {
            Assert.IsNotNull(canvasGroup);
            Assert.IsNotNull(mover);
            Assert.IsNotNull(startAnchor);
            Assert.IsNotNull(endAnchor);

            // 确保显示黑屏
            if (!canvasGroup.gameObject.activeSelf)
            {
                canvasGroup.gameObject.SetActive(true);
            }

            canvasGroup.alpha = 1;

            // 必须等到动画控件移动完成后才能激活关卡
            AllowLevelActivation = false;

            mover.position = startAnchor.position;
            currentPosition = 0;
            currentProgress = 0;

            // 确保显示动画控件
            if (!mover.gameObject.activeSelf)
            {
                mover.gameObject.SetActive(true);
            }

            StartCoroutine(UpdateMover());
        }

        public override void UpdateProgress(float progress, ALevelMap map)
        {
            currentProgress = progress / 0.9f;
        }

        public override void PromptToActivate(ALevelMap map)
        {
        }

        private IEnumerator UpdateMover()
        {
            while (true)
            {
                // 移动完成后激活关卡
                if (Mathf.Approximately(currentPosition, 1))
                {
                    AllowLevelActivation = true;
                    yield break;
                }

                // 向当前目标位置移动
                currentPosition += speed * Time.deltaTime;
                currentPosition = Mathf.Min(currentPosition, currentProgress);
                mover.position = Vector3.Lerp(startAnchor.position, endAnchor.position, currentPosition);
                yield return null;
            }
        }

        // 释放/还原使用的资源
        private void ReleaseSources(ALevelMap map, LoadMode mode)
        {
            canvasGroup.gameObject.SetActive(false);
            mover.gameObject.SetActive(false);
        }
    }
}