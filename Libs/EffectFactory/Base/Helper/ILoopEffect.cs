namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的特效接口。
    /// </summary>
    public interface ILoopEffect
    {
        void Loop();
        void Stop();
        void SmoothStop();
    }
}