using EasyEditor;
using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 循环播放的粒子特效工厂。
    /// </summary>
    [System.Serializable]
    public class ParticleLoopParamFactory : PlayLoopParamFactory
    {
        [SerializeField]
        private ParticleSystem particle;
        [SerializeField]
        private bool alignSprite;

        [SerializeField]
        private bool scale;

        [Visibility("scale", true)]
        [Inspector(label = "Multiple")]
        [SerializeField]
        private float multiple = 1f;

        /// <summary>
        /// 粒子 prefab。
        /// </summary>
        public ParticleSystem Particle
        {
            get { return particle; }
        }

        /// <summary>
        /// 是否根据粒子发射器的方向调整 sprite 方向。
        /// 用于溅血等场合需要多种 sprite 方向的场合。
        /// </summary>
        public bool AlignSprite
        {
            get { return alignSprite; }
        }

        /// <summary>
        /// 是否缩放粒子。
        /// 如果选择缩放粒子，粒子（包括其子级）的 ScalingMode 会被设置为 Heirarchy 模式。
        /// </summary>
        public bool Scale
        {
            get { return scale; }
        }

        /// <summary>
        /// 粒子的 localScale 缩放值。
        /// </summary>
        public float Multiple
        {
            get { return multiple; }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return Particle == null;
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("ParticleLoopParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            ParticleLoopParamObject obj = xform.GetComponent<ParticleLoopParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}