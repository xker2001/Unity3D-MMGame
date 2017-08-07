using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 适合非循环播放的参数工厂生成物体。
    /// </summary>
    abstract public class PlayOneShotParamObject : ParamObject
    {
        /// <summary>
        /// 播放然后在播放完毕后销毁。
        /// </summary>
        abstract public void PlayAndDestroy();

        /// <summary>
        /// 销毁自己。需要保证销毁 ParamObject 及所有衍生物体。
        /// </summary>
        abstract public void Destroy();
    }
}