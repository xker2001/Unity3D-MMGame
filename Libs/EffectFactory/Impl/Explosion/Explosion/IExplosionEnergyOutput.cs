using System;
using UnityEngine;

namespace MMGame.EffectFactory.Explosion
{
    /// <summary>
    /// 爆炸能量输出接口，处理伤害、摄相机震动等非声光特效部分的爆炸效果。
    /// 该接口的实现挂接到对应的 Factory 物体，并指定到 Factory 相应的字段。
    /// </summary>
    public interface IExplosionEnergyOutput
    {
        /// <summary>
        /// 输出爆炸能量。
        /// </summary>
        /// <param name="sender">爆炸的输出者，可以带有阵营等信息。</param>
        /// <param name="bomb">爆炸的 ParamObject。</param>
        /// <param name="position">爆炸发生的位置。</param>
        /// <param name="onComplete">爆炸输出回调。当爆炸输出不是瞬间完成时通知 ParamObject 输出完成。</param>
        void Send(GameObject sender, ExplosionParamObject bomb, Vector3 position, Action onComplete);
    }
}