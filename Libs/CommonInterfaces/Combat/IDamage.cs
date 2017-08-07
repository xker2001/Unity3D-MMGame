using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// 伤害输入包。
    /// </summary>
    public interface IDamage
    {
        /// <summary>
        /// 伤害的施加者。
        /// 需要时携带阵营等附加信息。
        /// </summary>
        GameObject Sender { get; set; }

        /// <summary>
        /// 施加伤害。
        /// 如果有阵营等附加信息，在这里进行判定。
        /// </summary>
        /// <param name="target">被伤害的对象。</param>
        /// <param name="direction">伤害力的方向。</param>
        void Apply(GameObject target, Vector3 direction);

        /// <summary>
        /// 重置伤害参数。
        /// </summary>
        void OnReset();
    }
}