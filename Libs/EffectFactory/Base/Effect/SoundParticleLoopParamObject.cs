using UnityEngine;
using MMGame;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的带音效的粒子特效 ParamObject。
    /// </summary>
    public class SoundParticleLoopParamObject : PlayLoopParamObject
    {
        private SoundParticleLoopParamFactory factory;
        private PlayLoopParamObject psObj;
        private PlayLoopParamObject sndObj;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(SoundParticleLoopParamFactory factory)
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

            if (!factory.ParticleFactory.IsNull())
            {
                if (!psObj)
                {
                    psObj = factory.ParticleFactory.Create(xform);
                    PoolManager.SetOnDespawnedCallback(psObj.transform, OnParticleObjectDespawned);
                }

                psObj.Loop();
            }

            if (!factory.SoundFactory.IsNull())
            {
                if (!sndObj)
                {
                    sndObj = factory.SoundFactory.Create(xform);
                    PoolManager.SetOnDespawnedCallback(sndObj.transform, OnSoundObjectDespawned);
                }

                sndObj.Loop();
            }

            isPlaying = true;
        }

        public override void Stop()
        {
            if (psObj)
            {
                psObj.Stop();
            }

            if (sndObj)
            {
                sndObj.Stop();
            }

            isPlaying = false;
        }

        public override void SmoothStop()
        {
            if (psObj)
            {
                psObj.SmoothStop();
            }

            if (sndObj)
            {
                sndObj.SmoothStop();
            }

            isPlaying = false;
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

        public override void SmoothDestroy()
        {
            if (psObj)
            {
                psObj.SmoothDestroy();
            }

            if (sndObj)
            {
                sndObj.SmoothDestroy();
            }
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