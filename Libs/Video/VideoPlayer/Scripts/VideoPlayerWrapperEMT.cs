#if EASY_MOVIE_TEXTURE
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.VideoPlayer.EasyMovieTexture
{
    /// <summary>
    /// 基于 Easy Movie Texture 插件的播放器实现。
    /// </summary>
    public class VideoPlayerWrapperEMT : AVideoPlayer
    {
        [SerializeField]
        private MediaPlayerCtrl player;

        private void Awake()
        {
            Assert.IsNotNull(player);
        }

        //--------------------------------------------------
        // 实现公有方法
        //--------------------------------------------------

        public override void Load(string path)
        {
            player.Load(path);
        }

        public override void Unload()
        {
            player.UnLoad();
        }

        public override void Play()
        {
            player.Play();
        }

        public override void Pause()
        {
            player.Pause();
        }

        public override void Stop()
        {
            player.Stop();
        }

        public override int GetLength()
        {
            return player.GetDuration();
        }

        public override int GetPosition()
        {
            return player.GetSeekPosition();
        }

        public override void SetPosition(int ms)
        {
            player.SeekTo(ms);
        }

        public override void SetVolume(float volume)
        {
            player.SetVolume(volume);
        }

        public override VideoState GetState()
        {
            MediaPlayerCtrl.MEDIAPLAYER_STATE state = player.GetCurrentState();

            switch (state)
            {
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.READY:
                    return VideoState.Ready;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.END:
                    return VideoState.End;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR:
                    return VideoState.Error;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY:
                    return VideoState.NotReady;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED:
                    return VideoState.Paused;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING:
                    return VideoState.Playing;
                case MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED:
                    return VideoState.Stopped;
                default:
                    return VideoState.Error;
            }
        }

        //--------------------------------------------------
        // 实现事件回调
        //--------------------------------------------------

        protected override void AddEventListeners()
        {
            player.OnReady += OnReady;
            player.OnEnd += OnEnd;
            player.OnVideoFirstFrameReady += OnFirstFrameReady;
            player.OnVideoError += OnError;
        }

        protected override void RemoveEventListeners()
        {
            player.OnReady -= OnReady;
            player.OnEnd -= OnEnd;
            player.OnVideoFirstFrameReady -= OnFirstFrameReady;
            player.OnVideoError -= OnError;
        }

        private void OnReady()
        {
            InvokeReadyCallbacks();
        }

        private void OnEnd()
        {
            InvokeFinishedCallbacks();
        }

        private void OnFirstFrameReady()
        {
            InvokeFirstFrameReadyCallbacks();
        }

        private void OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode,
                             MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
        {
            InvokeErrorCallbacks(string.Format(
                                     "{0}\n{1}", GetErrorMessage(errorCode), GetErrorMessage(errorCodeExtra)));
        }

        //--------------------------------------------------
        // 私有方法
        //--------------------------------------------------

        /// <summary>
        /// 将 Easy Movie Texture 的错误吗转换为错误信息字符串。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns>错误信息字符串。</returns>
        private string GetErrorMessage(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode)
        {
            switch (errorCode)
            {
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
                    return "Not valid for progressive playback.";
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_IO:
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_MALFORMED:
                    return "Malformed.";
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_TIMED_OUT:
                    return "Timed out.";
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNSUPPORTED:
                    return "Unsupported.";
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_SERVER_DIED:
                    return "Server died.";
                case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN:
                    return "Unknow.";
                default:
                    return "Unknow.";
            }
        }
    }
}

#endif