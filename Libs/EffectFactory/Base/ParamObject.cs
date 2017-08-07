using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 参数工厂创建的实例。依赖 PoolManager。
    /// </summary>
    abstract public class ParamObject : MonoBehaviour
    {
        /// <summary>
        /// 将实例重新挂接到对象池节点。
        /// </summary>
        public void AttachToPoolNode()
        {
            PoolManager.Reparent(transform);
        }
    }
}