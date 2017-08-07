namespace MMGame.Level
{
    /// <summary>
    /// 简化 LevelTransition 加载/切换关卡方法的接口。
    /// </summary>
    public interface ILevelLoader
    {
        void LoadLevel(ALevelMap map, LoadMode mode);
        void LoadLevelAsync(ALevelMap map, LoadMode mode);
        void SwitchToLevel(ALevelMap map);
        void SwitchToLevelAsync(ALevelMap map);
    }
}