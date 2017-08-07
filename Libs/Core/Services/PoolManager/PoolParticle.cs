using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    [DisallowMultipleComponent]
    public class PoolParticle : PoolEntity
    {
        private ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void Disable()
        {
            CancelInvoke();
        }

        /// <summary>
        /// 当粒子死亡后自动回收自身，由 PoolManager.SetAutoDespawn() 手动启动。
        /// </summary>
        internal void CheckAndDespawnSelf()
        {
            if (!IsInvoking("CheckAlive"))
            {
                InvokeRepeating("CheckAlive", 0.001f, 0.5f);
            }
        }

        /// <summary>
        /// 由 PoolManager.Despawn() 调用。
        /// </summary>
        internal override void OnDespawn()
        {
            base.OnDespawn();

            if (ps.isPlaying)
            {
                ps.Stop();
                ps.Clear();
            }

            CancelInvoke();

            // 之后才被 PoolManager disable
        }

        /// <summary>
        /// 检查粒子存活情况，如果已经死亡则回收自身。
        /// </summary>
        private void CheckAlive()
        {
            if (!ps.IsAlive())
            {
                PoolManager.Despawn(ps);
            }
        }
    }
}