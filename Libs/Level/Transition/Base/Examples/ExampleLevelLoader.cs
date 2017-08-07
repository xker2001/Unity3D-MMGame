using EasyEditor;
using UnityEngine;

namespace MMGame.Level.Example
{
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class ExampleLevelLoader : MonoBehaviour, ILevelLoader
    {
        //--------------------------------------------------
        // 序列化参数
        //-------------------------------------------------

        [Message(text = "对象未实现 IAsyncProcessor！", messageType = MessageType.Error, method = "IsNotIAsyncProcessor")]
        [SerializeField]
        private MonoBehaviour asyncer;

        private bool IsNotIAsyncProcessor()
        {
            return !this.CheckOptionalInjection(asyncer, typeof(AAsyncProcessor));
        }

        public AAsyncProcessor Asyncer
        {
            get { return asyncer as AAsyncProcessor; }
        }

        [SerializeField]
        [Message(text = "对象未实现 ISceneFadeIn！", messageType = MessageType.Error, method = "IsNotISceneFadeIn")]
        private MonoBehaviour inFader;

        private bool IsNotISceneFadeIn()
        {
            return !this.CheckOptionalInjection(inFader, typeof(ASceneFadeInProcessor));
        }

        public ASceneFadeInProcessor InFader
        {
            get { return inFader as ASceneFadeInProcessor; }
        }

        [SerializeField]
        [Message(text = "对象未实现 ISceneFadeOut！", messageType = MessageType.Error, method = "IsNotISceneFadeOut")]
        private MonoBehaviour outFader;

        private bool IsNotISceneFadeOut()
        {
            return !this.CheckOptionalInjection(outFader, typeof(ASceneFadeOutProcessor));
        }

        public ASceneFadeOutProcessor OutFader
        {
            get { return outFader as ASceneFadeOutProcessor; }
        }

        [SerializeField]
        [Message(text = "对象未实现 ISceneBlackScreenProcessor！",
            messageType = MessageType.Error, method = "IsNotISceneBlackScreenProcessor")]
        private MonoBehaviour blackScreen;

        private bool IsNotISceneBlackScreenProcessor()
        {
            return !this.CheckOptionalInjection(blackScreen, typeof(ASceneBlackScreenProcessor));
        }

        public ASceneBlackScreenProcessor BlackScreen
        {
            get { return blackScreen as ASceneBlackScreenProcessor; }
        }

        //--------------------------------------------------
        // 注册服务
        //--------------------------------------------------

        private void OnEnable()
        {
            ServiceLocator.Register<ILevelLoader>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<ILevelLoader>();
        }

        //--------------------------------------------------
        // 接口方法
        //--------------------------------------------------

        public void LoadLevel(ALevelMap map, LoadMode mode)
        {
            LevelTransition.LoadLevel(map, mode);
        }

        public void LoadLevelAsync(ALevelMap map, LoadMode mode)
        {
            LevelTransition.LoadLevelAsync(map, LoadMode.Single, Asyncer);
        }

        public void SwitchToLevel(ALevelMap map)
        {
            LevelTransition.SwitchToLevel(map, OutFader, BlackScreen, InFader);
        }

        public void SwitchToLevelAsync(ALevelMap map)
        {
            LevelTransition.SwitchToLevelAsync(map, OutFader, BlackScreen, InFader, Asyncer);
        }
    }
}