using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace MMGame.UI
{
    public class UIRotate : AUIEffect
    {
        [Tooltip("是否本地旋转（目前勾选与否未发现区别）。")]
        [SerializeField]
        private bool localRotate = true;

        [Tooltip("是否在 OnEnable() 中重置旋转方向。")]
        [SerializeField]
        private bool resetOnEnable = true;

        [Title("Easy type")]
        [SerializeField]
        private Ease easeType = Ease.Linear;

        [Title("From & To")]
        [Tooltip("选中时进行绝对量旋转，from 和 to 都是绝对值；"
                 + "未选中时进行相对量旋转，to 为相对起始位置的增量。")]
        [SerializeField]
        private bool fromTo;

        [ShowIf("fromTo")]
        [SerializeField]
        private Vector3 from = Vector3.zero;

        [SerializeField]
        private Vector3 to = new Vector3(0, 0, 360);

        [Title("Duration")]
        [SerializeField]
        private bool randomDuration;

        [ShowIf("randomDuration")]
        [MinMaxSlider(0, 5, true)]
        [SerializeField]
        private Vector2 durationRange = new Vector2(0, 1);

        [HideIf("randomDuration")]
        [SerializeField]
        private float duration = 1;

        [Title("Loop")]
        [SerializeField]
        private bool loop;

        [ShowIf("loop")]
        [SerializeField]
        private LoopType loopType = LoopType.Incremental;

        [ShowIf("loop")]
        [SerializeField]
        private int loopTimes = -1;

        private RectTransform rectTransform;
        private Quaternion? originalRotation;
        private Vector3 relativeTo;
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
            originalRotation = localRotate ? rectTransform.localRotation : rectTransform.rotation;
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
            if (fromTo)
            {
                if (localRotate)
                {
                    rectTransform.localEulerAngles = from;
                }
                else
                {
                    rectTransform.eulerAngles = from;
                }

                relativeTo = to - from;
            }
            else
            {
                relativeTo = to;
            }

            actualDuration = randomDuration ? Random.Range(durationRange.x, durationRange.y) : duration;
        }

        protected override void PlayEffect()
        {
            if (tw == null)
            {
                tw = localRotate
                         ? rectTransform.DOLocalRotate(relativeTo, actualDuration, RotateMode.LocalAxisAdd)
                         : rectTransform.DORotate(relativeTo, actualDuration, RotateMode.LocalAxisAdd);

                tw.SetEase(easeType)
                  .OnComplete(SetSelfComplete)
                  .SetAutoKill(false)
                  .SetUpdate(UpdateType.Normal, true);

                if (loop)
                {
                    tw.SetLoops(loopTimes, loopType);
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

        private void ResetEffect()
        {
            if (originalRotation != null)
            {
                if (localRotate)
                {
                    rectTransform.localRotation = originalRotation.Value;
                }
                else
                {
                    rectTransform.rotation = originalRotation.Value;
                }
            }
        }
    }
}