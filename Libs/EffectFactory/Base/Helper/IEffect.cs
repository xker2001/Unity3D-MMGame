namespace MMGame.EffectFactory
{
    /// <summary>
    /// 单次播放的特效接口。
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// 播放一次特效。
        /// </summary>
        void PlayOneShot();

        /// <summary>
        /// 停止播放特效。
        /// 这里没有平滑停止，因为 PlayOneShot 隐含了自我回收。
        /// </summary>
        void Stop();
    }
}