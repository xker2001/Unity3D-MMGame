using EasyEditor;
using UnityEngine;
using MMGame;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的带音效的粒子特效工厂。
    /// </summary>
    [System.Serializable]
    public class SoundParticleLoopParamFactory : PlayLoopParamFactory
    {
        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private ParticleLoopParamFactory particleFactory;

        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private SoundLoopParamFactory soundFactory;

        public ParticleLoopParamFactory ParticleFactory
        {
            get { return particleFactory; }
        }

        public SoundLoopParamFactory SoundFactory
        {
            get { return soundFactory; }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return ParticleFactory.IsNull() && SoundFactory.IsNull();
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("SoundParticleLoopParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            SoundParticleLoopParamObject obj = xform.GetComponent<SoundParticleLoopParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}