using UnityEngine;
using MMGame;
using System.Collections;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的粒子类型 ParamObject。
    /// </summary>
    public class ParticleParamObject : PlayOneShotParamObject
    {
        private ParticleParamFactory factory;
        private ParticleSystem ps;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(ParticleParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        private void Awake()
        {
            xform = transform;
        }

        public override void PlayAndDestroy()
        {
            if (factory.IsNull())
            {
                DespawnSelf();
                return;
            }

            if (isPlaying)
            {
                return;
            }

            ps = PoolManager.Spawn(factory.Particle.name, factory.Particle, xform.position, xform.rotation);
            Transform psXform = ps.transform;

            if (factory.AlignSprite)
            {
                var main = ps.main;
                main.startRotation = xform.eulerAngles.y * Mathf.Deg2Rad;
            }

            foreach (ParticleSystem subPs in ps.GetComponentsInChildren<ParticleSystem>())
            {
                var main = subPs.main;
                main.loop = false;

                if (factory.Scale)
                {
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }
            }

            if (factory.Scale)
            {
                psXform.localScale *= factory.Multiple;
            }

            psXform.parent = xform;
            ps.Play();
            isPlaying = true;

            PoolManager.SetAutoDespawn(ps, DespawnSelf);

            // 粒子应该自检并自动回收，这里防止自检失败，时间一到强制回收。
            if (factory.MaxLifeTime > Mathf.Epsilon)
            {
                Invoke("Destroy", factory.MaxLifeTime);
            }
        }

        public override void Destroy()
        {
            if (ps)
            {
                PoolManager.Despawn(ps); // 会立即调用 DespawnSelf()
            }
            else
            {
                DespawnSelf();
            }
        }

        private void DespawnSelf()
        {
            CancelInvoke();
            ps = null;
            isPlaying = false;
            PoolManager.Despawn(xform);
        }
    }
}