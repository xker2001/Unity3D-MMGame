using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 适合循环播放的参数工厂。
    /// </summary>
    abstract public class PlayLoopParamFactory : ParamFactory
    {
        /// <summary>
        /// Cast Produce() 的类型，后同。
        /// </summary>
        /// <returns>参数工厂生成的物体。</returns>
        public PlayLoopParamObject Create()
        {
            return (PlayLoopParamObject) Produce();
        }

        public PlayLoopParamObject Create(Vector3 position)
        {
            return (PlayLoopParamObject) Produce(position);
        }

        public PlayLoopParamObject Create(Vector3 position, Quaternion rotation)
        {
            return (PlayLoopParamObject) Produce(position, rotation);
        }

        public PlayLoopParamObject Create(Transform parent)
        {
            return (PlayLoopParamObject) Produce(parent);
        }
    }
}