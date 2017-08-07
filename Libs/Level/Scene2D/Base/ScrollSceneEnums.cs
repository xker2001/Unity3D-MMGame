namespace MMGame.Scene2D
{
    /// <summary>
    /// 场景卷轴方式。
    /// </summary>
    public enum ScrollAxis
    {
        Horizontal, // 水平卷轴
        Vertical // 垂直卷轴
    }

    /// <summary>
    /// 相对屏幕坐标系的方向。
    /// </summary>
    public enum RelativeDirection
    {
        Positive, // 通常右/下为正向
        Negative
    }
}