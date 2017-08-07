using System;
using UnityEngine;

namespace MMGame.VideoPlayer
{
    /// <summary>
    /// 播放器状态枚举。
    /// </summary>
    public enum VideoState
    {
        NotReady,
        Ready,
        End,
        Playing,
        Paused,
        Stopped,
        Error
    }

    /// <summary>
    /// 用于适配第三方播放器的播放器抽象类。
    /// </summary>
    abstract public class AVideoPlayer : MonoBehaviour
    {
        virtual protected void OnEnable()
        {
            AddEventListeners();
        }

        virtual protected void OnDisable()
        {
            RemoveEventListeners();
        }

        //--------------------------------------------------
        // 事件
        //--------------------------------------------------

        public event Action Ready;
        public event Action Finished;
        public event Action<string> Error;
        public event Action FirstFrameReady;

        //--------------------------------------------------
        // 公有方法
        //--------------------------------------------------

        /// <summary>
        /// 加载视频资源。
        /// </summary>
        /// <param name="path">资源路径或 url。</param>
        abstract public void Load(string path);

        /// <summary>
        /// 卸载当前视频资源。
        /// </summary>
        abstract public void Unload();

        /// <summary>
        /// 播放视频。
        /// </summary>
        abstract public void Play();

        /// <summary>
        /// 暂停播放。
        /// </summary>
        abstract public void Pause();

        /// <summary>
        /// 停止播放。
        /// </summary>
        abstract public void Stop();

        /// <summary>
        /// 获取视频时间长度（ms）。
        /// </summary>
        /// <returns>视频长度（ms）。</returns>
        abstract public int GetLength();

        /// <summary>
        /// 获取当前播放位置。
        /// </summary>
        /// <returns>播放位置（ms）。</returns>
        abstract public int GetPosition();

        /// <summary>
        /// 设置播放位置。
        /// </summary>
        /// <param name="ms">播放位置（ms）。</param>
        abstract public void SetPosition(int ms);

        /// <summary>
        /// 设置播放音量。
        /// </summary>
        /// <param name="volume">音量，0~1。</param>
        abstract public void SetVolume(float volume);

        /// <summary>
        ///  获取当前播放状态。
        /// </summary>
        /// <returns>播放状态。</returns>
        abstract public VideoState GetState();

        //--------------------------------------------------
        // 事件回调
        //--------------------------------------------------

        abstract protected void AddEventListeners();
        abstract protected void RemoveEventListeners();

        protected void InvokeReadyCallbacks()
        {
            if (Ready != null)
            {
                Ready();
            }
        }

        protected void InvokeFinishedCallbacks()
        {
            if (Finished != null)
            {
                Finished();
            }
        }

        protected void InvokeFirstFrameReadyCallbacks()
        {
            if (FirstFrameReady != null)
            {
                FirstFrameReady();
            }
        }

        protected void InvokeErrorCallbacks(string msg)
        {
            if (Error != null)
            {
                Error(msg);
            }
        }
    }
}