using UnityEngine;
using System.Collections;
using Sirenix.Utilities;

namespace MMGame.UI
{
    /// <summary>
    /// OnEnable 时禁用指定控件并在延迟后重新激活。
    /// 注意该组件不能用于控制自身，因为一旦禁用自身后延迟控制函数也会中止执行。
    /// </summary>
    public class UIDelayEnable : EasyUIBehaviour
    {
        [System.Serializable]
        private class DelayParameters
        {
            public GameObject Target;
#pragma warning disable 0649
            public float Delay;
#pragma warning restore 0649
        }

        [SerializeField]
        private bool executeOnEnable = true;

        [Tooltip("在 disable 时将目标控件设置为 disabled，防止目标控件在父级再次激活时跟随激活，跳过控制执行动作（如播放激活音效）。"
                 + "目标控件的原始状态应当设置为 disabled，保证其父级第一次激活时不会跟随激活。")]
        [SerializeField]
        private bool autoDisableTargets = true;

        [SerializeField]
        private DelayParameters[] delays;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (executeOnEnable)
            {
                DelayEnable();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (autoDisableTargets)
            {
                delays.ForEach(dp => dp.Target.SetActive(false));
            }
        }

        public void DelayEnable()
        {
            foreach (DelayParameters dp in delays)
            {
                if (dp.Delay < Mathf.Epsilon)
                {
                    if (!dp.Target.activeSelf)
                    {
                        dp.Target.SetActive(true);
                    }
                }
                else
                {
                    dp.Target.SetActive(false);
                    StartCoroutine(DelayEnableObject(dp.Target, dp.Delay));
                }
            }
        }

        private IEnumerator DelayEnableObject(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);

            go.SetActive(true);
        }
    }
}