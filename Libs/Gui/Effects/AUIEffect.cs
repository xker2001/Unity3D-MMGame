using UnityEngine;
using UnityEngine.Events;
using MMGame.EffectFactory;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

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

    abstract public class AUIEffect : UIBehaviour
    {
        [FoldoutGroup("Common")]
        [Tooltip("效果作用的目标。如果为 null 则作用于组件所在物体。")]
        [SerializeField]
        private GameObject target;

        [FoldoutGroup("Common")]
        [Tooltip("效果所在分组，供第三方按组播放效果。")]
        [SerializeField]
        private UIEffectGroup @group = UIEffectGroup.None;

        [FoldoutGroup("Common")]
        [Tooltip("在 Start() 中播放。")]
        [SerializeField]
        private bool playOnStart;

        [FoldoutGroup("Common")]
        [Tooltip("在 OnEnable() 中播放。注意会略过第一个 OnEnable。")]
        [SerializeField]
        private bool playOnEnable;

        [FoldoutGroup("Common")]
        [Tooltip("在 Start() 中进行播放准备（如预置到起始位置）")]
        [SerializeField]
        private bool prepareOnStart;

        [FoldoutGroup("Common")]
        [Tooltip("在 OnEnable() 中进行播放准备（如预置到起始位置）。注意会略过第一个 OnEnable。")]
        [SerializeField]
        private bool prepareOnEnable;

        [FoldoutGroup("Common")]
        [Tooltip("播放完毕后将 game object 禁用。")]
        [SerializeField]
        private bool disableOnComplete;

        [FoldoutGroup("Common")]
        [SerializeField]
        private float delay; // the delay of playing effect

        [SerializeField]
        private SoundParamFactory sound;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onPlay;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onPrepare;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onComplete;

        private bool isStarted;

        public bool IsPlaying { get; private set; }

        public UnityEvent OnComplete
        {
            get { return onComplete; }
            set { onComplete = value; }
        }

        public UnityEvent OnPrepare
        {
            get { return onPrepare; }
            set { onPrepare = value; }
        }

        public UnityEvent OnPlay
        {
            get { return onPlay; }
            set { onPlay = value; }
        }

        public UIEffectGroup Group
        {
            get { return @group; }
        }

        public GameObject Target
        {
            get { return target ? target : gameObject; }
        }

        /// <summary>
        /// 执行播放特效前的准备工作，如将 alpha 设置为 0.
        /// </summary>
        abstract protected void PreparePlaying();

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

            if (prepareOnStart)
            {
                Prepare();
            }

            if (playOnStart)
            {
                Play();
            }

            isStarted = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isStarted)
            {
                if (prepareOnEnable)
                {
                    Prepare();
                }

                if (playOnEnable)
                {
                    Play();
                }
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
        /// 执行播放特效前的准备工作，如将 alpha 设置为 0、位置设置到起始点。
        /// 
        /// 注意：
        /// - 可以设置则 Start() 和 OnEnable() 中自动执行。
        /// - 在 Play() 之前必定会执行。
        /// 
        /// 该公有方法在特殊需要时使用。如一个不会被 disable 掉的打分面板，出现的方法是从屏幕外移动到屏幕内，
        /// 然后从屏幕上方移入星星图标，其过程如下：
        /// - 第一次掉落星星，星星就位。
        /// - 面板移出屏幕。
        /// - 对星星特效执行 Prepare()。
        /// - 面板移入屏幕。
        /// - 对星星特效执行 Play()。
        /// </summary>
        public void Prepare()
        {
            PreparePlaying();
            onPrepare.Invoke();
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

            Prepare();

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
        virtual protected void SetSelfComplete()
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
            onPlay.Invoke();

            if (!sound.IsNull())
            {
                sound.SpatialBlend = 0;
                sound.Create().PlayAndDestroy();
            }

            PlayEffect();
        }
    }
}