using EasyEditor;
using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 非循环播放的粒子特效工厂。
    /// </summary>
    [System.Serializable]
    public class ParticleParamFactory : PlayOneShotParamFactory
    {
        [SerializeField]
        private ParticleSystem particle;

        [SerializeField]
        private bool alignSprite;

        [Tooltip("0 为没有限制。")]
        [SerializeField]
        private float maxLifeTime = 0.2f;

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
        /// 粒子可以存在的最大时间，为 0 时无限制。
        /// 用于强制回收粒子，应对可能的粒子自我死亡检测失败的情况。
        /// </summary>
        public float MaxLifeTime
        {
            get { return maxLifeTime; }
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
            Transform objPrefab = Prefab.Load("ParticleParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            ParticleParamObject obj = xform.GetComponent<ParticleParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}