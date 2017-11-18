using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.UI
{
    public class UIFloat : AUIEffect
    {
        [Tooltip("是否在 OnEnable() 中重置位置。")]
        [SerializeField]
        private bool resetOnEnable = true;

        [Tooltip("相对位移还是绝对位移。")]
        [SerializeField]
        private bool relative = true;

        [HideIf("relative")]
        [SerializeField]
        private bool useDummy;

        [HideIf("useDummy")]
        [SerializeField]
        private Vector3 from;

        [HideIf("useDummy")]
        [SerializeField]
        private Vector3 to;

        [ShowIf("useDummy")]
        [SerializeField, Required]
        private Transform fromDummy;

        [ShowIf("useDummy")]
        [SerializeField, Required]
        private Transform toDummy;

        [SerializeField]
        private Ease easeType = Ease.InOutSine;

        [SerializeField]
        private bool randomDuration;

        [ShowIf("randomDuration")]
        [MinMaxSlider(0, 5, true)]
        [SerializeField]
        private Vector2 durationRange = new Vector2(0, 1);

        [HideIf("randomDuration")]
        [SerializeField]
        private float duration = 1;

        [SerializeField]
        private int loopTimes = -1;

        private RectTransform rectTransform;
        private Vector3? originalPosition;
        private Vector3 actualFrom; // 绝对位置
        private Vector3 actualTo;
        private float actualDuration;
        private Tweener tw;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = Target.GetComponent<RectTransform>();
        }

        protected override void OnEnable()
        {
            if (resetOnEnable)
            {
                ResetEffect();
            }

            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            // 必须在 Start 中执行，在 OnEnable 中执行可能记录错误信息。
            originalPosition = rectTransform.localPosition;
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

        protected override void PreparePlaying()
        {
            if (relative)
            {
                actualFrom = rectTransform.localPosition + from;
                actualTo = rectTransform.localPosition + to;
            }
            else if (useDummy)
            {
                actualFrom = fromDummy.localPosition;
                actualTo = toDummy.localPosition;
            }
            else
            {
                actualFrom = from;
                actualTo = to;
            }

            rectTransform.localPosition = actualFrom;
            actualDuration = randomDuration ? Random.Range(durationRange.x, durationRange.y) : duration;
        }

        protected override void PlayEffect()
        {
            if (tw == null)
            {
                tw = rectTransform.DOLocalMove(actualTo, actualDuration);

                tw.SetEase(easeType)
                  .SetLoops(loopTimes, LoopType.Yoyo)
                  .OnComplete(SetSelfComplete)
                  .SetAutoKill(false)
                  .SetUpdate(UpdateType.Normal, true);
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

        private void ResetEffect()
        {
            if (originalPosition != null)
            {
                rectTransform.localPosition = originalPosition.Value;
            }
        }
    }
}