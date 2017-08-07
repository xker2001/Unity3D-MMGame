using EasyEditor;
using MMGame.Level;
using UnityEngine;

namespace MMGame.SimpleLevelManager
{
    public class SimpleLevelLoader : MonoBehaviour, ILevelLoader
    {
        //--------------------------------------------------
        // 过渡效果控制器
        //--------------------------------------------------

        /// <summary>
        /// 淡出控制器。
        /// </summary>
        [SerializeField]
        private ASceneFadeOutProcessor outFader;

        /// <summary>
        /// 黑屏控制器。
        /// </summary>
        [SerializeField]
        private ASceneBlackScreenProcessor blackScreen;

        /// <summary>
        /// 异步加载控制器。
        /// </summary>
        [SerializeField]
        private AAsyncProcessor asyncer;

        /// <summary>
        /// 淡入控制器。
        /// </summary>
        [SerializeField]
        private ASceneFadeInProcessor inFader;

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
            LevelTransition.LoadLevelAsync(map, LoadMode.Single, asyncer);
        }

        public void SwitchToLevel(ALevelMap map)
        {
            LevelTransition.SwitchToLevel(map, outFader, blackScreen, inFader);
        }

        public void SwitchToLevelAsync(ALevelMap map)
        {
            LevelTransition.SwitchToLevelAsync(map, outFader, blackScreen, inFader, asyncer);
        }
    }
}