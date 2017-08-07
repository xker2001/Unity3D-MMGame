using UnityEngine;
using UnityEngine.Events;
using MMGame.EffectFactory;

namespace MMGame.UI
{
    public enum UIEffectGroup : int
    {
        None,
        Group0,
        Group1,
        Group2,
        Group3,
        Group4,
        Group5,
        Group6,
        Group7,
        Group8,
        Group9,
        Group10,
        Group11,
        Group12,
        Group13,
        Group14,
        Group15,
        Group16,
        Group17,
        Group18,
        Group19
    }

    abstract public class AUIEffect : EasyUIBehaviour
    {
        [Tooltip("效果所在分组，供第三方按组播放效果。")]
        [SerializeField]
        private UIEffectGroup @group = UIEffectGroup.None;

        [Tooltip("在 Start() 中播放。")]
        [SerializeField]
        private bool playOnStart;

        [Tooltip("在 OnEnable() 中播放。")]
        [SerializeField]
        private bool playOnEnable;

        [Tooltip("播放完毕后将 game object 禁用。")]
        [SerializeField]
        private bool disableOnComplete;

        [SerializeField]
        private float delay; // the delay of playing effect

        [SerializeField]
        private SoundParamFactory sound;

        [SerializeField]
        private UnityEvent onComplete;

        private bool isStarted;

        public bool IsPlaying { get; private set; }

        public UIEffectGroup Group
        {
            get { return @group; }
        }

        /// <summary>
        /// 执行播放特效前的准备工作，如将 alpha 设置为 0.
        /// </summary>
        abstract protected void InitPlaying();

        /// <summary>
        /// 执行播放特效的逻辑。
        /// 执行完毕的时候应当调用 OnComplete()。
        /// </summary>
        abstract protected void PlayEffect();

        /// <summary>
        /// 执行停止播放特效的逻辑。
        /// </summary>
        abstract protected void StopEffect();

        protected override void Start()
        {
            base.Start();

            if (playOnStart)
            {
                Play();
            }

            isStarted = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (playOnEnable && isStarted)
            {
                Play();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (IsPlaying)
            {
                Stop();
            }
        }

        /// <summary>
        /// 播放特效。
        /// 如果 game object 处于禁用状态则先激活。
        /// </summary>
        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            InitPlaying();

            if (delay > Mathf.Epsilon)
            {
                Invoke("StartPlay", delay);
            }
            else
            {
                StartPlay();
            }
        }

        /// <summary>
        /// 停止播放特效，仅用于特效为循环播放的情形。
        /// </summary>
        public void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            IsPlaying = false;
            StopEffect();
        }

        /// <summary>
        /// 特效播放完毕时的回调函数，需要派生类手动调用。
        /// </summary>
        virtual protected void OnComplete()
        {
            IsPlaying = false;
            onComplete.Invoke();

            if (disableOnComplete)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 开始执行播放特效。
        /// </summary>
        private void StartPlay()
        {
            if (!sound.IsNull())
            {
                sound.SpatialBlend = 0;
                sound.Create().PlayAndDestroy();
            }

            PlayEffect();
        }
    }
}