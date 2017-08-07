using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace MMGame.Level
{
    /// <summary>
    /// 关卡过渡控制器。
    /// 该类自带 DontDestroyOnLoad 属性。
    /// 配合 ILevelLoader 服务使用。
    /// </summary>
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class LevelTransition : MonoBehaviour
    {
        private static LevelTransition instance;

        private static LevelTransition Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (LevelTransition) FindObjectOfType(typeof(LevelTransition));
                }

                return instance;
            }
        }

        /// <summary>
        /// 当前激活状态的关卡（Scene）
        /// </summary>
        public static ALevelMap ActiveLevel { get; private set; }

        /// <summary>
        /// 新场景加载成功后是否手动执行淡入过程。
        /// 如果是自动执行，则在加载成功后立即执行淡入（sceneLoaded 事件中）；
        /// 如果手动执行，则需要场景脚本主动调用 LevelTransition.FadeIn()
        /// 方法来启动淡入过程。
        /// </summary>
        public static bool ManuallyFadeIn { get; set; }

        // 关卡淡出的事件
        // 所有前一个 LevelMap 为 from，后一个为 to

        public static event Action<ALevelMap> WillFadeOut;
        public static event Action<ALevelMap> StartFadeOut;
        public static event Action<ALevelMap> FadeOutCompleted;

        // 保持黑屏的事件

        public static event Action<ALevelMap> WillBlackScreen;
        public static event Action<ALevelMap> StartBlackScreen;
        public static event Action<ALevelMap> BlackScreenCompleted;

        // 加载关卡的事件

        public static event Action<ALevelMap, LoadMode> WillLoadLevel;
        public static event Action<ALevelMap, LoadMode> StartLoadLevel;
        public static event Action<ALevelMap, LoadMode> LoadLevelPaused;

        /// <summary>
        /// 注意第一个 LevelMap（previous map）可能为 null
        /// </summary>
        public static event Action<ALevelMap, ALevelMap> ActiveLevelChanged;

        public static event Action<ALevelMap, LoadMode> LoadLevelCompleted;

        // 关卡淡入的事件

        public static event Action<ALevelMap> StartFadeIn;
        public static event Action<ALevelMap> FadeInCompleted;

        /// <summary>
        /// 关卡字典，用于向 SceneManager.sceneLoaded 传递加载中的 LevelMap 数据
        /// </summary>
        private static Dictionary<string, ALevelMap> loadingLevelDic = new Dictionary<string, ALevelMap>();

        /// <summary>
        /// 是否正在处于一个 switch 的过程中。
        /// 防止重叠 switch level。
        /// </summary>
        private static bool isSwitching;

        /// <summary>
        /// 是否异步切换关卡。
        /// </summary>
        private static bool isAsyncSwitching;

        private static ASceneFadeInProcessor currentInFader;
        private static ASceneBlackScreenProcessor currentBlackScreenProcessor;
        private static AAsyncProcessor currentAsyncer;

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        /// <summary>
        /// 同步加载关卡。
        /// </summary>
        /// <param name="map">待加载的关卡。</param>
        /// <param name="mode">加载模式。</param>
        public static void LoadLevel(ALevelMap map,
                                     LoadMode mode = LoadMode.Single)
        {
            Instance.DoLoadLevel(map, mode);
        }

        /// <summary>
        /// 异步加载关卡。
        /// </summary>
        /// <param name="map">待加载的关卡。</param>
        /// <param name="mode">加载模式。</param>
        /// <param name="asyncer">异步加载控制器。</param>
        public static void LoadLevelAsync(ALevelMap map,
                                          LoadMode mode = LoadMode.Single,
                                          AAsyncProcessor asyncer = null)
        {
            Instance.DoLoadLevelAsync(map, mode, asyncer);
        }

        /// <summary>
        /// 切换到指定关卡，限定 Sync + Single 模式。
        /// </summary>
        /// <param name="nextMap">目标关卡。</param>
        /// <param name="outFader">淡出控制器。</param>
        /// <param name="blackScreen">黑屏阶段控制器。</param>
        /// <param name="inFader">淡入控制器。</param>
        public static void SwitchToLevel(ALevelMap nextMap,
                                         ASceneFadeOutProcessor outFader = null,
                                         ASceneBlackScreenProcessor blackScreen = null,
                                         ASceneFadeInProcessor inFader = null)
        {
            Instance.DoSwitchToLevel(nextMap, outFader, blackScreen, inFader, null, false);
        }

        /// <summary>
        /// 切换到指定关卡，限定 Async + Single 模式。
        /// </summary>
        /// <param name="nextMap">目标关卡。</param>
        /// <param name="outFader">淡出控制器。</par
        /// am>
        /// <param name="blackScreen">黑屏阶段控制器。</param>
        /// <param name="inFader">淡入控制器。</param>
        /// <param name="asyncer">异步加载控制器。</param>
        public static void SwitchToLevelAsync(ALevelMap nextMap,
                                              ASceneFadeOutProcessor outFader = null,
                                              ASceneBlackScreenProcessor blackScreen = null,
                                              ASceneFadeInProcessor inFader = null,
                                              AAsyncProcessor asyncer = null)
        {
            Instance.DoSwitchToLevel(nextMap, outFader, blackScreen, inFader, asyncer, true);
        }

        /// <summary>
        /// 执行场景淡入过程。
        /// </summary>
        public static void FadeIn()
        {
            if (currentInFader == null)
            {
                Debug.LogErrorFormat("Try to fade in when there is no in fader: {0}", ActiveLevel.SceneName);
                return;
            }

            Instance.DoFadeIn(ActiveLevel, currentInFader);
        }

        /// <summary>
        /// 激活场景改变的回调函数。
        /// </summary>
        private void OnActiveSceneChanged(Scene previousScene, Scene currentScene)
        {
            // 变更当前激活的关卡
            ALevelMap currentMap = GetLevelMap(currentScene);

            ALevelMap previousMap = ActiveLevel;
            SetActiveLevel(currentMap);

            // 发送关卡变更事件
            if (ActiveLevelChanged != null)
            {
                ActiveLevelChanged(previousMap, currentMap);
            }
        }

        /// <summary>
        /// 场景加载完毕的回调函数。
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 发送 OnLoadLevelCompleted 事件
            if (LoadLevelCompleted != null)
            {
                ALevelMap map = GetLevelMap(scene);

                LoadMode loadMode = mode == LoadSceneMode.Additive ? LoadMode.Additive : LoadMode.Single;
                LoadLevelCompleted(map, loadMode); // 通常在这里设置 ManuallyFadeIn
            }

            // 载入的关卡不是当前激活状态的关卡
            if (ActiveLevel.SceneName != scene.name)
            {
                return;
            }

            // 没有淡入控制器，切换关卡过程结束
            if (currentInFader == null)
            {
                isSwitching = false;
                return;
            }

            // 初始化 fade in，确保打开黑屏
            DoInitFadeIn(ActiveLevel, currentInFader);

            // 自动执行 fade in
            if (!ManuallyFadeIn)
            {
                DoFadeIn(ActiveLevel, currentInFader);
            }
        }

        /// <summary>
        /// 同步加载关卡。
        /// </summary>
        /// <param name="map">待加载的关卡。</param>
        /// <param name="mode">加载模式。</param>
        private void DoLoadLevel(ALevelMap map, LoadMode mode)
        {
            map.ReferencedLevel = ActiveLevel;
            StoreLoadingLevelMap(map);
            LoadSceneMode loadMode = mode == LoadMode.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if (WillLoadLevel != null)
            {
                WillLoadLevel(map, mode);
            }

            if (StartLoadLevel != null)
            {
                StartLoadLevel(map, mode);
            }

            SceneManager.LoadScene(map.SceneName, loadMode);
        }

        /// <summary>
        /// 异步加载关卡。
        /// </summary>
        /// <param name="map">待加载的关卡。</param>
        /// <param name="mode">加载模式。</param>
        /// <param name="asyncer">异步加载控制器。</param>
        private void DoLoadLevelAsync(ALevelMap map, LoadMode mode, AAsyncProcessor asyncer)
        {
            map.ReferencedLevel = ActiveLevel;
            StoreLoadingLevelMap(map);

            if (WillLoadLevel != null)
            {
                WillLoadLevel(map, mode);
            }

            if (StartLoadLevel != null)
            {
                StartLoadLevel(map, mode);
            }

            StartCoroutine(LoadLevelAsyncCoroutine(map, mode, asyncer));
        }

        /// <summary>
        /// 异步加载迭代器。
        /// </summary>
        private IEnumerator LoadLevelAsyncCoroutine(ALevelMap map, LoadMode mode, AAsyncProcessor asyncer)
        {
            if (asyncer != null)
            {
                asyncer.StartProgress(map);
            }

            LoadSceneMode loadMode = mode == LoadMode.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(map.SceneName, loadMode);
            // 没有 asyncer 时设置为 true，反之设置为 asyncer.AllowSceneActivation
            asyncOp.allowSceneActivation = asyncer == null || asyncer.AllowLevelActivation;

            float progress = 0;

            while (!asyncOp.isDone)
            {
                if (asyncer == null)
                {
                    yield return null;
                    continue;
                }

                // 注意奇怪的地方：
                // 1. 当下一个场景很小，且没有 print("")，暂停前输出的 progress 是 0。
                // 2. 当下一个场景很小，有 print("")，暂停前输出的 progress 是 0.9。
                // 3. 当下一个场景很大，没有 print("")，暂停前输出的 progress 是 0.9，且前面有一大串正确的递增过程。
                // 4. 这里注意，大量加载同一个 model 即可获得漂亮的 progress。

                // print("");
                asyncer.UpdateProgress(asyncOp.progress, map);

                // AllowSceneActivation 在 PromptToActivate 中设置为 true
                asyncOp.allowSceneActivation = asyncer.AllowLevelActivation;

                // 非自动激活场景，第一次到达 0.9 时启动提示激活场景的方法
                if (!asyncOp.allowSceneActivation && asyncOp.progress >= 0.9f && progress < 0.9f)
                {
                    if (LoadLevelPaused != null)
                    {
                        LoadLevelPaused(map, mode);
                    }

                    asyncer.PromptToActivate(map);
                }

                progress = asyncOp.progress;
                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// 切换到指定关卡，限定 Single 模式。
        /// </summary>
        /// <param name="nextMap">目标关卡。</param>
        /// <param name="outFader">淡出控制器。</param>
        /// <param name="blackScreen">黑屏阶段控制器。</param>
        /// <param name="inFader">淡入控制器。</param>
        /// <param name="asyncer">异步加载控制器。</param>
        /// <param name="async">是否异步加载。</param>
        private void DoSwitchToLevel(ALevelMap nextMap,
                                     ASceneFadeOutProcessor outFader,
                                     ASceneBlackScreenProcessor blackScreen,
                                     ASceneFadeInProcessor inFader,
                                     AAsyncProcessor asyncer,
                                     bool async)
        {
            Assert.IsNotNull(nextMap);

            if (isSwitching)
            {
                Debug.LogError("LevelTransition is in switching");
                return;
            }

            isSwitching = true;
            ManuallyFadeIn = false;
            StoreProcessors(blackScreen, asyncer, inFader);
            isAsyncSwitching = async;

            if (outFader != null)
            {
                DoFadeOut(nextMap, outFader);
                return; // 流程转入 FadeOutCompletedCallback
            }

            if (blackScreen != null)
            {
                DoProcessBlackScreen(nextMap, blackScreen);
                return; // 流程转入 BlackScreenCompletedCallback
            }

            if (async)
            {
                DoLoadLevelAsync(nextMap, LoadMode.Single, asyncer);
            }
            else
            {
                DoLoadLevel(nextMap, LoadMode.Single);
            }
        }

        /// <summary>
        /// 执行场景淡出过程。
        /// </summary>
        /// <param name="nextMap">下一个关卡。</param>
        /// <param name="outFader">淡出控制器。</param>
        private void DoFadeOut(ALevelMap nextMap, ASceneFadeOutProcessor outFader)
        {
            Assert.IsNotNull(outFader);

            if (WillFadeOut != null)
            {
                WillFadeOut(nextMap);
            }

            outFader.InitFadeOut(nextMap);

            if (StartFadeOut != null)
            {
                StartFadeOut(nextMap);
            }

            outFader.FadeOut(nextMap, FadeOutCompletedCallback);
        }

        /// <summary>
        /// 执行黑屏处理过程。
        /// </summary>
        /// <param name="nextMap">下一个关卡。</param>
        /// <param name="blackScreen">黑屏控制器。</param>
        private void DoProcessBlackScreen(ALevelMap nextMap, ASceneBlackScreenProcessor blackScreen)
        {
            Assert.IsNotNull(blackScreen);

            if (WillBlackScreen != null)
            {
                WillBlackScreen(nextMap);
            }

            blackScreen.InitBlackScreenProcessor(nextMap);

            if (StartBlackScreen != null)
            {
                StartBlackScreen(nextMap);
            }

            blackScreen.ProcessBlackScreen(nextMap, BlackScreenCompletedCallback);
        }


        private void DoInitFadeIn(ALevelMap map, ASceneFadeInProcessor inFader)
        {
            Assert.IsNotNull(map);
            Assert.IsNotNull(inFader);

            inFader.InitFadeIn(map);
        }

        /// <summary>
        /// 执行场景淡入过程。
        /// </summary>
        /// <param name="map">当前关卡。</param>
        /// <param name="inFader">淡入控制器。</param>
        private void DoFadeIn(ALevelMap map, ASceneFadeInProcessor inFader)
        {
            Assert.IsNotNull(map);
            Assert.IsNotNull(inFader);

            if (StartFadeIn != null)
            {
                StartFadeIn(map);
            }

            inFader.FadeIn(map, FadeInCompletedCallback);
        }

        /// <summary>
        /// Fade out 完成后的回调函数。
        /// </summary>
        private void FadeOutCompletedCallback(ALevelMap nextMap)
        {
            if (FadeOutCompleted != null)
            {
                FadeOutCompleted(nextMap);
            }

            if (currentBlackScreenProcessor != null)
            {
                DoProcessBlackScreen(nextMap, currentBlackScreenProcessor);
                return; // 流程转入 BlackScreenCompletedCallback
            }

            if (isAsyncSwitching)
            {
                DoLoadLevelAsync(nextMap, LoadMode.Single, currentAsyncer);
            }
            else
            {
                DoLoadLevel(nextMap, LoadMode.Single);
            }
        }

        /// <summary>
        /// Process black screen 完成后的回调函数。
        /// </summary>
        private void BlackScreenCompletedCallback(ALevelMap nextMap)
        {
            if (BlackScreenCompleted != null)
            {
                BlackScreenCompleted(nextMap);
            }

            if (isAsyncSwitching)
            {
                DoLoadLevelAsync(nextMap, LoadMode.Single, currentAsyncer);
            }
            else
            {
                DoLoadLevel(nextMap, LoadMode.Single);
            }
        }

        /// <summary>
        /// Fade in 完成后的回调函数。
        /// </summary>
        private void FadeInCompletedCallback(ALevelMap map)
        {
            isSwitching = false; // 关卡切换过程结束

            if (FadeInCompleted != null)

            {
                FadeInCompleted(map);
            }
        }

        /// <summary>
        /// 设置当前激活的关卡。
        /// </summary>
        /// <param name="map">关卡数据。</param>
        private void SetActiveLevel(ALevelMap map)
        {
            ActiveLevel = map;
        }

        /// <summary>
        /// 保存当前关卡切换过程中要使用的各个控制器。
        /// </summary>
        /// <param name="blackScreen">黑屏控制器。</param>
        /// <param name="asyncer">异步加载控制器。</param>
        /// <param name="inFader">淡入控制器。</param>
        private void StoreProcessors(ASceneBlackScreenProcessor blackScreen,
                                     AAsyncProcessor asyncer, ASceneFadeInProcessor inFader)
        {
            currentBlackScreenProcessor = blackScreen;
            currentAsyncer = asyncer;
            currentInFader = inFader;
        }

        /// <summary>
        /// 将正在加载的关卡数据保存到字典，供 OnSceneLoaded 回调时取回。
        /// </summary>
        /// <param name="map">关卡数据。</param>
        private void StoreLoadingLevelMap(ALevelMap map)
        {
            if (loadingLevelDic.ContainsKey(map.SceneName))
            {
                loadingLevelDic[map.SceneName] = map;
            }
            else
            {
                loadingLevelDic.Add(map.SceneName, map);
            }
        }

        /// <summary>
        /// 根据正在加载的关卡名称获取 LevelMap 数据。
        /// </summary>
        /// <param name="currentScene"></param>
        /// <returns></returns>
        private static ALevelMap GetLevelMap(Scene currentScene)
        {
            return loadingLevelDic.ContainsKey(currentScene.name)
                       ? loadingLevelDic[currentScene.name]
                       : new DefaultLevelMap(currentScene.name);
        }
    }
}