using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 适合循环播放的参数工厂生成物体。
    /// </summary>
    abstract public class PlayLoopParamObject : ParamObject
    {
        /// <summary>
        /// 循环播放。
        /// </summary>
        abstract public void Loop();

        /// <summary>
        /// 立即停止播放。
        /// </summary>
        abstract public void Stop();

        /// <summary>
        /// 平滑地停止播放。
        /// </summary>
        abstract public void SmoothStop();

        /// <summary>
        /// 立即销毁自己。
        /// </summary>
        abstract public void Destroy();

        /// <summary>
        /// 平滑地销毁自己。
        /// </summary>
        abstract public void SmoothDestroy();
    }
}