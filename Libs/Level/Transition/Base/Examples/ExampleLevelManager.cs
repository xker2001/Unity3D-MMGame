using EasyEditor;
using UnityEngine;

namespace MMGame.Level.Example
{
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class ExampleLevelManager : MonoBehaviour
    {
        [SerializeField]
        private string TargetLevelName;

        void Awake()
        {
            LevelTransition.WillFadeOut += OnWillFadeOut;
            LevelTransition.StartFadeOut += OnStartFadeOut;
            LevelTransition.FadeOutCompleted += OnFadeOutCompleted;

            LevelTransition.WillBlackScreen += OnWillBlackScreen;
            LevelTransition.StartBlackScreen += OnStartBlackScreen;
            LevelTransition.BlackScreenCompleted += OnBlackScreenCompleted;

            LevelTransition.WillLoadLevel += OnWillLoadLevel;
            LevelTransition.StartLoadLevel += OnStartLoadLevel;
            LevelTransition.LoadLevelPaused += OnLoadLevelPaused;
            LevelTransition.ActiveLevelChanged += OnActiveLevelChanged;
            LevelTransition.LoadLevelCompleted += OnLoadLevelCompleted;

            LevelTransition.StartFadeIn += OnStartFadeIn;
            LevelTransition.FadeInCompleted += OnFadeInCompleted;
        }

        //--------------------------------------------------
        // 侦听关卡淡出事件
        //--------------------------------------------------

        private void OnWillFadeOut(ALevelMap map)
        {
            Debug.LogFormat("<b>OnWillFadeOut</b>/{0}", map.SceneName);
        }

        private void OnStartFadeOut(ALevelMap map)
        {
            Debug.LogFormat("<b>OnStartFadeOut</b>/{0}", map.SceneName);
        }

        private void OnFadeOutCompleted(ALevelMap map)
        {
            Debug.LogFormat("<b>OnFadeOutCompleted</b>/{0}", map.SceneName);
        }

        //--------------------------------------------------
        // 侦听黑屏事件
        //--------------------------------------------------

        private void OnWillBlackScreen(ALevelMap map)
        {
            Debug.LogFormat("<b>OnWillBlackScreen</b>/{0}", map.SceneName);
        }

        private void OnStartBlackScreen(ALevelMap map)
        {
            Debug.LogFormat("<b>OnStartBlackScreen</b>/{0}", map.SceneName);
        }

        private void OnBlackScreenCompleted(ALevelMap map)
        {
            Debug.LogFormat("<b>OnBlackScreenCompleted</b>/{0}", map.SceneName);
        }

        //--------------------------------------------------
        // 侦听关卡淡入事件
        //--------------------------------------------------

        private void OnStartFadeIn(ALevelMap map)
        {
            Debug.LogFormat("<b>OnStartFadeIn</b>/{0}", map.SceneName);
        }

        private void OnFadeInCompleted(ALevelMap map)
        {
            Debug.LogFormat("<b>OnFadeInCompleted</b>/{0}", map.SceneName);
        }

        //--------------------------------------------------
        // 侦听关卡加载事件
        //--------------------------------------------------

        private void OnWillLoadLevel(ALevelMap map, LoadMode mode)
        {
            Debug.LogFormat("<b>OnWillLoadLevel</b>/{0}/{1}", map.SceneName, mode);
        }

        private void OnStartLoadLevel(ALevelMap map, LoadMode mode)
        {
            Debug.LogFormat("<b>OnStartLoadLevel</b>/{0}/{1}", map.SceneName, mode);
        }

        private void OnLoadLevelPaused(ALevelMap map, LoadMode mode)
        {
            Debug.LogFormat("<b>OnLoadLevelPaused</b>/{0}/{1}", map.SceneName, mode);
        }

        private void OnActiveLevelChanged(ALevelMap previousMap, ALevelMap currentMap)
        {
            string preName = previousMap == null ? "null" : previousMap.SceneName;
            Debug.LogFormat("<b>OnActiveLevelChanged</b>/{0}/{1}", preName, currentMap.SceneName);
        }

        private void OnLoadLevelCompleted(ALevelMap map, LoadMode mode)
        {
            Debug.LogFormat("<b>OnLoadLevelCompleted</b>/{0}/{1}", map.SceneName, mode);
        }

        //--------------------------------------------------
        // 测试方法
        //--------------------------------------------------

        [Inspector]
        public void LoadLevel()
        {
            ALevelMap map = new DefaultLevelMap(TargetLevelName);
            ServiceLocator.Get<ILevelLoader>().LoadLevel(map, LoadMode.Single);
        }

        [Inspector]
        public void LoadLevelAsync()
        {
            ALevelMap map = new DefaultLevelMap(TargetLevelName);
            ServiceLocator.Get<ILevelLoader>().LoadLevelAsync(map, LoadMode.Single);
        }

        [Inspector]
        public void SwitchToTargetLevel()
        {
            ALevelMap map = new DefaultLevelMap(TargetLevelName);
            ServiceLocator.Get<ILevelLoader>().SwitchToLevel(map);
        }

        [Inspector]
        public void SwitchToTargetLevelAsync()
        {
            ALevelMap map = new DefaultLevelMap(TargetLevelName);
            ServiceLocator.Get<ILevelLoader>().SwitchToLevelAsync(map);
        }
    }
}