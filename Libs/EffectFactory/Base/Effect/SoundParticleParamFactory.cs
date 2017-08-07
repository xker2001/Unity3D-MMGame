using EasyEditor;
using UnityEngine;
using MMGame;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的带音效的粒子特效工厂。
    /// </summary>
    [System.Serializable]
    public class SoundParticleParamFactory : PlayOneShotParamFactory
    {
        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private ParticleParamFactory particleFactory;

        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private SoundParamFactory soundFactory;

        public ParticleParamFactory ParticleFactory
        {
            get { return particleFactory; }
        }

        public SoundParamFactory SoundFactory
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
            Transform objPrefab = Prefab.Load("SoundParticleParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            SoundParticleParamObject obj = xform.GetComponent<SoundParticleParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}