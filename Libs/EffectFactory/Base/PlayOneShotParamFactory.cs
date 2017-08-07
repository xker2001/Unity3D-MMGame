using UnityEngine;

namespace MMGame.EffectFactory
{
    abstract public class PlayOneShotParamFactory : ParamFactory
    {
        /// <summary>
        /// Cast Produce() 的类型，后同。
        /// </summary>
        /// <returns>参数工厂生成的物体。</returns>
        public PlayOneShotParamObject Create()
        {
            return (PlayOneShotParamObject) Produce();
        }

        public PlayOneShotParamObject Create(Vector3 position)
        {
            return (PlayOneShotParamObject) Produce(position);
        }

        public PlayOneShotParamObject Create(Vector3 position, Quaternion rotation)
        {
            return (PlayOneShotParamObject) Produce(position, rotation);
        }

        public PlayOneShotParamObject Create(Transform parent)
        {
            return (PlayOneShotParamObject) Produce(parent);
        }
    }
}