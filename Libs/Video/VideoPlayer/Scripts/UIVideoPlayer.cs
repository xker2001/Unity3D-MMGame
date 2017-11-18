using Sirenix.OdinInspector;

namespace MMGame.VideoPlayer
{
    public class UIVideoPlayer : AUIVideoPlayer
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            SetActive(blackScreen, true);
            SetActive(loadingIcon, false);
            SetActive(controls, true);
            SetActive(controlsMask, true);
            SetTimelineSliderValue(0);
            External.Action.TurnOffMusic();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnUnload();
            External.Action.TurnOnMusic();
        }

        //--------------------------------------------------
        // 公有方法
        //--------------------------------------------------

        public override void OnLoad(string path)
        {
            SetActive(controls, true);
            SetActive(controlsMask, true);
            SetTimelineSliderValue(0);
            SetActive(loadingIcon, true);
        }

        public override void OnStop()
        {
            SetPlayButton(false);
        }

        public override void OnPlay()
        {
            SetPlayButton(true);
        }

        public override void OnLoadAndPlay(string path)
        {
            SetPlayButton(true);
        }

        public override void OnPause()
        {
            SetPlayButton(false);
        }

        public override void OnUnload() {}
        public override void OnSetPosition(int ms) {}
        public override void OnSetVolume(float volume) {}

        //--------------------------------------------------
        // 事件回调方法
        //--------------------------------------------------

        protected override void OnReady()
        {
            SetTimelineSliderValue(0);
            SetActive(controls, true);
            SetActive(controlsMask, false);
        }

        protected override void OnFinished()
        {
            SetPlayButton(false);
        }

        protected override void OnFirstFrameReady()
        {
            SetActive(blackScreen, false);
            SetActive(loadingIcon, false);
        }

        protected override void OnError(string msg)
        {
            print(msg);
        }

        protected override void OnTimelinePointerDown(float value)
        {
            SetTimelineSliderValue(value);
        }

        protected override void OnTimelineDragging(float value)
        {
            SetTimelineSliderValue(value);
        }

        protected override void OnTimelinePointerUp(float value)
        {
            SetTimelineSliderValue(value);
        }

        protected override void OnPlayOrPause(bool status) {}

        protected override void OnClose()
        {
            Unload();
        }

        protected override void OnClickScreen() {}

        //--------------------------------------------------
        // 测试方法 
        //--------------------------------------------------

        // http://clips.vorwaerts-gmbh.de/VfE_html5.mp4
        public string testVideo;

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.9f, 1, 1)]
        private void TestLoadAndPlay()
        {
            LoadAndPlay(testVideo);
        }
    }
}