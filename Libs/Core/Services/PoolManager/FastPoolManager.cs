using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    /// <summary>
    /// 快速对象池管理器。
    /// </summary>
    /// <remarks>
    /// 快速对象池管理用于对产出和回收速度要求极高的场合（如雪花）。它去除了对各种回调的支持，
    /// 支持基于 prefab 的对象池和基于单一组件的对象池。使用方法为根据指定的 prefab 或组
    /// 件类型获取对象池，然后用对象池产生、回收实例。
    /// 使用时将该组件挂接到物体，放置到场景中。物体会跨场景存在。
    /// </remarks>
    public class FastPoolManager : MonoBehaviour
    {
        private static FastPoolManager instance;

        private static FastPoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (FastPoolManager) FindObjectOfType(typeof(FastPoolManager));
                }

                return instance;
            }
        }

        [SerializeField] private int hotToColdPerFrame = 20;

        /// <summary>
        /// 按 prefab 索引的对象池字典。
        /// </summary>
        private static Dictionary<Transform, FastPrefabPool> prefabPoolDic = new Dictionary<Transform, FastPrefabPool>();

        /// <summary>
        /// 按组件名称名称索引的对象池字典。
        /// </summary>
        private static Dictionary<string, FastComponentPool> componentPoolDic =
            new Dictionary<string, FastComponentPool>();

        /// <summary>
        /// 对象池根节点。
        /// </summary>
        private static Transform root;

        /// <summary>
        /// 对象池实例总数量。
        /// </summary>
        public static int Count { get; set; }

        /// <summary>
        /// 对象池根节点名称。
        /// </summary>
        /// <remarks>设置为公有可读是单元测试需要</remarks>
        public static string RootName
        {
            get { return "FastObjectPools"; }
        }

        /// <summary>
        /// 每帧可以从热备用状态改成冷备用状态的对象池实例的最大数量。
        /// </summary>
        /// <remarks> 为需要大量快速产出/回收的场合设置更大的值。</remarks>
        public static int HotToColdPerFrame
        {
            get { return Instance.hotToColdPerFrame; }
        }


        // ------------------------------------------------------

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            CreateRoot();
        }

        void OnDestroy()
        {
            DestroyAll();
        }

        void LateUpdate()
        {
            foreach (FastPool pool in prefabPoolDic.Values)
            {
                pool.HotToCold();
            }

            foreach (FastPool pool in componentPoolDic.Values)
            {
                pool.HotToCold();
            }
        }

        // ------------------------------------------------------

        /// <summary>
        /// 获取指定 prefab 的对象池。
        /// </summary>
        /// <param name="prefab">Prefab</param>
        /// <returns>对象池对象。</returns>
        public static FastPool GetPool(Transform prefab)
        {
            CreateRoot();

            FastPrefabPool pool;

            if (!prefabPoolDic.TryGetValue(prefab, out pool))
            {
                pool = new FastPrefabPool(prefab);
                pool.Root = root;
                prefabPoolDic.Add(prefab, pool);
            }

            return pool;
        }

        /// <summary>
        /// 获取指定组件类型的对象池。
        /// </summary>
        /// <param name="component">组件类型。</param>
        /// <returns>对象池对象。</returns>
        public static FastPool GetPool(Component component)
        {
            CreateRoot();

            FastComponentPool pool;
            string name = component.GetType().FullName;

            if (!componentPoolDic.TryGetValue(name, out pool))
            {
                pool = new FastComponentPool(component);
                pool.Root = root;
                componentPoolDic.Add(name, pool);
            }

            return pool;
        }

        /// <summary>
        /// 销毁指定 prefab 的对象池。
        /// </summary>
        /// <param name="prefab">Prefab。</param>
        public static void DestroyPool(Transform prefab)
        {
            FastPrefabPool pool;

            if (prefabPoolDic.TryGetValue(prefab, out pool))
            {
                pool.Destroy();
                prefabPoolDic.Remove(prefab);
            }
        }

        /// <summary>
        /// 销毁指定组件类型的对象池。
        /// </summary>
        /// <param name="componentName">组件名称。</param>
        public static void DestroyPool(string componentName)
        {
            FastComponentPool pool;

            if (componentPoolDic.TryGetValue(componentName, out pool))
            {
                pool.Destroy();
                componentPoolDic.Remove(componentName);
            }
        }

        /// <summary>
        /// 销毁所有对象池。
        /// </summary>
        public static void DestroyAll()
        {
            foreach (FastPool pool in prefabPoolDic.Values)
            {
                pool.Destroy();
            }

            prefabPoolDic.Clear();

            foreach (FastPool pool in componentPoolDic.Values)
            {
                pool.Destroy();
            }

            componentPoolDic.Clear();
        }

        /// <summary>
        /// 创建对象池根节点。如果已经存在则什么也不做。
        /// </summary>
        private static void CreateRoot()
        {
            if (root)
            {
                return;
            }

            GameObject go = new GameObject(RootName);
            go.AddComponent<FastObjectPools>().enabled = true;
            root = go.transform;
        }
    }

    // ------------------------------------------------------

    /// <summary>
    /// 快速对象池基类。
    /// </summary>
    abstract public class FastPool
    {
        private HashSet<Transform> spawnedPool = new HashSet<Transform>();
        private Queue<Transform> despawnedPool = new Queue<Transform>(256);
        private Queue<Transform> hotDespawnedPool = new Queue<Transform>(256);

        public string Name { get; set; }
        public Transform Root { get; set; }

        /// <summary>
        /// 创建新的对象池实例。
        /// </summary>
        /// <param name="position">创建后的初始位置。</param>
        /// <param name="rotation">创建后的初始方向。</param>
        /// <returns>对象池实例。</returns>
        protected abstract Transform Instantiate(Vector3 position, Quaternion rotation);

        /// <summary>
        /// 产出一个对象池实例。
        /// </summary>
        /// <param name="position">出生的位置。</param>
        /// <param name="rotation">出生的方向。</param>
        /// <returns>对象池实例。</returns>
        public Transform Spawn(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            Transform xform;

            // 从热备用集合中取出
            if (hotDespawnedPool.Count > 0)
            {
                xform = hotDespawnedPool.Dequeue();
            }
            // 从冷备用集合中取出
            else if (despawnedPool.Count > 0)
            {
                xform = despawnedPool.Dequeue();
            }
            // 创建新的对象池实例
            else
            {
                xform = Instantiate(position, rotation);
                FastPoolManager.Count += 1;
            }

            // 放进活跃池，设置位置方向父节点，激活，搞定。
            spawnedPool.Add(xform);
            xform.position = position;
            xform.rotation = rotation;
            xform.parent = Root;

            if (!xform.gameObject.activeSelf)
            {
                xform.gameObject.SetActive(true);
            }

            return xform;
        }

        /// <summary>
        /// 回收一个对象池实例到冷备用状态。
        /// </summary>
        /// <param name="xform">对象池实例。</param>
        public void Despawn(Transform xform)
        {
            if (spawnedPool.Remove(xform))
            {
                despawnedPool.Enqueue(xform);
                xform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 回收一个对象池实例到热备用状态。
        /// </summary>
        /// <param name="xform">对象池实例。</param>
        public void HotDespawn(Transform xform)
        {
            if (spawnedPool.Remove(xform))
            {
                hotDespawnedPool.Enqueue(xform);
            }
        }

        /// <summary>
        /// 将热备用的对象池实例改为冷备用。
        /// </summary>
        public void HotToCold()
        {
            int count = FastPoolManager.HotToColdPerFrame;

            while (hotDespawnedPool.Count > 0 && count > 0)
            {
                despawnedPool.Enqueue(hotDespawnedPool.Dequeue());
                count -= 1;
            }
        }

        /// <summary>
        /// 销毁对象池中所有实例，清空对象池。
        /// </summary>
        public void Destroy()
        {
            foreach (Transform xform in spawnedPool)
            {
                if (xform)
                {
                    Object.Destroy(xform.gameObject);
                }
            }

            spawnedPool.Clear();

            while (despawnedPool.Count > 0)
            {
                Object.Destroy(despawnedPool.Dequeue());
            }

            while (hotDespawnedPool.Count > 0)
            {
                Object.Destroy(hotDespawnedPool.Dequeue());
            }
        }
    }

    /// <summary>
    /// 基于 prefab 创建新实例的对象池。
    /// </summary>
    public class FastPrefabPool : FastPool
    {
        private Transform prefab;

        public FastPrefabPool(Transform prefab)
        {
            this.prefab = prefab;
        }

        protected override Transform Instantiate(Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(prefab, position, rotation) as Transform;
        }
    }

    /// <summary>
    /// 基于组件类型创建新实例的对象池。
    /// 具体为创建空对象，再挂接指定类型的组件。
    /// </summary>
    public class FastComponentPool : FastPool
    {
        private Component component;

        public FastComponentPool(Component component)
        {
            this.component = component;
        }

        protected override Transform Instantiate(Vector3 position, Quaternion rotation)
        {
            System.Type type = component.GetType();
            GameObject obj = new GameObject(type.Name);
            obj.AddComponent(type);
            return obj.transform;
        }
    }
}