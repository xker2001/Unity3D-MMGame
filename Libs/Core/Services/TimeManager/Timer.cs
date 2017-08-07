using System;
using UnityEngine;

namespace MMGame
{
    public class Timer : IComparable<Timer>
    {
        /// <summary>
        /// 剩余时间。
        /// </summary>
        public float LeftTime
        {
            get { return TimerManager.GetLeftTime(this); }
        }

        /// <summary>
        /// 创建一个新的计时器。
        /// </summary>
        /// <param name="time">计时时长。</param>
        /// <param name="callback">回调函数。</param>
        /// <returns>计时器实例。</returns>
        public static Timer Create(float time, Action callback)
        {
            return TimerManager.Create(time, callback);
        }

        /// <summary>
        /// 销毁自身。
        /// </summary>
        public void Destroy()
        {
            TimerManager.Destroy(this);
        }

        internal void Invoke()
        {
            if (Callback != null)
            {
                Callback();
            }
        }

        /// <summary>
        /// 计时到达时间。
        /// </summary>
        internal float GoalTime { get; set; }

        /// <summary>
        /// 回调函数。
        /// </summary>
        internal Action Callback { get; set; }

        /// <summary>
        /// 计时器是否处于激活状态。
        /// </summary>
        internal bool IsActive { get; set; }

        /// <summary>
        /// 对 Timer 的目标时间进行反向的比较。
        /// </summary>
        /// <param name="other">被比较的 Timer。</param>
        /// <returns>如果大于被比较的 Timer 返回 -1，反之返回 1，如果相等返回 0。</returns>
        public int CompareTo(Timer other)
        {
            return GoalTime < other.GoalTime ? 1 : GoalTime > other.GoalTime ? -1 : 0;
        }
    }
}