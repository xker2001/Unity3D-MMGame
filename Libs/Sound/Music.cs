using System;
using UnityEngine;

namespace MMGame.Sound
{
    /// <summary>
    /// 基础的音乐播放服务，供具体的音乐播放组件调用。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour
    {
        private static Music singleton;

        public static Music Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = (Music) FindObjectOfType(typeof(Music));
                }

                return singleton;
            }
        }

        /// <summary>
        /// 半静音时的音量比例。
        /// </summary>
        [SerializeField]
        private float halfMuteVolume = 0.5f;

        private AudioSource audioSource;
        private MusicParameters currentMusicParams;
        private MusicParameters nextMusicParams;

        /// <summary>
        /// 下一首音乐是否循环播放。
        /// </summary>
        private bool nextMusicIsLoop;

        private float volume;

        /// <summary>
        /// 当前的实际音量比，用于整体改变音量大小（通常在将音乐置于幕后时使用）。
        /// 实际音量 = 音量 * 音量比
        /// </summary>
        private float volumePercent = 1;

        private bool isFadeIn;
        private bool isFadeOut;
        private float? fadeInDuration;
        private float? fadeOutDuration;
        private bool isMute;
        private float muteFactor = 1;
        private float? muteDuration;

        /// <summary>
        /// 是否正在准备播放下一首音乐（等待当前淡出完成）。
        /// </summary>
        private bool isWaitForPlaying;

        /// <summary>
        /// 是否在播放音乐。用于检测播放状态的变化。
        /// </summary>
        private bool isPlaying;

        /// <summary>
        /// 当前音量，其最大值由具体音乐的 volume 参数决定。
        /// </summary>
        private float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                UpdateVolume();
            }
        }

        public static event Action PlayStarted;
        public static event Action PlayEnded;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0;
        }

        /// <summary>
        /// 当前正在播放的 audio clip。
        /// </summary>
        public static AudioClip Clip
        {
            get { return Singleton == null ? null : Singleton.audioSource.clip; }
        }

        /// <summary>
        /// 是否正在播放音乐。
        /// </summary>
        public static bool IsPlaying
        {
            get { return Singleton != null && Singleton.audioSource.isPlaying; }
        }

        /// <summary>
        /// 开始淡出停止当前音乐。
        /// </summary>
        public static void Stop(float? fadeOutDuration)
        {
            if (Singleton == null)
            {
                return;
            }

            Singleton.isFadeIn = false;
            Singleton.isWaitForPlaying = false;
            Singleton.isFadeOut = Singleton.audioSource.isPlaying;
            Singleton.fadeOutDuration = fadeOutDuration;
        }

        /// <summary>
        /// 立即停止播放当前音乐。如果正在淡入或淡出，则淡入淡出过程会被中断。
        /// </summary>
        public static void StopImmediately()
        {
            if (Singleton == null)
            {
                return;
            }

            Singleton.audioSource.Stop();
            Singleton.isFadeIn = false;
            Singleton.isFadeOut = false;
            Singleton.isWaitForPlaying = false;
            Singleton.currentMusicParams = null;
        }

        /// <summary>
        /// 开始淡入播放下一首音乐。
        /// - 如果正在淡出，则等淡出完成后再开始新的淡入。
        /// - 如果正在淡入，则正在进行的淡入过程会被中断。
        /// - 如果 fadeInDuration 为 null，则使用音乐自带的淡入时间参数。
        /// - 目前没有处理上一首音乐正在正常播放的情况。
        /// </summary>
        public static void Play(MusicParameters musicParams, float? fadeInDuration = null, bool loop = false)
        {
            if (Singleton == null)
            {
                return;
            }

            if (Singleton.isFadeOut)
            {
                Singleton.nextMusicParams = musicParams;
                Singleton.nextMusicIsLoop = loop;
                Singleton.isWaitForPlaying = true;
                Singleton.fadeInDuration = fadeInDuration;
            }
            else
            {
                if (Singleton.currentMusicParams != null && Singleton.currentMusicParams.Clip == musicParams.Clip)
                {
                    return;
                }

                Singleton.currentMusicParams = musicParams;
                Singleton.audioSource.clip = musicParams.Clip;
                Singleton.audioSource.loop = loop;
                Singleton.audioSource.time = 0;
                Singleton.audioSource.Play();

                Singleton.Volume = 0;
                Singleton.isFadeIn = true;
                Singleton.isWaitForPlaying = false;
                Singleton.nextMusicParams = null;
                Singleton.fadeInDuration = fadeInDuration;
            }
        }

        /// <summary>
        /// 淡入循环播放下一首音乐。
        /// 如果 fadeInDuration 为 null，则使用音乐自带的淡入时间参数。
        /// </summary>
        public static void Loop(MusicParameters musicParams, float? fadeInDuration = null)
        {
            Play(musicParams, fadeInDuration, true);
        }

        /// <summary>
        /// 将整体音量降低到参数指定的比例。
        /// 如果 fadeOutDuration 为 null，则使用音乐自带的淡出时间参数。
        /// </summary>
        public static void TurnDown(float percent, float? fadeOutDuration = null)
        {
            if (Singleton == null)
            {
                return;
            }

            Singleton.volumePercent = percent;
            Singleton.fadeOutDuration = fadeOutDuration;
            Singleton.UpdateVolume();
        }

        /// <summary>
        /// 将整体音量降低到预设的比例。
        /// 如果 fadeOutDuration 为 null，则使用音乐自带的淡出时间参数。
        /// </summary>
        public static void TurnDown(float? fadeOutDuration = null)
        {
            TurnDown(Singleton.halfMuteVolume, fadeOutDuration);
        }

        /// <summary>
        /// 恢复音量到正常比例。
        /// 如果 fadeInDuration 为 null，则使用音乐自带的淡入时间参数。
        /// </summary>
        public static void TurnUp(float? fadeInDuration = null)
        {
            if (Singleton == null)
            {
                return;
            }

            Singleton.volumePercent = 1;
            Singleton.fadeInDuration = fadeInDuration;
            Singleton.UpdateVolume();
        }

        /// <summary>
        /// 静音。
        /// 如果 muteDuration 为 null，则使用音乐自带的淡出时间参数。
        /// </summary>
        /// <param name="muteDuration"></param>
        public static void TurnOff(float? muteDuration = null)
        {
            if (Singleton.currentMusicParams == null)
            {
                TurnOffImmediately();
            }
            else
            {
                Singleton.isMute = true;
                Singleton.muteDuration = muteDuration;
            }
        }

        public static void TurnOffImmediately()
        {
            Singleton.isMute = true;
            Singleton.muteFactor = 0;
        }

        public static void TurnOn(float? muteDuration = null)
        {
            if (Singleton.currentMusicParams == null)
            {
                TurnOnImmediately();
            }
            else
            {
                Singleton.isMute = false;
                Singleton.muteDuration = muteDuration;
            }
        }

        public static void TurnOnImmediately()
        {
            Singleton.isMute = false;
            Singleton.muteFactor = 1;
        }

        /// <summary>
        /// 更新 audio source 的实际音量。
        /// </summary>
        private void UpdateVolume()
        {
            audioSource.volume = volume * volumePercent * muteFactor;
        }

        private void Update()
        {
            if (isMute && muteFactor > Mathf.Epsilon)
            {
                float muteDuration = this.muteDuration.HasValue
                                         ? this.muteDuration.Value
                                         : currentMusicParams.FadeOutDuration;
                float fadeSpeed = 1 / muteDuration;
                muteFactor -= fadeSpeed * Time.deltaTime;

                if (muteFactor < 0)
                {
                    muteFactor = 0;
                }

                UpdateVolume();
            }
            else if (!isMute && muteFactor < 1)
            {
                float muteDuration = this.muteDuration.HasValue
                                         ? this.muteDuration.Value
                                         : currentMusicParams.FadeInDuration;
                float fadeSpeed = 1 / muteDuration;
                muteFactor += fadeSpeed * Time.deltaTime;

                if (muteFactor > 1)
                {
                    muteFactor = 1;
                }

                UpdateVolume();
            }

            if (isFadeIn)
            {
                float fadeInDuration = Singleton.fadeInDuration.HasValue
                                           ? Singleton.fadeInDuration.Value
                                           : currentMusicParams.FadeInDuration;
                float fadeSpeed = currentMusicParams.Volume / fadeInDuration;
                Volume += fadeSpeed * Time.deltaTime;

                if (Volume >= currentMusicParams.Volume)
                {
                    Volume = currentMusicParams.Volume;
                    isFadeIn = false;
                    Singleton.fadeInDuration = null;
                }
            }
            else if (isFadeOut)
            {
                float fadeOutDuration = Singleton.fadeOutDuration.HasValue
                                            ? Singleton.fadeOutDuration.Value
                                            : currentMusicParams.FadeOutDuration;
                float fadeSpeed = currentMusicParams.Volume / fadeOutDuration;
                Volume -= fadeSpeed * Time.deltaTime;

                if (Volume <= 0)
                {
                    Volume = 0;
                    isFadeOut = false;
                    audioSource.Stop();
                    currentMusicParams = null;
                    Singleton.fadeOutDuration = null;

                    if (isWaitForPlaying && nextMusicParams != null)
                    {
                        Play(nextMusicParams, Singleton.fadeInDuration, nextMusicIsLoop);
                    }
                }
            }

            if (!isPlaying && IsPlaying && PlayStarted != null)
            {
                PlayStarted();
            }
            else if (isPlaying && !IsPlaying && PlayEnded != null)
            {
                currentMusicParams = null;
                PlayEnded();
            }

            isPlaying = IsPlaying;
        }
    }
}