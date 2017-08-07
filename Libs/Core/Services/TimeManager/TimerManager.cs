using System;
using System.Collections.Generic;
using UnityEngine;
using MMGame;

namespace MMGame
{
    public class TimerManager : MonoBehaviour
    {
        /// <summary>
        /// 计时器对象池。
        /// </summary>
        internal class TimerPool
        {
            private static Queue<Timer> pool = new Queue<Timer>();

            internal static Timer Pop()
            {
                if (pool.Count > 0)
                {
                    return pool.Dequeue();
                }
                else
                {
                    return new Timer();
                }
            }

            internal static void Push(Timer timer)
            {
                pool.Enqueue(timer);
            }

            /// <summary>
            /// 计时器数量，测试用。
            /// </summary>
            internal static int Count
            {
                get { return pool.Count; }
            }
        }

        /// <summary>
        /// 计时管理器当前时间。
        /// </summary>
        private static float currentTime;
        private static bool isPaused;
        private static Queue<Timer> timersToBeCreated = new Queue<Timer>();
        private static Queue<Timer> timersToBeDestroyed = new Queue<Timer>();

        // Unity 5.4 不支持 SortedSet
        private static List<Timer> timers = new List<Timer>();

        /// <summary>
        /// 计时器数量，测试用。
        /// </summary>
        internal static int PoolCount
        {
            get { return TimerPool.Count; }
        }

        /// <summary>
        /// 创建一个新的计时器，优先从对象池创建。
        /// </summary>
        /// <param name="time">计时时长。</param>
        /// <param name="callback">回调函数。</param>
        /// <returns>计时器实例。</returns>
        internal static Timer Create(float time, Action callback)
        {
            Timer timer = TimerPool.Pop();
            timer.GoalTime = currentTime + time;
            timer.Callback = callback;
            timer.IsActive = true;
            timersToBeCreated.Enqueue(timer);
            return timer;
        }

        /// <summary>
        /// 销毁一个计时器。
        /// </summary>
        /// <param name="timer">计时器实例。</param>
        internal static void Destroy(Timer timer)
        {
            if (!timer.IsActive)
            {
                return;
            }

            ReleaseTimer(timer);
        }

        /// <summary>
        /// 暂停所有计时器。
        /// </summary>
        public static void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// 恢复所有计时器的计时。
        /// </summary>
        public static void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// 销毁所有计时器。
        /// </summary>
        public static void Clear()
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                ReleaseTimer(i);
            }
        }

        /// <summary>
        /// 获取指定计时器的剩余时长。
        /// </summary>
        /// <param name="timer">计时器。</param>
        /// <returns>时长。</returns>
        internal static float GetLeftTime(Timer timer)
        {
            return timer.GoalTime - currentTime;
        }

        void Update()
        {
            if (isPaused)
            {
                return;
            }

            while (timersToBeCreated.Count > 0)
            {
                timers.AddSorted(timersToBeCreated.Dequeue());
            }

            while (timersToBeDestroyed.Count > 0)
            {
                Timer timer = timersToBeDestroyed.Dequeue();
                TimerPool.Push(timer);
                timers.Remove(timer);
            }

            currentTime += Time.deltaTime;

            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if (timers[i].GoalTime > currentTime)
                {
                    break;
                }

                timers[i].Invoke();
                ReleaseTimer(i);
            }
        }

        /// <summary>
        /// 回收一个指定索引的计时器到对象池。
        /// </summary>
        /// <param name="i">计时器的索引</param>
        private static void ReleaseTimer(int i)
        {
            ResetTimer(timers[i]);
            TimerPool.Push(timers[i]);
            timers.RemoveAt(i);
        }

        /// <summary>
        /// 回收一个计时器到对象池。
        /// </summary>
        /// <param name="timer">计时器实例。</param>
        private static void ReleaseTimer(Timer timer)
        {
            ResetTimer(timer);
            timersToBeDestroyed.Enqueue(timer);
        }

        /// <summary>
        /// 重置一个计时器（以回收）。
        /// </summary>
        /// <param name="timer">计时器实例。</param>
        private static void ResetTimer(Timer timer)
        {
            timer.GoalTime = 0;
            timer.Callback = null;
            timer.IsActive = false;
        }
    }
}