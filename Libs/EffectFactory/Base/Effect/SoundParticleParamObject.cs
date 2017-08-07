using UnityEngine;
using MMGame;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的带音效的粒子特效 ParamObject。
    /// </summary>
    public class SoundParticleParamObject : PlayOneShotParamObject
    {
        // private
        private SoundParticleParamFactory factory;
        private PlayOneShotParamObject psObj;
        private PlayOneShotParamObject sndObj;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(SoundParticleParamFactory factory)
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

            if (!factory.ParticleFactory.IsNull())
            {
                psObj = factory.ParticleFactory.Create(xform);
                PoolManager.SetOnDespawnedCallback(psObj.transform, OnParticleObjectDespawned);
                psObj.PlayAndDestroy();
            }

            if (!factory.SoundFactory.IsNull())
            {
                sndObj = factory.SoundFactory.Create(xform);
                PoolManager.SetOnDespawnedCallback(sndObj.transform, OnSoundObjectDespawned);
                sndObj.PlayAndDestroy();
            }

            isPlaying = true;
        }

        public override void Destroy()
        {
            if (psObj)
            {
                psObj.Destroy();
            }

            if (sndObj)
            {
                sndObj.Destroy();
            }

            isPlaying = false;
        }

        private void DespawnSelf()
        {
            isPlaying = false;
            PoolManager.Despawn(xform);
        }

        private void OnParticleObjectDespawned()
        {
            psObj = null;

            if (sndObj == null)
            {
                DespawnSelf();
            }
        }

        private void OnSoundObjectDespawned()
        {
            sndObj = null;

            if (psObj == null)
            {
                DespawnSelf();
            }
        }
    }
}