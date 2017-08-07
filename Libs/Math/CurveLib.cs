using UnityEngine;

namespace MMGame
{
public static class CurveLib
{
    #region 指数平滑

    /// <summary>
    /// 对一个浮点数进行指数平滑。
    /// </summary>
    /// <returns>经过指数平滑后的数值</returns>
    /// <param name="currentRealValue">当前实际值。</param>
    /// <param name="lastSmoothValue">上一个平滑值。</param>
    /// <param name="alpha">平滑因子，范围为 0 ~ 1，值越小平滑力度越大。</param>
    public static float ExpSmooth (float currentRealValue, float lastSmoothValue, float alpha)
    {
        return alpha * currentRealValue + (1 - alpha) * lastSmoothValue;
    }

    /// <summary>
    /// 对一个二维矢量进行指数平滑。
    /// </summary>
    /// <returns>经过指数平滑后的数值</returns>
    /// <param name="currentRealValue">当前实际值。</param>
    /// <param name="lastSmoothValue">上一个平滑值。</param>
    /// <param name="alpha">平滑因子，范围为 0 ~ 1，值越小平滑力度越大。</param>
    public static Vector2 ExpSmooth (Vector2 currentRealValue, Vector2 lastSmoothValue, float alpha)
    {
        return alpha * currentRealValue + (1 - alpha) * lastSmoothValue;
    }

    /// <summary>
    /// 对一个三维矢量进行指数平滑。
    /// </summary>
    /// <returns>经过指数平滑后的数值</returns>
    /// <param name="currentRealValue">当前实际值。</param>
    /// <param name="lastSmoothValue">上一个平滑值。</param>
    /// <param name="alpha">平滑因子，范围为 0 ~ 1，值越小平滑力度越大。</param>
    public static Vector3 ExpSmooth (Vector3 currentRealValue, Vector3 lastSmoothValue, float alpha)
    {
        return alpha * currentRealValue + (1 - alpha) * lastSmoothValue;
    }

    #endregion

    #region 贝塞尔曲线

    /// <summary>
    /// 获取三次贝塞尔曲线（cubic bezier）上的点。
    /// </summary>
    /// <returns>三次贝塞尔曲线点的三维坐标。</returns>
    /// <param name="p0">起点。</param>
    /// <param name="p1">起点端控制点。</param>
    /// <param name="p2">终点端控制点</param>
    /// <param name="p3">终点。</param>
    /// <param name="f">插值因子，范围是 0 ~ 1，越大越靠近终点</param>
    public static Vector3 GetCubicBezierPoint (
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float f)
    {
        float f1 = 1 - f;
        return f1 * f1 * f1 * p0
               + 3 * f * f1 * f1 * p1
               + 3 * f * f * f1 * p2
               + f * f * f * p3;
    }

    /// <summary>
    /// 创建一条三次贝塞尔曲线。
    /// </summary>
    /// <returns>一组表示贝塞尔曲线的点。</returns>
    /// <param name="p0">起点。</param>
    /// <param name="p1">起点端控制点。</param>
    /// <param name="p2">终点。</param>
    /// <param name="p3">终点端控制点。</param>
    /// <param name="pointNumber">曲线上点的数量（包括起点和终点）。</param>
    public static Vector3[] MakeBezierPoints (Vector3 p0, Vector3 p1,
            Vector3 p2, Vector3 p3, int pointNumber)
    {
        Vector3[] result = new Vector3[pointNumber];
        result[0] = p0;
        result[pointNumber - 1] = p3;
        float f = 0;
        float step = 1f / (pointNumber - 1);

        for (int i = 1; i < pointNumber - 1; i++)
        {
            f += step;
            result[i] = GetCubicBezierPoint (p0, p1, p2, p3, f);
        }

        return result;
    }

    /// <summary>
    /// 创建一条三次贝塞尔曲线。
    /// </summary>
    /// <returns>一组表示贝塞尔曲线的点。</returns>
    /// <param name="p0">起点。</param>
    /// <param name="p1">起点端控制点。</param>
    /// <param name="p2">终点。</param>
    /// <param name="p3">终点端控制点。</param>
    /// <param name="points">存放曲线点的数组。数组的长度决定点的数量。</param>
    public static void MakeBezierPoints (Vector3 p0, Vector3 p1,
                                         Vector3 p2, Vector3 p3, ref Vector3[] points)
    {
        int len = points.Length;
        points[0] = p0;
        points[len - 1] = p3;
        float f = 0;
        float step = 1f / (len - 1);

        for (int i = 1; i < len - 1; i++)
        {
            f += step;
            points[i] = GetCubicBezierPoint (p0, p1, p2, p3, f);
        }
    }

    #endregion
}
}
