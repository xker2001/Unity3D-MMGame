using UnityEngine;
using MMGame;
using System.Collections;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的粒子类型 ParamObject。
    /// </summary>
    public class ParticleLoopParamObject : PlayLoopParamObject
    {
        private ParticleLoopParamFactory factory;
        private ParticleSystem ps;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(ParticleLoopParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        private void Awake()
        {
            xform = transform;
        }

        public override void Loop()
        {
            if (isPlaying || factory.IsNull())
            {
                return;
            }

            if (!ps)
            {
                ps = PoolManager.Spawn(factory.Particle.name, factory.Particle, xform.position, xform.rotation);

                if (factory.AlignSprite)
                {
                    var main = ps.main;
                    main.startRotation = xform.eulerAngles.y * Mathf.Deg2Rad;
                }

                ps.transform.parent = xform;
            }

            foreach (ParticleSystem subPs in ps.GetComponentsInChildren<ParticleSystem>())
            {
                var main = subPs.main;
                main.loop = true;

                if (factory.Scale)
                {
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }
            }

            if (factory.Scale)
            {
                ps.transform.localScale *= factory.Multiple;
            }

            ps.Play();
            isPlaying = true;
        }

        public override void Stop()
        {
            if (ps)
            {
                ps.Stop();
                ps.Clear();
                // TODO 这里是否会回收掉 ps，是否需要重置缩放
            }

            isPlaying = false;
        }

        public override void SmoothStop()
        {
            if (ps)
            {
                ps.Stop();
                // TODO 这里是否会回收掉 ps，是否需要重置缩放
            }

            isPlaying = false;
        }

        public override void Destroy()
        {
            if (ps)
            {
                PoolManager.Despawn(ps); // PoolParticle 会停止并清空粒子
            }

            DespawnSelf();
        }

        public override void SmoothDestroy()
        {
            if (ps)
            {
                SmoothStop();
                PoolManager.SetAutoDespawn(ps, DespawnSelf);
            }
            else
            {
                DespawnSelf();
            }
        }

        private void DespawnSelf()
        {
            ps = null;
            isPlaying = false;
            PoolManager.Despawn(xform);
        }
    }
}