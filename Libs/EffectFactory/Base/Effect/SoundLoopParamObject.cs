using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的声音类型 ParamObject。
    /// </summary>
    public class SoundLoopParamObject : PlayLoopParamObject
    {
        private SoundLoopParamFactory factory;
        private AudioSource audioSource;
        private bool isPlaying;
        private Transform xform;

        public void SetParameters(SoundLoopParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            xform = transform;
        }

        public override void Loop()
        {
            if (isPlaying || factory.IsNull())
            {
                return;
            }

            audioSource.spatialBlend = factory.SpatialBlend;
            audioSource.clip = factory.Sound;
            audioSource.pitch = factory.Pitch;
            audioSource.volume = factory.Volume;
            audioSource.loop = true;
            audioSource.Play();
            isPlaying = true;
        }

        public override void Stop()
        {
            audioSource.Stop();
            isPlaying = false;
        }

        public override void SmoothStop()
        {
            Stop();
        }

        public override void Destroy()
        {
            Stop();
            PoolManager.Despawn(xform);
        }

        public override void SmoothDestroy()
        {
            Destroy();
        }
    }
}