using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.VideoPlayer
{
    abstract public class AUIVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// 第三方播放器适配器。
        /// </summary>
        [SerializeField]
        protected AVideoPlayer player;

        /// <summary>
        /// 黑屏遮罩。
        /// 用于在播放开始前显示黑色屏幕。
        /// </summary>
        [SerializeField]
        protected GameObject blackScreen;

        /// <summary>
        /// 触屏按钮。
        /// 用于响应点击视频画面的操作。
        /// </summary>
        [SerializeField]
        protected Button screenButton;

        /// <summary>
        /// 视频加载动画图标。
        /// </summary>
        [SerializeField]
        protected GameObject loadingIcon;

        /// <summary>
        /// 关闭按钮。
        /// 用于关闭当前播放界面。
        /// </summary>
        [SerializeField]
        protected Button closeButton;

        /// <summary>
        /// 底部控制区。
        /// </summary>
        [SerializeField]
        protected GameObject controls;

        /// <summary>
        /// 控制区遮罩，用于屏蔽控制区操作。
        /// </summary>
        [SerializeField]
        protected GameObject controlsMask;

        /// <summary>
        /// 播放按钮，使用 Toggle + UISwitchButton 实现。
        /// </summary>
        [SerializeField]
        protected Toggle playButton;

        /// <summary>
        /// 时间条滑块控制。
        /// </summary>
        [SerializeField]
        protected Slider timelineSlider;

        /// <summary>
        /// 时间条定位控制。
        /// </summary>
        [SerializeField]
        protected UITimelineLocator timelineLocator;

        /// <summary>
        /// 加载完成后是否自动播放。
        /// </summary>
        private bool playOnLoaded;

        /// <summary>
        /// 鼠标/触摸抬起后是否继续播放（用于拖动 timeline 后）。
        /// </summary>
        private bool playOnPointerUp;

        /// <summary>
        /// 当前加载的视频的长度。
        /// </summary>
        private int clipLength;

        //--------------------------------------------------
        // MonoBehaviour 事件回调
        //--------------------------------------------------

        virtual protected void Awake()
        {
            Assert.IsNotNull(player);
        }

        virtual protected void OnEnable()
        {
            player.Ready += OnReadyCallback;
            player.Finished += OnFinishedCallback;
            player.FirstFrameReady += OnFirstFrameReadyCallback;
            player.Error += OnErrorCallback;

            if (playButton)
            {
                playButton.onValueChanged.AddListener(OnPlayOrPauseCallback);
                playButton.isOn = false;
            }

            if (timelineLocator)
            {
                timelineLocator.PointerDown += OnTimelinePointerDownCallback;
                timelineLocator.Dragging += OnTimelineDraggingCallback;
                timelineLocator.PointerUp += OnTimelinePointerUpCallback;
            }

            if (closeButton)
            {
                closeButton.interactable = true;
                closeButton.onClick.AddListener(OnCloseCallback);
            }

            if (screenButton)
            {
                screenButton.onClick.AddListener(OnClickScreenCallback);
            }
        }

        virtual protected void OnDisable()
        {
            player.Ready -= OnReadyCallback;
            player.Finished -= OnFinishedCallback;
            player.FirstFrameReady -= OnFirstFrameReadyCallback;
            player.Error -= OnErrorCallback;

            if (playButton)
            {
                playButton.onValueChanged.RemoveListener(OnPlayOrPauseCallback);
            }

            if (timelineLocator)
            {
                timelineLocator.PointerDown -= OnTimelinePointerDownCallback;
                timelineLocator.Dragging -= OnTimelineDraggingCallback;
                timelineLocator.PointerUp -= OnTimelinePointerUpCallback;
            }

            if (closeButton)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            if (screenButton)
            {
                screenButton.onClick.RemoveAllListeners();
            }
        }

        virtual protected void LateUpdate()
        {
            if (GetState() == VideoState.Playing)
            {
                SetTimelineSliderValue(Mathf.Clamp01(1.0f * GetPosition() / clipLength));
            }
        }

        //--------------------------------------------------
        // 公有方法
        //--------------------------------------------------

        public void Load(string path)
        {
            Assert.IsFalse(string.IsNullOrEmpty(path));
            player.Load(path);
            OnLoad(path);
        }

        public void Unload()
        {
            player.Unload();
            OnUnload();
        }

        public void Play()
        {
            VideoState state = GetState();

            if (state == VideoState.Playing)
            {
                return;
            }

            if (state == VideoState.NotReady)
            {
                playOnLoaded = true;
            }
            else
            {
                player.Play();
            }

            OnPlay();
        }

        public void LoadAndPlay(string path)
        {
            Load(path);
            playOnLoaded = true;
            OnLoadAndPlay(path);
        }

        public void Pause()
        {
            if (GetState() != VideoState.Playing)
            {
                return;
            }

            player.Pause();
            playOnLoaded = false;
            OnPause();
        }

        public void Stop()
        {
            if (GetState() == VideoState.Stopped)
            {
                return;
            }

            player.Stop();
            playOnLoaded = false;
            OnStop();
        }

        public void SetPosition(int ms)
        {
            player.SetPosition(ms);
            OnSetPosition(ms);
        }

        public void SetVolume(float volume)
        {
            player.SetVolume(volume);
            OnSetVolume(volume);
        }

        public int GetLength()
        {
            return player.GetLength();
        }

        public int GetPosition()
        {
            return player.GetPosition();
        }

        public VideoState GetState()
        {
            return player.GetState();
        }

        //--------------------------------------------------
        // 事件回调
        //--------------------------------------------------

        private void OnReadyCallback()
        {
            clipLength = GetLength();

            if (playOnLoaded)
            {
                Play();
            }

            OnReady();
        }

        private void OnFinishedCallback()
        {
            playOnPointerUp = true;
            OnFinished();
        }

        private void OnFirstFrameReadyCallback()
        {
            OnFirstFrameReady();
        }

        private void OnErrorCallback(string msg)
        {
            OnError(msg);
        }

        private void OnTimelinePointerDownCallback(float value)
        {
            if (GetState() == VideoState.Playing)
            {
                Pause();
                playOnPointerUp = true;
            }

            SetPosition((int) (clipLength * value));
            OnTimelinePointerDown(value);
        }

        private void OnTimelinePointerUpCallback(float value)
        {
            if (playOnPointerUp)
            {
                Play();
                playOnPointerUp = false;
            }

            SetPosition((int) (clipLength * value));
            OnTimelinePointerUp(value);
        }

        private void OnTimelineDraggingCallback(float value)
        {
            OnTimelineDragging(value);
        }

        private void OnPlayOrPauseCallback(bool status)
        {
            VideoState state = GetState();

            if (status && (state == VideoState.Ready || state == VideoState.Paused
                           || state == VideoState.Stopped || state == VideoState.End))
            {
                Play();
            }
            else if (!status && (state == VideoState.Playing))
            {
                Pause();
            }

            OnPlayOrPause(status);
        }

        private void OnCloseCallback()
        {
            OnClose();
        }

        private void OnClickScreenCallback()
        {
            OnClickScreen();
        }

        //--------------------------------------------------
        // 抽象方法
        //--------------------------------------------------

        abstract public void OnLoad(string path);
        abstract public void OnUnload();
        abstract public void OnPlay();
        abstract public void OnLoadAndPlay(string path);
        abstract public void OnPause();
        abstract public void OnStop();
        abstract public void OnSetPosition(int ms);
        abstract public void OnSetVolume(float volume);

        abstract protected void OnReady();
        abstract protected void OnFinished();
        abstract protected void OnFirstFrameReady();
        abstract protected void OnError(string msg);
        abstract protected void OnPlayOrPause(bool status);
        abstract protected void OnClose();
        abstract protected void OnTimelinePointerDown(float value);
        abstract protected void OnTimelinePointerUp(float value);
        abstract protected void OnTimelineDragging(float value);
        abstract protected void OnClickScreen();

        //--------------------------------------------------
        // 非公有方法
        //--------------------------------------------------

        /// <summary>
        /// 激活/禁用物体。
        /// </summary>
        /// <param name="go">物体。</param>
        /// <param name="value">true or false。</param>
        protected void SetActive(GameObject go, bool value)
        {
            if (go)
            {
                go.SetActive(value);
            }
        }

        /// <summary>
        /// 设置时间条滑块位置。
        /// </summary>
        /// <param name="value"></param>
        virtual protected void SetTimelineSliderValue(float value)
        {
            if (timelineSlider)
            {
                timelineSlider.value = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// 设置播放按钮状态。
        /// true: 播放状态（显示暂停按钮）
        /// false: 暂停状态（显示播放按钮）
        /// </summary>
        /// <param name="play"></param>
        virtual protected void SetPlayButton(bool play)
        {
            if (playButton)
            {
                playButton.isOn = play;
            }
        }
    }
}