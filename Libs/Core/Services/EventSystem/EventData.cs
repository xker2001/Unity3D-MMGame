using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Event
{
    /// <summary>
    /// 事件数据包。
    /// </summary>
    abstract public class EventData
    {
        private readonly HashSet<GameObject> triggeredTargets = new HashSet<GameObject>();

        /// <summary>
        /// 事件的发送者。
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// 是否中断事件的广播。
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// 中断事件的广播。
        /// </summary>
        public void Stop()
        {
            IsStopped = true;
        }

        /// <summary>
        /// 重置当前事件实例以供下次使用。
        /// </summary>
        public void Reset()
        {
            Sender = null;
            IsStopped = false;
            triggeredTargets.Clear();
        }

        /// <summary>
        /// 记录已经执行触发的物体。
        /// </summary>
        /// <param name="obj"></param>
        internal void Trigger(GameObject obj)
        {
            triggeredTargets.Add(obj);
        }

        /// <summary>
        /// 指定物体是否已经被触发过。
        /// </summary>
        /// <param name="obj">指定的物体。</param>
        /// <returns>如果已经被触发过返回 true，反之返回 false。</returns>
        internal bool CheckTriggered(GameObject obj)
        {
            return triggeredTargets.Contains(obj);
        }
    }
}