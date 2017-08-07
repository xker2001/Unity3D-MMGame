using UnityEngine;
using MMGame;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的声音类型 ParamObject。
    /// </summary>
    public class SoundParamObject : PlayOneShotParamObject
    {
        private SoundParamFactory factory;
        private AudioSource audioSource;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(SoundParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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

            audioSource.spatialBlend = factory.SpatialBlend;
            audioSource.clip = factory.Sound;
            audioSource.pitch = factory.Pitch;
            audioSource.volume = factory.Volume;
            audioSource.loop = false;
            audioSource.Play();
            isPlaying = true;

            InvokeRepeating("CheckAlive", 0.001f, 0.5f);
        }

        public override void Destroy()
        {
            audioSource.Stop();
            DespawnSelf();
        }

        private void CheckAlive()
        {
            if (!audioSource.isPlaying)
            {
                DespawnSelf();
            }
        }

        private void DespawnSelf()
        {
            CancelInvoke();
            isPlaying = false;
            PoolManager.Despawn(xform);
        }
    }
}