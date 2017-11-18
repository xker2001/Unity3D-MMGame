using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// 与 MMGame.PoolManager 配合使用的组件基类。
    /// 注意
    /// 1. 组件并非一定要继承自 PoolBehaviour，如果不实现 OnSpawn() 和 OnDespawn()，组件应当直接
    ///    继承自 MonoBehaviour 以减少对 PoolManager 的依赖。
    /// 2. 应当认为每次创建的对象池实例都是完全一致的，在实例活动期间对组件和层级的任何改变应当在被回收
    ///    前重置为原始状态。为此应当确保：
    ///    a. 在 OnSpawn() 中保证所有有状态的成员被重置；
    ///    b. 在 OnDespawn() 中保证所有在活动期创建和持有的资源被释放。
    /// </summary>
    /// 
    /// OnSpawn() 和 OnDespawn() 设置为 abstract 而不是 virtual 的原因是为了提醒使用者，如果不需
    /// 要实现这两个方法，请直接继承 MonoBehaviour 而不是 PoolBehaviour。
    abstract public class PoolBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 在被对象池创建、OnEnable() 之前执行，通常用于重置组件状态。
        /// </summary>
        virtual public void OnSpawn() {}

        /// <summary>
        /// 在被对象池回收、OnDisable() 之前执行，通常用于释放组件在活动周期内创建并持有的资源。
        /// </summary>
        virtual public void OnDespawn() {}
    }
}