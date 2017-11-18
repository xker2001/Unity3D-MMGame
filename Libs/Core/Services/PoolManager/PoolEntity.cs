using UnityEngine;
using System.Collections.Generic;
using System;

namespace MMGame
{
    /// <summary>
    /// 配合 PoolManager 使用的专用组件，由 PoolManager 自动为所创建的物体添加。
    /// </summary>
    [DisallowMultipleComponent]
    public class PoolEntity : MonoBehaviour
    {
        /// <summary>
        /// 实例在 PoolManager 大数组中的索引。
        /// </summary>
        internal int Index { get; set; } // global index in pool manager

        /// <summary>
        /// 用于创建本实例的 prefab。
        /// </summary>
        internal Transform Prefab { get; set; }

        /// <summary>
        /// 实例的初始缩放值。
        /// </summary>
        internal Vector3 OriginalScale { get; set; }

        /// <summary>
        /// 本实例所属的 PoolManager.PrefabPool。
        /// </summary>
        internal PoolManager.PrefabPool PrefabPool { get; set; }

        /// <summary>
        /// 本实例所属的对象池节点。
        /// </summary>
        internal Transform Group { get; set; }

        /// <summary>
        /// 本实例所属的对象池的名称。
        /// </summary>
        internal string PoolName { get; set; }

        /// <summary>
        /// 本实例所属的 PoolManager.TransformPool。
        /// </summary>
        internal PoolManager.TransformPool TransformPool { get; set; }

        /// <summary>
        /// 实例是否已经被对象池回收。
        /// </summary>
        internal bool IsDespawned { get; set; }

        /// <summary>
        /// 实例被回收之后的回调函数，通过 PoolManager.SetOnDespawnedCallback() 设置。
        /// </summary>
        internal Action OnDespawnedCallback { get; set; }

        private PoolBehaviour[] ownComps; // 自身的 PoolBehaviour 组件
        private PoolBehaviour[][] childrenComps; // 所有子级（多层）的 PoolBehaviour 组件
        private bool hasGottenComps;

        void OnDestroy()
        {
            ownComps = null;
            childrenComps = null;
        }

        /// <summary>
        /// 获取 prefab 中各 PoolBehaviour 组件。
        /// 必须在组件结构被修改前获取。。
        /// </summary>
        private void GetComps()
        {
            ownComps = GetComponents<PoolBehaviour>();

            Transform xform = transform;

            childrenComps = new PoolBehaviour[xform.childCount][];

            for (int i = 0; i < xform.childCount; i++)
            {
                childrenComps[i] = xform.GetChild(i).GetComponentsInChildren<PoolBehaviour>();
            }

            hasGottenComps = true;
        }

        /// <summary>
        /// 由 PoolManager.DoSpawn() 调用。
        /// </summary>
        internal virtual void OnSpawn()
        {
            if (!hasGottenComps)
            {
                GetComps();
            }

            // 执行自己各组件的 OnSpawn。如果在此过程中在子级创建了新对象池实例，
            // 新实例的 OnSpawn 应该由该创建的 DoSpawn 调用执行。
            for (int i = 0; i < ownComps.Length; i++)
            {
                ownComps[i].OnSpawn();
            }

            // Prefab 状态下的子级（多层）的组件
            for (int i = 0; i < childrenComps.Length; i++)
            {
                for (int j = 0; j < childrenComps[i].Length; j++)
                {
                    childrenComps[i][j].OnSpawn();
                }
            }
        }

        /// <summary>
        /// 由 PoolManager.Despawn() 调用。
        /// </summary>
        internal virtual void OnDespawn()
        {
            // 执行自己各组件的 OnDespawn。如果持有子级对象池实例，应该在这里全部释放完毕。
            for (int i = 0; i < ownComps.Length; i++)
            {
                ownComps[i].OnDespawn();
            }

            // Prefab 状态下的子级（多层）的组件
            for (int i = 0; i < childrenComps.Length; i++)
            {
                for (int j = 0; j < childrenComps[i].Length; j++)
                {
                    childrenComps[i][j].OnDespawn();
                }
            }
        }
    }
}