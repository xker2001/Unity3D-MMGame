namespace MMGame
{
    /// <summary>
    /// 角色所属阵营。
    /// 本接口将在开发实践中迭代改进，如增加多阵营支持等。
    /// </summary>
    public interface ICamp
    {
        /// <summary>
        /// 获取阵营（枚举）
        /// </summary>
        /// <returns></returns>
        int GetCamp();

        /// <summary>
        /// 设置阵营。
        /// </summary>
        /// <param name="camp"></param>
        void SetCamp(int camp);
    }
}