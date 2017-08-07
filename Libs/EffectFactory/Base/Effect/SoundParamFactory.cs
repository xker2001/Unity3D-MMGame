using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的声音工厂。
    /// </summary>
    [System.Serializable]
    public class SoundParamFactory : PlayOneShotParamFactory
    {
        [SerializeField] private AudioClip sound;
        [SerializeField] [Range(0, 1)] private float volume = 0.6f;
        [SerializeField] [Range(0, 10)] private float pitch = 1;
        [SerializeField] [Range(0, 1)] private float spatialBlend = 1; // 3D on default

        /// <summary>
        /// 声音资源。
        /// </summary>
        public AudioClip Sound
        {
            get { return sound; }
        }

        /// <summary>
        /// 音量。
        /// </summary>
        public float Volume
        {
            get { return Mathf.Clamp01(volume); }
        }

        /// <summary>
        /// 音调。
        /// </summary>
        public float Pitch
        {
            get { return pitch; }
        }

        /// <summary>
        /// 2D 音效或 3D 音效。
        /// 值为 0 时为 2D 音效，值为 1 是为 3D 音效。
        /// </summary>
        public float SpatialBlend
        {
            get { return spatialBlend; }
            set
            {
                Assert.IsTrue(value >= 0 && value <= 1f);
                spatialBlend = value;
            }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return Sound == null || Volume < Mathf.Epsilon;
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("SoundParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            SoundParamObject obj = xform.GetComponent<SoundParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}