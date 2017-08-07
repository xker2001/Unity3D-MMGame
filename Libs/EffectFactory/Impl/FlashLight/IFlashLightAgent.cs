using UnityEngine;

namespace MMGame.EffectFactory.FlashLight
{
    /// <summary>
    /// FlashLight 系统的代理接口，负责从实际项目中获取所需数据。
    /// </summary>
    public interface IFlashLightAgent
    {
        /// <summary>
        /// 获取地面照亮效果模型片放置的位置。
        /// </summary>
        /// <param name="position">FlashLight 物体的位置。</param>
        /// <returns></returns>
        Vector3 GetGroundLightPosition(Vector3 position);
    }
}