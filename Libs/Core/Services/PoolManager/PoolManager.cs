using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

namespace MMGame
{
    /// <summary>
    /// 对象池管理器。
    /// 使用方法：
    /// 将该组件挂接到物体，放置到场景中。物体会跨场景存在。如果有必要，将 PoolManager 的脚本执行顺序调到优先。
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager instance;

        private static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (PoolManager) FindObjectOfType(typeof(PoolManager));
                }

                return instance;
            }
        }

        /// <summary>
        /// 通过 Transform 节点名称进行索引的对象池。
        /// </summary>
        internal class TransformPool
        {
            /// <summary>
            /// 对象池的名称。
            /// </summary>
            public string PoolName;

            /// <summary>
            /// 对象池的 Transform 节点。
            /// </summary>
            public Transform Group;

            /// <summary>
            /// 活跃对象索引集合（在 allTransforms 中的索引）。
            /// </summary>
            public HashSet<int> EnabledPool = new HashSet<int>();

            /// <summary>
            /// 不活跃对象索引集合。
            /// </summary>
            public HashSet<int> DisabledPool = new HashSet<int>();
        }

        /// <summary>
        /// 通过 prefab 进行索引的对象池。
        /// </summary>
        internal class PrefabPool
        {
            /// <summary>
            /// 该 prefab 对象池中实例化的数量，用于名称编号。
            /// </summary>
            public int Count;

            public Transform Prefab;

            /// <summary>
            /// 活跃对象索引集合。
            /// </summary>
            //            public HashSet<int> EnabledPool = new HashSet<int>();
            /// <summary>
            /// 不活跃对象索引集合。
            /// </summary>
            public Stack<int> DisabledPool = new Stack<int>();
        }

        // ------------------------------------------------------

        /// <summary>
        /// 对象池中能容纳的实例的最大数量。
        /// </summary>
        [SerializeField]
        private int maxCount = 10000;

        /// <summary>
        /// 对象池根节点名称。
        /// </summary>
        /// <remarks>设置为公有单元测试需要</remarks>
        public const string RootName = "ObjectPools";

        /// <summary>
        /// 所有的对象实例。
        /// </summary>
        private static Transform[] allTransforms;

        /// <summary>
        /// 所有的 PoolEntity 组件。
        /// </summary>
        private static PoolEntity[] allPoolEntities;

        /// <summary>
        /// 实例的总数量。
        /// </summary>
        private static int count;

        /// <summary>
        /// 对象池根节点。
        /// </summary>
        private static Transform root;

        /// <summary>
        /// 按节点名称索引的对象池字典。
        /// </summary>
        private static Dictionary<string, TransformPool> transformPoolDic = new Dictionary<string, TransformPool>();

        /// <summary>
        /// 按 prefab 索引的对象池字典。
        /// </summary>
        private static Dictionary<Transform, PrefabPool> prefabPoolDic = new Dictionary<Transform, PrefabPool>();

        // ------------------------------------------------------

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            allTransforms = new Transform[maxCount];
            allPoolEntities = new PoolEntity[maxCount];
        }

        /// <summary>
        /// 强制在根节点被销毁前销毁所有已创建的实例。
        /// 原因：当场景中存在大量对象池创建的实例时，退出 play 模式会导致长时间卡死。
        /// </summary>
        void OnDestroy()
        {
            DestroyAll();
        }

        /// <summary>
        /// 检查指定名称的对象池是否存在。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <returns>存在返回 true，反之返回 false。</returns>
        public static bool HasPool(string poolName)
        {
            return transformPoolDic.ContainsKey(poolName);
        }

        /// <summary>
        /// 根据实例在数组中的索引获取实例。
        /// </summary>
        /// <param name="index">实例在数组中的索引。</param>
        /// <returns>实例</returns>
        public static Transform GetByIndex(int index)
        {
            Assert.IsTrue(index < allTransforms.Length && index >= 0);
            return allTransforms[index];
        }

        /// <summary>
        /// 根据名称获取对象池的 Transform 节点。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <returns>对象池 Transform。</returns>
        public static Transform GetGroup(string poolName)
        {
            TransformPool pool;
            transformPoolDic.TryGetValue(poolName, out pool);
            return pool != null ? pool.Group : null;
        }

        /// <summary>
        /// 将实例重新挂接到所属的对象池 Transform 上。
        /// </summary>
        /// <param name="xform">实例。</param>
        public static void Reparent(Transform xform)
        {
            xform.parent = xform.GetComponent<PoolEntity>().Group;
        }

        /// <summary>
        /// 获取实例的 prefab。
        /// </summary>
        /// <param name="xform">实例。</param>
        /// <returns>Prefab。</returns>
        public static Transform GetPrefab(Transform xform)
        {
            return xform.GetComponent<PoolEntity>().Prefab;
        }

        /// <summary>
        /// 获取实例的 prefab。
        /// </summary>
        /// <param name="go">实例。</param>
        /// <returns>Prefab。</returns>
        public static Transform GetPrefab(GameObject go)
        {
            return go.GetComponent<PoolEntity>().Prefab;
        }

        /// <summary>
        /// 获取指定名称对象池中所有活跃的实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <returns>实例集合。</returns>
        /// <exception cref="ApplicationException">没有找到指定名称的对象池。</exception>
        public static HashSet<int> GetItems(string poolName)
        {
            TransformPool pool;

            if (transformPoolDic.TryGetValue(poolName, out pool))
            {
                return pool.EnabledPool;
            }
            else
            {
                throw new ApplicationException(string.Format(
                                                             "PoolManager.GetItems: pool named {0} does not exist!",
                                                             poolName));
            }
        }

        /// <summary>
        /// 获取指定名称对象池中所有不活跃的实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <returns>实例集合。</returns>
        /// <exception cref="ApplicationException">没有找到指定名称的对象池。</exception>
        public static HashSet<int> GetInactiveItems(string poolName)
        {
            TransformPool pool;

            if (transformPoolDic.TryGetValue(poolName, out pool))
            {
                return pool.DisabledPool;
            }
            else
            {
                throw new ApplicationException(string.Format(
                                                             "PoolManager.GetItems: pool named {0} does not exist!",
                                                             poolName));
            }
        }

        // ------------------------------------------------------
        // Spawn
        // ------------------------------------------------------

        /// <summary>
        /// 将一个物体加入对象池。
        /// </summary>
        /// <param name="xform">物体实例。</param>
        /// <param name="comp">物体实例上的 PoolEntity 组件。</param>
        /// <param name="pool">对象池 Transform。</param>
        /// <param name="enable">物体是否活跃。</param>
        private static void AddToTransformPool(Transform xform, PoolEntity comp, TransformPool pool, bool enable)
        {
            if (enable)
            {
                pool.EnabledPool.Add(comp.Index);
            }
            else
            {
                pool.DisabledPool.Add(comp.Index);
            }

            comp.TransformPool = pool;
            comp.Group = pool.Group;
            comp.PoolName = pool.PoolName;
            xform.SetParent(pool.Group);
        }

        /// <summary>
        /// 生成一个实例（细节执行）。
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="prefab">对象实例的 prefab。</param>
        /// <param name="position">出生的位置。</param>
        /// <param name="rotation">出生的方向。</param>
        /// <returns>实例。</returns>
        private static Transform DoSpawn(string poolName, Transform prefab,
                                         Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            PrefabPool prefabPool = GetPrefabPool(prefab);

            Transform xform;
            TransformPool transformPool;
            PoolEntity comp;

            // 没有不活跃的实例，创建一个新的。
            if (prefabPool.DisabledPool.Count == 0)
            {
                transformPool = GetTransformPool(poolName);
                xform = Instantiate(prefabPool, transformPool, true, out comp, position, rotation);
            }
            // 有不活跃的实例
            else
            {
                // 从 prefab 对象池中取出一个不活跃的实例放到活跃索引集合中
                int index = prefabPool.DisabledPool.Pop();
                xform = allTransforms[index];
                xform.position = position;
                xform.rotation = rotation;

                comp = allPoolEntities[index];
                comp.IsDespawned = false;

                // 放置到指定名称的对象池节点下
                string currentPoolName = comp.PoolName;

                if (!string.IsNullOrEmpty(currentPoolName) && currentPoolName == poolName)
                {
                    comp.TransformPool.DisabledPool.Remove(index);
                    comp.TransformPool.EnabledPool.Add(index);
                }
                else
                {
                    transformPool = GetTransformPool(poolName);

                    if (!string.IsNullOrEmpty(currentPoolName))
                    {
                        comp.TransformPool.DisabledPool.Remove(index);
                    }

                    AddToTransformPool(xform, comp, transformPool, true);
                }
            }

            if (xform.localScale != comp.OriginalScale)
            {
                xform.localScale = comp.OriginalScale;
            }

            // 调用 PoolBehaviour 组件的 ResetForSpawn() 方法
            comp.OnSpawn();

            // 激活这个实例。
            if (!xform.gameObject.activeSelf)
            {
                xform.gameObject.SetActive(true);
            }

            return xform;
        }


        /// <summary>
        /// 创建一个新的物体并挂接一个 PoolEntity 组件。
        /// </summary>
        /// <param name="prefabPool">Prefab 对象池。</param>
        /// <param name="transformPool">Transform 对象池。</param>
        /// <param name="enable">是否激活。</param>
        /// <param name="position">创建后的初始位置。</param>
        /// <param name="rotation">创建后的初始方向。</param>
        /// <returns>创建出来的实例。</returns>
        private static Transform Instantiate(PrefabPool prefabPool,
                                             TransformPool transformPool,
                                             bool enable,
                                             out PoolEntity comp,
                                             Vector3 position = default(Vector3),
                                             Quaternion rotation = default(Quaternion))
        {
            if (count >= Instance.maxCount)
            {
                throw new ApplicationException(
                                               "POOL MANAGER: \nThe number of game objects has reached its limit! Try to increase the Max Count of PoolManager.");
            }

            // 创建新的实例
            Transform xform;

            xform = Instantiate(prefabPool.Prefab, position, rotation) as Transform;
            prefabPool.Count += 1;
            xform.name += prefabPool.Count.ToString("#000");

            // 添加 PoolEntity/PoolParticle 组件

            ParticleSystem ps = xform.GetComponent<ParticleSystem>();

            comp = ps
                       ? xform.gameObject.AddComponent<PoolParticle>()
                       : xform.gameObject.AddComponent<PoolEntity>();

            comp.PrefabPool = prefabPool;
            comp.Prefab = prefabPool.Prefab;
            comp.Index = count;
            comp.OriginalScale = xform.localScale;

            // 添加到大数组
            allTransforms[count] = xform;
            allPoolEntities[count] = comp;
            count += 1;

            // 添加到 prefab 对象池
            if (enable)
            {
                comp.IsDespawned = false;
            }
            else
            {
                prefabPool.DisabledPool.Push(comp.Index);
            }

            // 添加到 Transform 对象池
            // need comp.Index
            AddToTransformPool(xform, comp, transformPool, enable);

            return xform;
        }

        /// <summary>
        /// 生成一个实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">Prefab。</param>
        /// <param name="position">生成位置。</param>
        /// <param name="rotation">生成方向。</param>
        /// <returns>实例。</returns>
        public static Transform Spawn(string poolName, Transform prefab,
                                      Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            return DoSpawn(poolName, prefab, position, rotation);
        }

        /// <summary>
        /// 生成一个实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefabName">Prefab 名称。</param>
        /// <param name="position">生成位置。</param>
        /// <param name="rotation">生成方向。</param>
        /// <returns>实例。</returns>
        public static Transform Spawn(string poolName, string prefabName,
                                      Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            return DoSpawn(poolName, Prefab.Load(prefabName), position, rotation);
        }

        /// <summary>
        /// 生成一个 ParticleSystem 类型的实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">粒子 prefab。</param>
        /// <param name="position">生成的位置。</param>
        /// <param name="rotation">生成的方向。</param>
        /// <returns>实例。</returns>
        public static ParticleSystem Spawn(string poolName, ParticleSystem prefab, Vector3 position,
                                           Quaternion rotation)
        {
            ParticleSystem ps = DoSpawn(poolName, prefab.transform, position, rotation).GetComponent<ParticleSystem>();
            return ps;
        }

        /// <summary>
        /// 生成一个 ParticleSystem 类型的实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">粒子 prefab。</param>
        /// <returns>实例。</returns>
        public static ParticleSystem Spawn(string poolName, ParticleSystem prefab)
        {
            return Spawn(poolName, prefab, default(Vector3), default(Quaternion));
        }

        // ------------------------------------------------------
        // Despawn
        // ------------------------------------------------------

        /// <summary>
        /// 检查一个实例是否已经被回收。
        /// </summary>
        /// <param name="xform">实例。</param>
        /// <returns>如果已经被回收，返回 true，反之返回 false。</returns>
        public static bool IsDespawned(Transform xform)
        {
            return xform.GetComponent<PoolEntity>().IsDespawned;
        }

        /// <summary>
        /// 回收一个实例。
        /// </summary>
        /// <param name="xform">实例。</param>
        /// <param name="reparent">是否重新挂接到所属的对象池节点。</param>
        /// <exception cref="ApplicationException">在给出的实例上没有找到 PoolEntity 组件。</exception>
        public static void Despawn(Transform xform, bool reparent = true)
        {
            PoolEntity comp = xform.GetComponent<PoolEntity>();

            if (!comp)
            {
                throw new ApplicationException(string.Format(
                                                             "Trying to despawn a game object that was not spawned by pool manager: {0}",
                                                             xform.name));
            }

            Despawn(comp.Index, reparent);
        }

        /// <summary>
        /// 回收一个实例。
        /// </summary>
        /// <param name="index">实例在大数组中的索引。</param>
        /// <param name="reparent">是否重新挂接到所属的对象池节点。</param>
        /// <exception cref="ApplicationException">实例早已被回收。</exception>
        public static void Despawn(int index, bool reparent = true)
        {
            PoolEntity comp = allPoolEntities[index];

            if (!comp)
            {
                Debug.Log("It seems that the whole object pool has been destroyed.");
                return;
            }

            TransformPool transformPool = transformPoolDic[comp.PoolName];
            PrefabPool prefabPool = prefabPoolDic[comp.Prefab];

            if (comp.IsDespawned)
            {
                throw new ApplicationException(string.Format(
                                                             "Game object seems to be despawned already: {0}",
                                                             GetByIndex(index).name));
            }

            prefabPool.DisabledPool.Push(index);
            transformPool.EnabledPool.Remove(index);
            transformPool.DisabledPool.Add(index);

            // 1. 调用所有 PoolBehaviour 组件的 ReleaseForDespawn() 方法
            // 2. 特定类型的 PoolEntity 有时候需要在 Despawn 时执行一些特定的工作，如 PoolParticle 清除粒子等
            comp.OnDespawn();
            comp.gameObject.SetActive(false);

            if (reparent)
            {
                allTransforms[index].SetParent(comp.Group);
            }

            if (comp.OnDespawnedCallback != null)
            {
                comp.OnDespawnedCallback();
                comp.OnDespawnedCallback = null;
            }

            comp.IsDespawned = true;
        }

        /// <summary>
        /// 回收一个粒子类型的实例。
        /// </summary>
        /// <param name="ps">粒子实例。</param>
        /// <param name="reparent">是否重新挂接到所属的对象池节点。</param>
        public static void Despawn(ParticleSystem ps, bool reparent = true)
        {
            Despawn(ps.transform, reparent);
        }

        /// <summary>
        /// 设置粒子类型的实例在粒子死亡后自动回收。
        /// </summary>
        /// <param name="ps">粒子类型的实例。</param>
        /// <param name="onDespawned">回收后执行的回调函数。</param>
        /// <remarks>可以多次调用来改变回调函数。</remarks>
        public static void SetAutoDespawn(ParticleSystem ps, Action onDespawned)
        {
            PoolParticle comp = ps.GetComponent<PoolParticle>();
            comp.OnDespawnedCallback = onDespawned;
            comp.CheckAndDespawnSelf();
        }

        /// <summary>
        /// 设置实例被回收后执行的回调函数。
        /// </summary>
        /// <param name="xform">实例。</param>
        /// <param name="onDespawnedCallback">回调函数。</param>
        public static void SetOnDespawnedCallback(Transform xform, Action onDespawnedCallback)
        {
            xform.GetComponent<PoolEntity>().OnDespawnedCallback = onDespawnedCallback;
        }

        /// <summary>
        /// 回收指定名称对象池中的所有实例。
        /// </summary>
        /// <param name="poolName"></param>
        /// <exception cref="ApplicationException"></exception>
        public static void DespawnPool(string poolName)
        {
            TransformPool pool;

            if (transformPoolDic.TryGetValue(poolName, out pool))
            {
                int[] indexes = new int[pool.EnabledPool.Count];
                pool.EnabledPool.CopyTo(indexes);

                for (int i = 0; i < indexes.Length; i++)
                {
                    Despawn(indexes[i]);
                }
            }
            else
            {
                Debug.LogError(string.Format("PoolManager.DespawnPool: pool named {0} does not exist!", poolName));
            }
        }

        // ------------------------------------------------------
        // Destroy
        // ------------------------------------------------------

        /// <summary>
        /// 销毁所有实例；清空所有的对象池，包括 prefab 对象池。
        /// </summary>
        public static void DestroyAll()
        {
            prefabPoolDic.Clear();
            transformPoolDic.Clear();

            for (int i = 0; i < count; i++)
            {
                if (allTransforms[i] != null)
                {
                    Destroy(allTransforms[i].gameObject);
                    allTransforms[i] = null;
                }

                allPoolEntities[i] = null;
            }

            count = 0;
        }

        // ------------------------------------------------------
        // Preload
        // ------------------------------------------------------

        /// <summary>
        /// 分批预加载实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">Prefab。</param>
        /// <param name="amount">实例总数量。</param>
        /// <param name="perAmount">每帧创建的实例数量，为 0 时创建全部。</param>
        public static void Preload(string poolName, Transform prefab, int amount, int perAmount = 0)
        {
            Instance.DoPreload(poolName, prefab, amount, perAmount);
        }

        /// <summary>
        /// 分批预加载非循环（？忘记为什么了）的粒子类型实例。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">Prefab。</param>
        /// <param name="amount">创建的实例总数量。</param>
        /// <param name="perAmount">每帧创建的实例数量，为 0 时一次性创建全部。</param>
        public static void Preload(string poolName, ParticleSystem prefab, int amount, int perAmount = 0)
        {
            Instance.DoPreload(poolName, prefab.transform, amount, perAmount);
        }

        /// <summary>
        /// 分批预加载实例（细节执行）。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <param name="prefab">Prefab。</param>
        /// <param name="amount">创建的实例总数量。</param>
        /// <param name="perAmount">每帧创建的实例数量，为 0 是一次性创建全部。</param>
        private void DoPreload(string poolName, Transform prefab, int amount, int perAmount)
        {
            PrefabPool prefabPool = GetPrefabPool(prefab);
            TransformPool transformPool = GetTransformPool(poolName);
            bool prefabEnabled = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            perAmount = perAmount <= 0 ? amount : perAmount;
            StartPreloading(prefabPool, prefabEnabled, transformPool, amount, perAmount);
        }

        /// <summary>
        /// 开始执行分批加载实例。
        /// </summary>
        /// <param name="prefabPool">Prefab 对象池。</param>
        /// <param name="prefabEnabled">Prefab 对象池活跃实例集合。</param>
        /// <param name="transformPool">Transform 对象池。</param>
        /// <param name="amount">创建的实例总数量。</param>
        /// <param name="perAmount">每帧创建的实例数量，为 0 时一次性创建全部。</param>
        internal void StartPreloading(PrefabPool prefabPool, bool prefabEnabled,
                                      TransformPool transformPool, int amount, int perAmount)
        {
            StartCoroutine(PreloadBatch(prefabPool, prefabEnabled, transformPool, amount, perAmount));
        }

        /// <summary>
        /// 分批加载实例协程。
        /// </summary>
        /// <param name="prefabPool">Prefab 对象池。</param>
        /// <param name="prefabEnabled">Prefab 对象池活跃实例集合。</param>
        /// <param name="transformPool">Transform 对象池。</param>
        /// <param name="amount">创建的实例总数量。</param>
        /// <param name="perAmount">每帧创建的实例数量，为 0 时一次性创建全部。</param>
        /// <returns>IEnumerator 对象。</returns>
        internal IEnumerator PreloadBatch(PrefabPool prefabPool, bool prefabEnabled,
                                          TransformPool transformPool, int amount, int perAmount)
        {
            yield return new WaitForEndOfFrame();

            PoolEntity comp;

            for (int i = 0; i < perAmount; i++)
            {
                Instantiate(prefabPool, transformPool, false, out comp);
                amount -= 1;

                if (amount == 0)
                {
                    prefabPool.Prefab.gameObject.SetActive(prefabEnabled);
                    yield break;
                }
            }

            StartCoroutine(PreloadBatch(prefabPool, prefabEnabled, transformPool, amount, perAmount));
        }

        // ------------------------------------------------------

        /// <summary>
        /// 创建对象池根节点。
        /// </summary>
        private static void CreateRoot()
        {
            GameObject rootObj = new GameObject(RootName);
            rootObj.AddComponent<ObjectPools>().enabled = true;
            root = rootObj.transform;
        }

        /// <summary>
        /// 创建 Transform 对象池。
        /// </summary>
        /// <param name="name">对象池名称。</param>
        /// <returns>TransformPool 对象。</returns>
        private static TransformPool CreateTransformPool(string name)
        {
            if (!root)
            {
                CreateRoot();
            }

            TransformPool pool = new TransformPool();
            pool.PoolName = name;
            pool.Group = (new GameObject(name)).transform;
            pool.Group.SetParent(root);
            transformPoolDic.Add(name, pool);
            return pool;
        }

        /// <summary>
        /// 创建 Prefab 对象池。
        /// </summary>
        /// <param name="prefab">Prefab。</param>
        /// <returns>PrefabPool 对象。</returns>
        private static PrefabPool CreatePrefabPool(Transform prefab)
        {
            PrefabPool pool = new PrefabPool();
            prefabPoolDic.Add(prefab, pool);
            pool.Prefab = prefab;
            return pool;
        }

        /// <summary>
        /// 获取 Transform 对象池，如果不存在则创建一个。
        /// </summary>
        /// <param name="poolName">对象池名称。</param>
        /// <returns>TransformPool 对象。</returns>
        private static TransformPool GetTransformPool(string poolName)
        {
            TransformPool transformPool;

            if (!transformPoolDic.TryGetValue(poolName, out transformPool))
            {
                transformPool = CreateTransformPool(poolName);
            }

            return transformPool;
        }

        /// <summary>
        /// 获取 PrefabPool 对象池，如果不存在则创建一个。
        /// </summary>
        /// <param name="prefab">Prefab。</param>
        /// <returns>PrefabPool 对象。</returns>
        private static PrefabPool GetPrefabPool(Transform prefab)
        {
            PrefabPool prefabPool;

            if (!prefabPoolDic.TryGetValue(prefab, out prefabPool))
            {
                prefabPool = CreatePrefabPool(prefab);
            }

            return prefabPool;
        }
    }
}