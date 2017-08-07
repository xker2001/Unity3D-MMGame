using UnityEngine;

namespace MMGame
{
public static class MathLib
{
    #region 数值转换到区间

    /// <summary>
    /// 将浮点数循环至指定区间内。
    /// </summary>
    /// <param name="value">待循环的浮点数。</param>
    /// <param name="minValue">区间最小值。</param>
    /// <param name="maxValue">区间最大值。</param>
    /// <returns>循环在指定区间内的值</returns>
    /// <remarks>
    /// 循环值不会等于区间最大值。
    /// </remarks>
    public static float Cycle (float value, float minValue, float maxValue)
    {
        float delta = maxValue - minValue;

        if (value < minValue)
        {
            float remainder = (value - maxValue) % delta; // remainder is negative here
            return remainder == 0 ? minValue : maxValue + remainder;
        }
        else if (value >= maxValue)
        {
            return minValue + (value - minValue) % delta;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// 将整型数循环至指定的区间内。
    /// </summary>
    /// <param name="value">待变换的整数。</param>
    /// <param name="minValue">区间最小值。</param>
    /// <param name="maxValue">区间最大值。</param>
    /// <returns>循环在指定区间内的值</returns>
    /// <remarks>
    /// 循环值不会等于区间最大值。
    /// </remarks>
    public static int Cycle (int value, int minValue, int maxValue)
    {
        int delta = maxValue - minValue;

        if (value < minValue)
        {
            int remainder = (value - maxValue) % delta; // remainder is negative here
            return remainder == 0 ? minValue : maxValue + remainder;
        }

        if (value >= maxValue)
        {
            return minValue + (value - minValue) % delta;
        }

        return value;
    }

    #endregion

    #region 数值比较

    /// <summary>
    /// 比较两个浮点是否（近似）相等。
    /// </summary>
    /// <param name="a">待比较的浮点数。</param>
    /// <param name="b">待比较的浮点数。</param>
    /// <returns>如果相等返回 true，反之返回 false。</returns>
    public static bool AreEqual (float a, float b)
    {
        return Mathf.Approximately (a, b);
    }

    /// <summary>
    /// 比较两个浮点是否不（近似）相等。
    /// </summary>
    /// <param name="a">待比较的浮点数。</param>
    /// <param name="b">待比较的浮点数。</param>
    /// <returns>如果不相等返回 true，反之返回 false。</returns>
    public static bool AreNotEqual (float a, float b)
    {
        return !Mathf.Approximately (a, b);
    }

    #endregion
}
}