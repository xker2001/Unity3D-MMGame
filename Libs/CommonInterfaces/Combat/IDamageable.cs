namespace MMGame
{
    /// <summary>
    /// 可被伤害接口。
    /// 可以跟 IDamage 配合使用。
    /// </summary>
    public interface IDamageable
    {
        void GetDamage(IDamage damage);
    }
}