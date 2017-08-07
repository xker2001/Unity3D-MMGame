using UnityEngine;

namespace MMGame
{
public static class VectorLib
{
    #region Vector3

    /// <summary>
    /// 计算两个矢量之间的夹角。从上往下看，顺时针为正，逆时针为负（待确定）。
    /// </summary>
    /// <param name="vec1">矢量一，不能为 Vector3.up。</param>
    /// <param name="vec2">矢量二。</param>
    /// <remarks>
    /// 该方法有待改进，或添加更加详细的说明。
    /// </remarks>
    public static float Angle (Vector3 vec1, Vector3 vec2)
    {
        float angle = Vector3.Angle (vec1, vec2);
        Vector3 line1Right = Vector3.Cross (vec1, Vector3.up);
        float sign = Mathf.Sign (Vector3.Dot (vec2, line1Right));
        return sign * angle;
    }

    #endregion
}
}