using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MMGame
{
    /// <summary>
    /// 用于取代 MonoBehaviour.Update/FixedUpdate/LateUpdate 的委托方法类型。
    /// </summary>
    public delegate void FastUpdate(float deltaTime);

    /// <summary>
    /// 心跳管理器。与 Unity 内建功能相比添加了编辑模式支持，同时效率更高。
    ///
    /// 使用方法：
    /// 挂接到场景中的物体，物体会跨场景存在。
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UpdateManager : MonoBehaviour
    {
        // 准备添加到 Update 列表中的方法
        private static List<FastUpdate> updatesToBeAdded = new List<FastUpdate>();
        // 准备从 Update 列表中移除的方法
        private static List<FastUpdate> updatesToBeRemoved = new List<FastUpdate>();
        // 本次心跳将要执行的 Update 方法列表
        private static List<FastUpdate> activeUpdates = new List<FastUpdate>();

        // 准备添加到 LateUpdate 列表中的方法
        private static List<FastUpdate> lateUpdatesToBeAdded = new List<FastUpdate>();
        // 准备从 LateUpdate 列表中移除的方法
        private static List<FastUpdate> lateUpdatesToBeRemoved = new List<FastUpdate>();
        // 本次心跳将要执行的 LateUpdate 方法列表
        private static List<FastUpdate> activeLateUpdates = new List<FastUpdate>();

        // 准备添加到 FixedUpdate 列表中的方法
        private static List<FastUpdate> fixedUpdatesToBeAdded = new List<FastUpdate>();
        // 准备从 FixedUpdate 列表中移除的方法
        private static List<FastUpdate> fixedUpdatesToBeRemoved = new List<FastUpdate>();
        // 本次心跳将要执行的 FixedUpdate 方法列表
        private static List<FastUpdate> activeFixedUpdates = new List<FastUpdate>();

        //--------------------------------------------------
        // 这里的回调列表使用 List 而不是 Hashset 是因为 List
        // 可以保证 Update 按照注册的先后顺写执行。
        //--------------------------------------------------

        private static double timeSinceStartup;
        private static double deltaTime;

        public static float DeltaTime
        {
            get
            {
#if UNITY_EDITOR
                return Application.isPlaying
                           ? Time.deltaTime
                           : (float) deltaTime * Time.timeScale;
#else
            return Time.deltaTime;
#endif
            }
        }

        public static float FixedDeltaTime
        {
            get { return Time.fixedDeltaTime; }
        }

        void Awake()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

#if UNITY_EDITOR

        void OnEnable()
        {
            if (FindObjectsOfType<UpdateManager>().Length > 1)
            {
                throw new ApplicationException("UpdateManager was created more than one times.");
            }

            timeSinceStartup = EditorApplication.timeSinceStartup;

            if (Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
            }
            else
            {
                EditorApplication.update += EditorUpdate;
            }
        }

        void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

#endif

        /// <summary>
        /// 向 UpdateManager 注册一个 Update 方法。
        /// 如果之前已经提交注销该方法，则取消注销申请；如果之前已经提交注册该方法，则保留注册申请。
        /// </summary>
        /// <param name="func">待注册的 Update 方法。</param>
        public static void RegisterUpdate(FastUpdate func)
        {
            updatesToBeRemoved.Remove(func);

            if (!updatesToBeAdded.Contains(func))
            {
                updatesToBeAdded.Add(func);
            }
        }

        /// <summary>
        /// 从 UpdateManager 注销一个 Update 方法。
        /// 如果之前已经提交注册该方法，则取消注册申请；如果之前已经提交注销该方法，则保留注销申请。
        /// </summary>
        /// <param name="func">待注销的 Update 方法。</param>
        public static void UnregisterUpdate(FastUpdate func)
        {
            updatesToBeAdded.Remove(func);

            if (!updatesToBeRemoved.Contains(func))
            {
                updatesToBeRemoved.Add(func);
            }
        }

        /// <summary>
        /// 向 UpdateManager 注册一个 LateUpdate 方法。
        /// </summary>
        /// <param name="func">待注册的 LateUpdate 方法。</param>
        public static void RegisterLateUpdate(FastUpdate func)
        {
            lateUpdatesToBeRemoved.Remove(func);

            if (!lateUpdatesToBeAdded.Contains(func))
            {
                lateUpdatesToBeAdded.Add(func);
            }
        }

        /// <summary>
        /// 从 UpdateManager 注销一个 LateUpdate 方法。
        /// </summary>
        /// <param name="func">待注销的 LateUpdate 方法。</param>
        public static void UnregisterLateUpdate(FastUpdate func)
        {
            lateUpdatesToBeAdded.Remove(func);

            if (!lateUpdatesToBeRemoved.Contains(func))
            {
                lateUpdatesToBeRemoved.Add(func);
            }
        }

        /// <summary>
        /// 向 UpdateManager 注册一个 FixedUpdate 方法。
        /// </summary>
        /// <param name="func">待注册的 FixedUpdate 方法。</param>
        public static void RegisterFixedUpdate(FastUpdate func)
        {
            fixedUpdatesToBeRemoved.Remove(func);

            if (!fixedUpdatesToBeAdded.Contains(func))
            {
                fixedUpdatesToBeAdded.Add(func);
            }
        }

        /// <summary>
        /// 从 UpdateManager 注销一个 FixedUpdate 方法。
        /// </summary>
        /// <param name="func">待注销的 FixedUpdate 方法。</param>
        public static void UnregisterFixedUpdate(FastUpdate func)
        {
            fixedUpdatesToBeAdded.Remove(func);

            if (!fixedUpdatesToBeRemoved.Contains(func))
            {
                fixedUpdatesToBeRemoved.Add(func);
            }
        }

        void Update()
        {
            // 非运行模式由 EditorUpdate() 负责
            if (!Application.isPlaying || Time.timeScale < Mathf.Epsilon)
            {
                return;
            }

            DoUpdateTicks(Time.deltaTime);
        }

        void LateUpdate()
        {
            DoLateUpdateTicks(Time.deltaTime);
        }

        void FixedUpdate()
        {
            DoFixedUpdateTicks(Time.fixedDeltaTime);
        }

#if UNITY_EDITOR

        private static void EditorUpdate()
        {
            // 运行模式由 Update() 负责。
            if (Application.isPlaying)
            {
                return;
            }

            // Note: 计算 deltaTime。升级到 5.5 之后可以直接使用 Time.deltaTime。
            deltaTime = EditorApplication.timeSinceStartup - timeSinceStartup;
            timeSinceStartup = EditorApplication.timeSinceStartup;

            DoUpdateTicks((float) deltaTime * Time.timeScale);
        }

#endif

        private static void DoUpdateTicks(float deltaTime)
        {
            for (int i = 0; i < updatesToBeRemoved.Count; i++)
            {
                activeUpdates.Remove(updatesToBeRemoved[i]);
            }

            updatesToBeRemoved.Clear();

            for (int i = 0; i < updatesToBeAdded.Count; i++)
            {
                activeUpdates.Add(updatesToBeAdded[i]);
            }

            updatesToBeAdded.Clear();

            for (int i = 0; i < activeUpdates.Count; i++)
            {
                activeUpdates[i](deltaTime);
            }
        }

        private static void DoLateUpdateTicks(float deltaTime)
        {
            for (int i = 0; i < lateUpdatesToBeRemoved.Count; i++)
            {
                activeLateUpdates.Remove(lateUpdatesToBeRemoved[i]);
            }

            lateUpdatesToBeRemoved.Clear();

            for (int i = 0; i < lateUpdatesToBeAdded.Count; i++)
            {
                activeLateUpdates.Add(lateUpdatesToBeAdded[i]);
            }

            lateUpdatesToBeAdded.Clear();

            for (int i = 0; i < activeLateUpdates.Count; i++)
            {
                activeLateUpdates[i](deltaTime);
            }
        }

        private static void DoFixedUpdateTicks(float deltaTime)
        {
            for (int i = 0; i < fixedUpdatesToBeRemoved.Count; i++)
            {
                activeFixedUpdates.Remove(fixedUpdatesToBeRemoved[i]);
            }

            fixedUpdatesToBeRemoved.Clear();

            for (int i = 0; i < fixedUpdatesToBeAdded.Count; i++)
            {
                activeFixedUpdates.Add(fixedUpdatesToBeAdded[i]);
            }

            fixedUpdatesToBeAdded.Clear();

            for (int i = 0; i < activeFixedUpdates.Count; i++)
            {
                activeFixedUpdates[i](deltaTime);
            }
        }
    }
}