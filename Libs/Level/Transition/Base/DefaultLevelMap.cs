namespace MMGame.Level
{
    public class DefaultLevelMap : ALevelMap
    {
        public override string SceneName { get; protected set; }
        public override ALevelMap ReferencedLevel { get; set; }

        public DefaultLevelMap(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}