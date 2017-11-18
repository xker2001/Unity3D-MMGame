using System;
using System.Collections.Generic;
using System.Collections;
using MMGame.DataType;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 按序播放给定的 AUIEffect 数组，可以设定乱序和间隔时间。
    /// </summary>
    public class UIPlayEffectSequence : AUIEffect
    {
        [Serializable]
        private class UIEffectParameters
        {
            public AUIEffect Effect;

            [TableColumnWidth(50)]
            public float Delay;
        }

        /// <summary>
        /// 待播放的效果列表。
        /// </summary>
        [TableList]
        [SerializeField]
        private List<UIEffectParameters> effects;

        /// <summary>
        /// 每次开始新的循环前是否执行一次乱序。
        /// </summary>
        [SerializeField]
        private bool randomOrder;

        /// <summary>
        /// 是否循环播放。
        /// </summary>
        [SerializeField]
        private bool loop;

        /// <summary>
        /// 循环播放的中间时间间隔。
        /// </summary>
        [ShowIf("loop")]
        [SerializeField]
        private RandomFloat loopInterval;

        private AUIEffect currentEffect;
        private int currentIndex;

        protected override void Awake()
        {
            for (var i = 0; i < effects.Count; i++)
            {
                effects[i].Effect.OnComplete.AddListener(OnEffectComplete);
            }
        }

        protected override void OnDestroy()
        {
            for (var i = 0; i < effects.Count; i++)
            {
                effects[i].Effect.OnComplete.RemoveListener(OnEffectComplete);
            }
        }

        protected override void PreparePlaying()
        {
            if (randomOrder)
            {
                effects.Shuffle();
            }

            if (effects.Count > 0)
            {
                effects[0].Effect.Prepare();
            }

            currentEffect = null;
            currentIndex = 0;
        }

        protected override void PlayEffect()
        {
            if (effects.Count > 0)
            {
                DelayPlayEffect(effects[0], effects[0].Delay);
            }
        }

        protected override void StopEffect()
        {
            if (currentEffect)
            {
                currentEffect.Stop();
                currentEffect = null;
            }

            StopAllCoroutines();
        }

        private void DelayPlayEffect(UIEffectParameters effectParameters, float delay)
        {
            if (delay > Mathf.Epsilon)
            {
                StartCoroutine(PlayOneEffect(effectParameters, delay));
            }
            else
            {
                effectParameters.Effect.Play();
            }

            currentEffect = effectParameters.Effect;
        }

        private IEnumerator PlayOneEffect(UIEffectParameters effectParameters, float delay)
        {
            yield return new WaitForSeconds(delay);

            effectParameters.Effect.Play();
        }

        private void OnEffectComplete()
        {
            currentEffect = null;

            if (currentIndex < effects.Count - 1)
            {
                currentIndex += 1;
                DelayPlayEffect(effects[currentIndex], effects[currentIndex].Delay);
            }
            else if (currentIndex == effects.Count - 1 && loop)
            {
                if (randomOrder)
                {
                    effects.Shuffle();
                }

                currentIndex = 0;
                DelayPlayEffect(effects[currentIndex], effects[currentIndex].Delay + loopInterval.Value);
            }
            else
            {
                SetSelfComplete();
            }
        }
    }
}