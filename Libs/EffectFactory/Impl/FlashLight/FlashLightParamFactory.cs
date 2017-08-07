using UnityEngine;

namespace MMGame.EffectFactory.FlashLight
{
    /// <summary>
    /// 创建一个可以照亮地面的伪闪光。
    /// 包括一个动态光源和一个用于模拟照亮地面的模型特效。
    /// 使用模型模拟照亮效果的原因是地面的 shader 可能不接受光照。
    /// </summary>
    [System.Serializable]
    public class FlashLightParamFactory : PlayOneShotParamFactory
    {
        [SerializeField] private float fadeInTime = 0.05f;
        [SerializeField] private float fadeOutTime = 0.05f;
        [SerializeField] private float livingTime = 0f;
        [SerializeField] private float lightRange = 3;
        [SerializeField] private Color lightColor = Color.red;
        [SerializeField] private float lightIntensity = 1;
        [SerializeField] private bool enableGroundLight = true;
        [SerializeField] private float groundLightScale = 1;
        [SerializeField] [Range(0, 1)] private float groundLightAlpha = 0.2f;
        [SerializeField] private Transform lightPrefab;

        /// <summary>
        /// 动态光源的淡入时间。
        /// </summary>
        public float FadeInTime
        {
            get { return fadeInTime; }
        }

        /// <summary>
        /// 动态光源的淡出时间。
        /// </summary>
        public float FadeOutTime
        {
            get { return fadeOutTime; }
        }

        /// <summary>
        /// 动态光源在淡入后和淡出前之间的维持时间。
        /// </summary>
        public float LivingTime
        {
            get { return livingTime; }
        }

        /// <summary>
        /// 动态光源的光照范围。
        /// </summary>
        public float LightRange
        {
            get { return lightRange; }
        }

        /// <summary>
        /// 动态光源的颜色。
        /// </summary>
        public Color LightColor
        {
            get { return lightColor; }
        }

        /// <summary>
        /// 动态光源的强度（Intensity）。
        /// </summary>
        public float LightIntensity
        {
            get { return lightIntensity; }
        }

        /// <summary>
        /// 是否使用伪地面照亮效果。
        /// </summary>
        public bool EnableGroundLight
        {
            get { return enableGroundLight; }
        }

        /// <summary>
        /// 地面光照模型片的缩放值。
        /// 真实的缩放值 = 光照范围 * GroundLightScale
        /// </summary>
        public float GroundLightScale
        {
            get { return groundLightScale; }
        }

        /// <summary>
        /// 地面光照模型片的 alpha 值。
        /// </summary>
        public float GroundLightAlpha
        {
            get { return groundLightAlpha; }
        }

        /// <summary>
        /// 地面伪光照的模型片 prefab。
        /// </summary>
        public Transform LightPrefab
        {
            get { return lightPrefab; }
        }

        // ------------------------------------------------------

        public override bool IsNull()
        {
            return LightRange < Mathf.Epsilon || LightIntensity < Mathf.Epsilon;
        }

        protected override ParamObject Produce()
        {
            Transform objPrefab = Prefab.Load("FlashLightParamObject");
            Transform xform = PoolManager.Spawn(objPrefab.name, objPrefab);
            FlashLightParamObject obj = xform.GetComponent<FlashLightParamObject>();
            obj.SetParameters(this);
            return obj;
        }
    }
}