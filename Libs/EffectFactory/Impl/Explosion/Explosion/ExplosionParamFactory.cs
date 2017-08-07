using UnityEngine;
using EasyEditor;

namespace MMGame.EffectFactory.Explosion
{
    /// <summary>
    /// 爆炸专用特效工厂。
    /// 爆炸的输出由对应的 ExplosionParamObject 上挂接的 IDamage 和 ICameraShake 组件完成。
    /// </summary>
    [System.Serializable]
    public class ExplosionParamFactory : PlayOneShotParamFactory
    {
        [SerializeField] private SoundParticleParamFactory explodeEffect;
        [SerializeField] private ParticleParamFactory groundEffect;
        [SerializeField] private float maxHeightToShowGroundEffect = 2;

        [Message(text = "IExplosionEnergyOutput only!", method = "IsNotIExplosionEnergyOutput")]
        [SerializeField] private MonoBehaviour energyOutput;

        /// <summary>
        /// 检查 energyOutput 字段值是否实现了 IExplosionEnergyOutput 接口
        /// </summary>
        /// <returns>如果实现了接口返回 true，反之返回 false。</returns>
        private bool IsNotIExplosionEnergyOutput()
        {
            return energyOutput != null && energyOutput.GetComponent<IExplosionEnergyOutput>() == null;
        }

        /// <summary>
        /// 爆炸特效工厂。
        /// </summary>
        public SoundParticleParamFactory ExplodeEffect
        {
            get { return explodeEffect; }
        }

        /// <summary>
        /// 爆炸地面痕迹特效工厂。
        /// </summary>
        public ParticleParamFactory GroundEffect
        {
            get { return groundEffect; }
        }

        /// <summary>
        /// 可以显示地面痕迹的最高爆炸高度。
        /// 爆炸点高度超过该高度时不显示地面痕迹。
        /// </summary>
        public float MaxHeightToShowGroundEffect
        {
            get { return maxHeightToShowGroundEffect; }
        }

        /// <summary>
        /// 爆炸能量输出接口组件。
        /// </summary>
        public IExplosionEnergyOutput EnergyOutput
        {
            get { return energyOutput.GetComponent<IExplosionEnergyOutput>(); }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return EnergyOutput == null && ExplodeEffect.IsNull();
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("ExplosionParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            ExplosionParamObject obj = xform.GetComponent<ExplosionParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}