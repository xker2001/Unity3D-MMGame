using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// 与 MMGame.PoolManager 配合使用的组件基类。
    /// 注意
    /// 1. 组件并非一定要继承自 PoolBehaviour，如果不实现 ResetForSpawn() 和 ReleaseForDespawn()，
    ///    组件应当直接继承自 MonoBehaviour 以减少对 PoolManager 的依赖。
    /// 2. 应当认为每次创建的对象池实例都是完全一致的，在实例活动期间对组件和层级的任何改变应当在被回收
    ///    前重置为原始状态。为此应当确保：
    ///    a. 在 ResetForSpawn() 中保证所有有状态的成员被重置；
    ///    b. 在 ReleaseForDespawn() 中保证所有在活动期创建和持有的资源被释放。
    /// </summary>
    abstract public class PoolBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 被对象池创建、OnEnable() 之前执行的回调函数，用于重置状态。
        /// </summary>
        abstract public void ResetForSpawn();

        /// <summary>
        /// 在被对象池回收、OnDisable() 之前执行的回调函数，用于释放占用的资源。
        /// </summary>
        abstract public void ReleaseForDespawn();
    }
}