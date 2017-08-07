namespace MMGame
{
    /// <summary>
    /// 判断阵营对阵营是否可以形成伤害。
    /// </summary>
    public interface ICampDamage
    {
        /// <summary>
        /// 是否可以伤害指定阵营。
        /// </summary>
        /// <param name="perpetrator">伤害施加者的阵营。</param>
        /// <param name="victim">受害者的阵营。</param>
        /// <returns>如果可以施加伤害，返回 true，反之返回 false。</returns>
        bool CanDamage(ICamp perpetrator, ICamp victim);
    }
}