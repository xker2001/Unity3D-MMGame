using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// 对象池根节点组件。
    /// </summary>
    [DisallowMultipleComponent]
    public class ObjectPools : MonoBehaviour
    {
        private void OnDisable()
        {
            PoolManager.DestroyAll();
        }
    }
}