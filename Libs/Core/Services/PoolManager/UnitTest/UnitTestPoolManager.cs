using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    public class UnitTestPoolManager : UnitTest
    {
        public Transform PrefabNormal;
        public ParticleSystem PrefabParticle;
        public Transform PrefabOnSpawn;

        // TODO: 循环粒子未测
        // public ParticleSystem PrefabLoopParticle;

        private string poolName = "UnitTestForPoolManager";
        private string prefabName = "UnitTest/PrefabNormal";

        #region Spawn

        /// <summary>
        /// 对象池第一次使用，创建根节点等。
        /// </summary>
        [TestMethod(0)]
        public void TestFirstSpawn()
        {
            Transform inst;

            // 根节点未创建
            IsNull(GameObject.Find(PoolManager.RootName));

            // Spawn 一个实例
            inst = PoolManager.Spawn(poolName, prefabName);

            // 根节点（ObjectPools）已创建
            IsNotNull(GameObject.Find(PoolManager.RootName));

            // 以 poolName 为名的子节点也被创建
            Transform group = PoolManager.GetGroup(poolName);
            IsNotNull(group);

            // 子节点的名称与 poolName 相同
            AreEqual(group.name, poolName);

            // 创建的实例上被添加了 PoolEntity 组件
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            IsNotNull(comp);
        }

        /// <summary>
        /// 根据 prefab 名称创建一个对象池实例，
        /// 检查层级、PoolEntity 参数等。
        /// </summary>
        [TestMethod(1)]
        public void TestSpawnPrefabName()
        {
            Transform inst;
            inst = PoolManager.Spawn(poolName, prefabName);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.parent, group);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, Prefab.Load(prefabName));

            // position && rotation（默认，未测）
        }


        /// <summary>
        /// 根据 prefab 名称带初始位置和旋转参数创建一个对象池实例，
        /// 检查层级、PoolEntity 参数、位置和旋转等。
        /// </summary>
        [TestMethod(2)]
        public void TestSpawnPrefabNamePosRot()
        {
            Vector3 pos = new Vector3(1, 2, 3);
            Quaternion rot = new Quaternion(1, 2, 3, 4);

            Transform inst;
            inst = PoolManager.Spawn(poolName, prefabName, pos, rot);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.parent, group);

            // 已设置正确的位置和旋转
            AreEqual(inst.position, pos);
            AreEqual(inst.rotation, rot);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, MMGame.Prefab.Load(prefabName));
        }

        /// <summary>
        /// 根据 prefab 创建一个对象池实例，
        /// 检查层级、PoolEntity 参数等。
        /// </summary>
        [TestMethod(3)]
        public void TestSpawnPrefab()
        {
            Transform inst;
            inst = PoolManager.Spawn(poolName, PrefabNormal);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.parent, group);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, PrefabNormal);

            // position && rotation（默认，未测）
        }

        /// <summary>
        /// 根据 prefab 带初始位置和旋转参数创建一个对象池实例，
        /// 检查层级、PoolEntity 参数、位置和旋转等。
        /// </summary>
        [TestMethod(4)]
        public void TestSpawnPrefabPosRot()
        {
            Vector3 pos = new Vector3(1, 2, 3);
            Quaternion rot = new Quaternion(1, 2, 3, 4);

            Transform inst;
            inst = PoolManager.Spawn(poolName, PrefabNormal, pos, rot);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.parent, group);

            // 已设置正确的位置和旋转
            AreEqual(inst.position, pos);
            AreEqual(inst.rotation, rot);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, PrefabNormal);
        }

        /// <summary>
        /// 根据 prefab 创建一个对象池粒子实例，
        /// 检查层级、PoolEntity 参数等。
        /// </summary>
        [TestMethod(5)]
        public void TestSpawnParticle()
        {
            ParticleSystem inst;
            inst = PoolManager.Spawn(poolName, PrefabParticle);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.transform.parent, group);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, PrefabParticle.transform);

            // position && rotation（默认，未测）
        }

        /// <summary>
        /// 根据 prefab 带初始位置和旋转参数创建一个对象池粒子实例，
        /// 检查层级、PoolEntity 参数、位置和旋转等。
        /// </summary>
        [TestMethod(6)]
        public void TestSpawnParticlePosRot()
        {
            Vector3 pos = new Vector3(1, 2, 3);
            Quaternion rot = new Quaternion(1, 2, 3, 4);

            ParticleSystem inst;
            inst = PoolManager.Spawn(poolName, PrefabParticle, pos, rot);
            PoolEntity comp = inst.GetComponent<PoolEntity>();
            Transform group = PoolManager.GetGroup(poolName);

            // 实例被创建
            IsNotNull(inst);

            // 实例被挂接在 poolName 节点下
            AreSame(inst.transform.parent, group);

            // 已设置正确的位置和旋转
            AreEqual(inst.transform.position, pos);
            AreEqual(inst.transform.rotation, rot);

            // PoolEntity 的参数都正确
            AreEqual(comp.PoolName, poolName);
            AreSame(comp.Group, group);
            AreSame(comp.Prefab, PrefabParticle.transform);
        }

        #endregion Spawn

        #region Despawn

        private Transform instNormal;
        private ParticleSystem instParticle;
        private ParticleSystem instLoopParticle;

        /// <summary>
        /// 准备：
        /// 创建一些实例和粒子实例，修改它们的层级。
        /// </summary>
        [TestRun(20)]
        public void TestPrepareForDespawn()
        {
            instNormal = PoolManager.Spawn(poolName, PrefabNormal);
            instNormal.parent = transform;

            // TODO: 粒子自毁未测
            // instParticle = PoolManager.Spawn(poolName, PrefabParticle);
            // instParticle.transform.parent = transform;
        }


        /// <summary>
        /// Despawn 实例并检查结果。
        /// </summary>
        [TestMethod(21)]
        public void TestDespawnTransform()
        {
            // 检测当前状态
            IsTrue(instNormal.gameObject.activeSelf);
            AreSame(instNormal.parent, transform);

            // 执行 Despawn
            PoolManager.Despawn(instNormal);

            // 不再是激活状态
            IsFalse(instNormal.gameObject.activeSelf);
            // 被重新挂接到对象池节点
            AreSame(instNormal.parent, PoolManager.GetGroup(poolName));
        }


        /// <summary>
        /// 准备：
        /// 创建指定名称的对象池节点。
        /// </summary>
        [TestRun(23)]
        public void TestPrepareForDespawnPool()
        {
            // 本来没有节点
            IsFalse(PoolManager.HasPool("ToBeDespawned"));

            // Spawn 一些实例
            for (int i = 0; i < 100; i++)
            {
                PoolManager.Spawn("ToBeDespawned", PrefabNormal);
            }

            // 节点已经被生成了
            IsTrue(PoolManager.HasPool("ToBeDespawned"));
        }

        /// <summary>
        /// Despawn 对象池节点并检查结果。
        /// </summary>
        [TestMethod(24)]
        public void TestDespawnPool()
        {
            // 对象池中的实例数量为 100 个
            HashSet<int> items = PoolManager.GetItems("ToBeDespawned");
            AreEqual(items.Count, 100);

            // Despawn
            PoolManager.DespawnPool("ToBeDespawned");

            // 对象池中的实例数量为 0
            items = PoolManager.GetItems("ToBeDespawned");
            AreEqual(items.Count, 0);
        }

        #endregion Despawn

        #region OnSpawn() and OnDespawn()

        private Transform instOnSpawn;

        /// <summary>
        /// 第一次调用 OnSpawn()。
        /// 首次通过反射创建方法字典。
        /// </summary>
        [TestMethod(26)]
        public void TestFirstOnSpawn()
        {
            instOnSpawn = PoolManager.Spawn(poolName, PrefabOnSpawn);

            // OnSpawn() 被正确调用
            UnitTestPoolManagerEntity[] comps = instOnSpawn.GetComponentsInChildren<UnitTestPoolManagerEntity>();

            for (int i = 0; i < comps.Length; i++)
            {
                IsTrue(comps[i].OnSpawnCalled);
            }
        }

        /// <summary>
        /// 第一次调用 OnDespawn()。
        /// </summary>
        [TestMethod(27)]
        public void TestFirstOnDespawn()
        {
            PoolManager.Despawn(instOnSpawn);

            // OnDespawn() 被正确调用
            UnitTestPoolManagerEntity[] comps = instOnSpawn.GetComponentsInChildren<UnitTestPoolManagerEntity>();

            for (int i = 0; i < comps.Length; i++)
            {
                IsTrue(comps[i].OnDespawnCalled);
            }
        }

        /// <summary>
        /// 第二次调用 OnSpawn()。
        /// 通过方法字典获取方法并调用。
        /// </summary>
        [TestMethod(28)]
        public void TestSecondOnSpawn()
        {
            instOnSpawn = PoolManager.Spawn(poolName, PrefabOnSpawn);

            // OnSpawn() 被正确调用
            UnitTestPoolManagerEntity[] comps = instOnSpawn.GetComponentsInChildren<UnitTestPoolManagerEntity>();

            for (int i = 0; i < comps.Length; i++)
            {
                IsTrue(comps[i].OnSpawnCalled);
            }
        }

        /// <summary>
        /// 第二次调用 OnDespawn()。
        /// </summary>
        [TestMethod(29)]
        public void TestSecondOnDespawn()
        {
            PoolManager.Despawn(instOnSpawn);

            // OnDespawn() 被正确调用
            UnitTestPoolManagerEntity[] comps = instOnSpawn.GetComponentsInChildren<UnitTestPoolManagerEntity>();

            for (int i = 0; i < comps.Length; i++)
            {
                IsTrue(comps[i].OnDespawnCalled);
            }
        }

        #endregion

        #region GetItems

        private string poolNameDestroy = "PoolToBeDestroyed";
        private Transform[] insts = new Transform[100];

        /// <summary>
        /// 准备：
        /// 生成 100 个实例。
        /// </summary>
        [TestRun(30)]
        public void TestPrepareForDestroyPoolSpawn()
        {
            for (int i = 0; i < 100; i++)
            {
                insts[i] = PoolManager.Spawn(poolNameDestroy, PrefabNormal);
            }
        }

        /// <summary>
        /// Despawn 掉 50 个。
        /// </summary>
        [TestRun(31)]
        public void TestPrepareForDestroyPoolDespawn()
        {
            for (int i = 0; i < 50; i++)
            {
                PoolManager.Despawn(insts[i]);
            }
        }

        /// <summary>
        /// 检查剩余的活动的对象池实例数量。
        /// </summary>
        [TestMethod(32)]
        public void TestGetItems()
        {
            AreEqual(PoolManager.GetItems(poolNameDestroy).Count, 50);
        }

        #endregion GetItems

        #region Preload

        private string poolNamePreload = "PoolForPreload";

        /// <summary>
        /// 用每帧 5 个的速度创建对象池实例。
        /// </summary>
        [TestRun(50)]
        public void TestPrepareForPreload()
        {
            PoolManager.Preload(poolNamePreload, PrefabNormal, 100, 5);
        }

        /// <summary>
        /// 1 帧后检查产生了 5 个对象池实例。
        /// </summary>
        [TestMethod(51)]
        public void TestPreload5()
        {
            AreEqual(PoolManager.GetInactiveItems(poolNamePreload).Count, 5);
        }

        /// <summary>
        /// 10 帧后检查产生了 50 个对象池实例。
        /// </summary>
        [TestMethod(60)]
        public void TestPreload50()
        {
            AreEqual(PoolManager.GetInactiveItems(poolNamePreload).Count, 50);
        }

        /// <summary>
        /// 20 帧后检查产生了 100 个对象池实例。
        /// </summary>
        [TestMethod(70)]
        public void TestPreload100()
        {
            AreEqual(PoolManager.GetInactiveItems(poolNamePreload).Count, 100);
        }

        /// <summary>
        /// 再过 10 帧，对象池实例稳定在预期的 100 个。
        /// </summary>
        [TestMethod(80)]
        public void TestPreload()
        {
            AreEqual(PoolManager.GetInactiveItems(poolNamePreload).Count, 100);
        }

        #endregion Preload

        /// <summary>
        /// 测试 GetPrefab() 方法。
        /// </summary>
        [TestMethod(90)]
        public void TestGetPrefab()
        {
            Transform inst;
            inst = PoolManager.Spawn(poolName, PrefabNormal);
            AreSame(PrefabNormal, PoolManager.GetPrefab(inst));
            AreSame(PrefabNormal, PoolManager.GetPrefab(inst.gameObject));
        }


        /// <summary>
        /// 销毁整个对象池。
        /// </summary>
        [TestMethod(100)]
        public void TestDestroyAll()
        {
            PoolManager.DestroyAll();
        }

        /// <summary>
        /// 全场对象池实例数量为 0。
        /// </summary>
        [TestMethod(101)]
        public void TestDestroyAllResult()
        {
            PoolEntity[] entities = FindObjectsOfType(typeof(PoolEntity)) as PoolEntity[];
            AreEqual(entities.Length, 0);
        }

        #region Spawn to another pool

        private Transform reusedEntity;

        /// <summary>
        /// 在 Pool01 中创建一个对象池实例。
        /// </summary>
        [TestMethod(110)]
        public void TestSpawnToAnotherPoolSpawn01()
        {
            reusedEntity = PoolManager.Spawn("Pool01", PrefabNormal);
        }

        /// <summary>
        /// Despawn 该实例。
        /// </summary>
        [TestMethod(111)]
        public void TestSpawnToAnotherPoolDespawn01()
        {
            PoolManager.Despawn(reusedEntity);
        }

        /// <summary>
        /// 在 Pool02 中创建一个对象池实例，取回了之前 Pool01 使用过的实例。
        /// </summary>
        [TestMethod(112)]
        public void TestSpawnToAnotherPoolSpawn02()
        {
            AreSame(reusedEntity, PoolManager.Spawn("Pool02", PrefabNormal));
        }

        /// <summary>
        /// 再 Despawn 该实例。
        /// </summary>
        [TestMethod(113)]
        public void TestSpawnToAnotherPoolDespawn02()
        {
            PoolManager.Despawn(reusedEntity);
        }

        /// <summary>
        /// 重新在 Pool01 中创建。
        /// </summary>
        [TestMethod(114)]
        public void TestSpawnToAnotherPoolSpawn01Again()
        {
            AreSame(reusedEntity, PoolManager.Spawn("Pool01", PrefabNormal));
        }

        #endregion Spawn to another pool
    }
}