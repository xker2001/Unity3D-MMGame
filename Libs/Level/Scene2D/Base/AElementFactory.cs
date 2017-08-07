using System;

namespace MMGame.Scene2D
{
    /// <summary>
    /// ASceneElement 的参数工厂，类似 ParamFactory。
    /// </summary>
    [Serializable]
    abstract public class AElementFactory
    {
        /// <summary>
        /// 判断参数工厂是否可用。
        /// </summary>
        abstract public bool IsNull();

        /// <summary>
        /// 根据工厂参数创建 ASceneElement 实例。
        /// </summary>
        /// <returns>ASceneElement 实例。</returns>
        abstract public ASceneElement Create();
    }
}