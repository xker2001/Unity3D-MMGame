
#if NODE_CANVAS
using System;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.AI.BehaviourTree
{
    /// <summary>
    /// 行为树管理组件，负责调用、维护第三方行为树，同时维护事件缓存。
    /// </summary>
    [RequireComponent(typeof(EventCache))]
    public class NCBehaviourTreeManager : MonoBehaviour, IBehaviourTreeManager
    {
        [SerializeField] private bool startOnEnable = true;
        [SerializeField] private BehaviourTreeOwner btOwner;

        private EventCache eventCache; // 事件缓存组件

        void Awake()
        {
            eventCache = GetComponent<EventCache>();
            Assert.IsNotNull(eventCache);
        }

        void OnEnable()
        {
            if (startOnEnable)
            {
                StartBehaviour();
            }
        }

        void OnDisable()
        {
            StopBehaviour();
        }

        /// <summary>
        /// 开始运行行为树。
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void StartBehaviour()
        {
            if (!btOwner)
            {
                throw new NullReferenceException("NodeCanvas behaviour tree is not assigned.");
            }

            UpdateManager.Register(FastUpdate);
        }

        /// <summary>
        /// 停止运行行为树。
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void StopBehaviour()
        {
            if (!btOwner)
            {
                throw new NullReferenceException("NodeCanvas behaviour tree is not assigned.");
            }

            eventCache.Clear();
            UpdateManager.Unregister(FastUpdate);
        }

        /// <summary>
        /// 暂停运行行为树。
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void PauseBehaviour()
        {
            if (!btOwner)
            {
                throw new NullReferenceException("NodeCanvas behaviour tree is not assigned.");
            }

            btOwner.PauseBehaviour();
        }

        /// <summary>
        /// 单步执行行为树。
        /// </summary>
        /// <param name="deltaTime">帧时长。</param>
        private void FastUpdate(float deltaTime)
        {
            eventCache.PopHotEvents();
            btOwner.Tick();
        }
    }
}
#endif