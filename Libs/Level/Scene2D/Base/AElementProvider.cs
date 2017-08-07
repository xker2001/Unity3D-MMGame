using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// ASceneElement 供给器，可以包含控制场景元素多样化的复杂逻辑。
    /// AElementProvider 通常由 Spawner 及其下属组件使用。
    /// </summary>
    abstract public class AElementProvider : MonoBehaviour
    {
        /// <summary>
        /// 获取下一个场景元素。
        /// </summary>
        /// <returns>新的场景元素。如果没有则返回 null。</returns>
        abstract public ASceneElement GetNext();
    }
}