using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的声音工厂。
    /// </summary>
    [System.Serializable]
    public class SoundLoopParamFactory : PlayLoopParamFactory
    {
        [SerializeField] private AudioClip sound;
        [SerializeField] [Range(0, 1)] private float volume = 0.6f;
        [SerializeField] [Range(0, 10)] private float pitch = 1;
        [SerializeField] [Range(0, 1)] private float spatialBlend = 1; // 3D on default

        public AudioClip Sound
        {
            get { return sound; }
        }

        public float Volume
        {
            get { return volume; }
        }

        public float SpatialBlend
        {
            get { return spatialBlend; }
            set
            {
                Assert.IsTrue(value >= 0 && value <= 1f);
                spatialBlend = value;
            }
        }

        public float Pitch
        {
            get { return pitch; }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return Sound == null || Volume < Mathf.Epsilon;
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("SoundLoopParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            SoundLoopParamObject obj = xform.GetComponent<SoundLoopParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}