using System;
using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    /// <summary>
    /// 用于取代 MonoBehaviour 的 Coroutine。
    /// NOTE！尚未仔细推敲，也未在实践中进行足够应用，可能需要改进设计。
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UMCoroutineManager : MonoBehaviour
    {
        private static HashSet<MonoBehaviour> coroutinesToBeRemoved = new HashSet<MonoBehaviour>();
        private static HashSet<UMCoroutine> coroutinesToBeAdded = new HashSet<UMCoroutine>();

        private static Dictionary<MonoBehaviour, HashSet<UMCoroutine>> activeCoroutines =
            new Dictionary<MonoBehaviour, HashSet<UMCoroutine>>();

        void Awake()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnEnable()
        {
            if (FindObjectsOfType<UMCoroutineManager>().Length > 1)
            {
                throw new ApplicationException("UMCoroutineManager was created more than one times.");
            }

            UpdateManager.RegisterUpdate(FastUpdate);
        }

        void OnDisable()
        {
            UpdateManager.UnregisterUpdate(FastUpdate);
        }

        /// <summary>
        /// 添加一个 Coroutine 到执行列表。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="coroutine">待添加的 UMCoroutine 的实例。</param>
        public static void AddCoroutine(UMCoroutine coroutine)
        {
            coroutinesToBeAdded.Add(coroutine);
        }

        /// <summary>
        /// 停止一个 Coroutine。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="coroutine">待停止的 Coroutine 实例。</param>
        public static void StopCoroutine(UMCoroutine coroutine)
        {
            coroutine.Stop();
        }

        /// <summary>
        /// 停止指定 MonoBehaviour 所有活动的 Coroutine 实例。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="owner">MonoBehaviour 实例。</param>
        public static void StopAllCoroutines(MonoBehaviour owner)
        {
            HashSet<UMCoroutine> coroutines;

            if (activeCoroutines.TryGetValue(owner, out coroutines))
            {
                foreach (UMCoroutine c in coroutines)
                {
                    c.Stop();
                }
            }
        }

        /// <summary>
        /// 暂停一个 Coroutine。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="coroutine">待暂停的 Coroutine 实例。</param>
        public static void PauseCoroutine(UMCoroutine coroutine)
        {
            coroutine.Pause();
        }

        /// <summary>
        /// 暂停指定 MonoBehaviour 所有活动的 Coroutine 实例。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="owner">MonoBehaviour 实例。</param>
        public static void PauseAllCoroutines(MonoBehaviour owner)
        {
            HashSet<UMCoroutine> coroutines;

            if (activeCoroutines.TryGetValue(owner, out coroutines))
            {
                foreach (UMCoroutine c in coroutines)
                {
                    c.Pause();
                }
            }
        }

        /// <summary>
        /// 恢复一个 Coroutine。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="coroutine">待恢复的 Coroutine 实例。</param>
        public static void ResumeCoroutine(UMCoroutine coroutine)
        {
            coroutine.Resume();
        }

        /// <summary>
        /// 恢复指定 MonoBehaviour 所有活动的 Coroutine 实例。该方法仅供扩展 MonoBehaviour 时使用。
        /// </summary>
        /// <param name="owner">MonoBehaviour 实例。</param>
        public static void ResumeAllCoroutines(MonoBehaviour owner)
        {
            HashSet<UMCoroutine> coroutines;

            if (activeCoroutines.TryGetValue(owner, out coroutines))
            {
                foreach (UMCoroutine c in coroutines)
                {
                    c.Resume();
                }
            }
        }

        /// <summary>
        /// 对 Coroutine 执行一次更新。
        /// </summary>
        /// <param name="coroutine">待更新的 Coroutine 实例。</param>
        private static void UpdateCoroutine(UMCoroutine coroutine)
        {
            if (coroutine.IsWaiting)
            {
                coroutine.Wait();
            }
            else
            {
                coroutine.MoveNext();
            }
        }

        private static void FastUpdate(float deltaTime)
        {
            foreach (UMCoroutine c in coroutinesToBeAdded)
            {
                MonoBehaviour owner = c.Owner;
                HashSet<UMCoroutine> coroutines;

                if (!activeCoroutines.TryGetValue(owner, out coroutines))
                {
                    activeCoroutines.Add(owner, new HashSet<UMCoroutine>());
                }

                activeCoroutines[owner].Add(c);
            }

            coroutinesToBeAdded.Clear();

            foreach (KeyValuePair<MonoBehaviour, HashSet<UMCoroutine>> kv in activeCoroutines)
            {
                kv.Value.RemoveWhere(c => c.IsStopped);

                if (kv.Value.Count == 0)
                {
                    coroutinesToBeRemoved.Add(kv.Key);
                }
                else
                {
                    foreach (UMCoroutine c in kv.Value)
                    {
                        if (!c.IsStopped)
                        {
                            UpdateCoroutine(c);
                        }
                    }
                }
            }

            foreach (MonoBehaviour owner in coroutinesToBeRemoved)
            {
                activeCoroutines.Remove(owner);
            }

            coroutinesToBeRemoved.Clear();
        }
    }
}