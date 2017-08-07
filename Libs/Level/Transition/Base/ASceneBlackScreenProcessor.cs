using System;
using UnityEngine;

namespace MMGame.Level
{
    /// <summary>
    /// 关卡淡出后黑屏阶段控制接口。
    /// 注意应当在处理前打开黑屏，在 LevelTransition.OnLoadLevelCompleted 中关闭黑屏。
    /// </summary>
    abstract public class ASceneBlackScreenProcessor : MonoBehaviour
    {
        /// <summary>
        /// 初始化黑屏控制器以备下次执行。
        /// </summary>
        /// <param name="nextMap">下一个场景。</param>
        abstract public void InitBlackScreenProcessor(ALevelMap nextMap);

        /// <summary>
        /// 执行淡入。
        /// </summary>
        /// <param name="nextMap">下一个场景。</param>
        /// <param name="onCompleted">Black screen 完成后的回调函数。</param>
        abstract public void ProcessBlackScreen(ALevelMap nextMap, Action<ALevelMap> onCompleted);
    }
}