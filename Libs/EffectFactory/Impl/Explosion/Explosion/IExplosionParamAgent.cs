using UnityEngine;

namespace MMGame.EffectFactory.Explosion
{
    /// <summary>
    /// Explosion 系统的代理接口，负责从实际项目中获取所需数据。
    /// </summary>
    public interface IExplosionParamAgent
    {
        /// <summary>
        /// 获取地面爆炸痕迹特效放置的位置。
        /// </summary>
        /// <param name="position">炸弹物体的位置。</param>
        /// <returns>特效放置的位置。</returns>
        Vector3 GetGroundEffectPosition(Vector3 position);

        /// <summary>
        /// 获取可以伤害的 layers。
        /// </summary>
        /// <param name="bomb">炸弹物体。</param>
        /// <returns>炸弹可以伤害的 layers。</returns>
        LayerMask GetHurtableLayers(ExplosionParamObject bomb);
    }
}