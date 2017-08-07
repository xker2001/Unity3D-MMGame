using UnityEngine;
using System.Collections;
using DG.Tweening;
using EasyEditor;
using MMGame.EffectFactory;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 计时控件。
    /// 注意该控件可能在暂停（timeScale = 0）的状况下运行，所以不用使用 Invoke 等会受影响的函数。
    /// </summary>
    public class UINumericCountdown : EasyUIBehaviour
    {
        [SerializeField]
        private bool playOnEnable;

        [SerializeField]
        private float delay; // default delay to start counting down

        [SerializeField]
        private Text numberText;

        [SerializeField]
        private int startNumber = 3;

        [SerializeField]
        private int endNumber = 1;

        [SerializeField]
        private float startScale = 0;

        [SerializeField]
        private float endScale = 1;

        [Tooltip("数字缩放的时间。")]
        [SerializeField]
        private float easeDuration = 0.5f;

        [SerializeField]
        private Ease easeType = Ease.Linear;

        [Tooltip("数字消失的时间。")]
        [Comment("计时间隔 = easeDuration + interval")]
        [SerializeField]
        private float interval = 0;

        [Tooltip("是否保持显示第一个数字。")]
        [SerializeField]
        private bool keepFirstNumber;

        [Tooltip("是否保持显示最后一个数字。")]
        [SerializeField]
        private bool keepLastNumber;

        [SerializeField]
        private SoundParamFactory countingSound;

        [SerializeField]
        private UnityEvent onCountdownEnd;

        private int number;
        private Vector3 originalScale;
        private Tweener tw;
        private int step;

        protected override void Awake()
        {
            base.Awake();
            originalScale = numberText.transform.localScale;
            step = (int) Mathf.Sign(endNumber - startNumber);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            numberText.gameObject.SetActive(false);

            if (playOnEnable)
            {
                StartCountdown(delay);
            }
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

        /// <summary>
        /// 开始计时。
        /// </summary>
        /// <param name="delay">开始计时的延迟时间。</param>
        public void StartCountdown(float delay = 0)
        {
            number = startNumber - step;

            // 如果保持显示第一个数字，用最大的 scale 显示
            if (keepFirstNumber)
            {
                ShowNumber(originalScale * Mathf.Max(startScale, endScale));
            }

            StartCoroutine(Countdown(delay));
        }

        /// <summary>
        /// 进行一步正常的倒计时。
        /// </summary>
        /// <param name="delay">到显示下一个数字的时间间隔。</param>
        private IEnumerator Countdown(float delay)
        {
            if (delay > Mathf.Epsilon)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            number += step;
            ShowNumber(originalScale * startScale);

            if (tw == null)
            {
                tw = numberText.transform.DOScale(originalScale * endScale, easeDuration)
                               .SetAutoKill(false)
                               .SetEase(easeType)
                               .SetUpdate(UpdateType.Normal, true) // 忽略 Time.timeScale
                               .OnPlay(PlaySound)
                               .OnComplete(OnCompleteShowNumber);
                tw.Play();
            }
            else
            {
                tw.Restart();
            }
        }

        /// <summary>
        /// 直接用最大 scale 显示下一步倒计时。
        /// 用于保留显示最后一个数字的情况。
        /// </summary>
        private IEnumerator CountdownWithLargeScale()
        {
            if (interval > Mathf.Epsilon)
            {
                yield return new WaitForSecondsRealtime(interval);
            }

            number += step;
            ShowNumber(originalScale * Mathf.Max(startScale, endScale));
            PlaySound();

            if (easeDuration > Mathf.Epsilon)
            {
                yield return new WaitForSecondsRealtime(easeDuration);
            }

            OnCompleteShowNumber();
        }

        /// <summary>
        /// 执行计时完毕后的回调函数。
        /// </summary>
        private IEnumerator OnCountdownComplete()
        {
            if (interval > Mathf.Epsilon)
            {
                yield return new WaitForSecondsRealtime(interval);
            }

            onCountdownEnd.Invoke();
        }

        private void ShowNumber(Vector3 scale)
        {
            numberText.text = number.ToString();
            numberText.gameObject.SetActive(true);
            numberText.transform.localScale = scale;
        }

        private void PlaySound()
        {
            if (!countingSound.IsNull())
            {
                countingSound.Create().PlayAndDestroy();
            }
        }

        private void OnCompleteShowNumber()
        {
            // 计时完毕
            if (number == endNumber)
            {
                if (!keepLastNumber)
                {
                    numberText.gameObject.SetActive(false);
                }

                StartCoroutine(OnCountdownComplete());
            }
            // 用最大 scale 显示并保留最后一个数字
            else if (keepLastNumber && number == endNumber - step && startScale > endScale)
            {
                numberText.gameObject.SetActive(false);
                StartCoroutine(CountdownWithLargeScale());
            }
            // 显示下一个数字
            else
            {
                numberText.gameObject.SetActive(false);
                StartCoroutine(Countdown(interval));
            }
        }
    }
}