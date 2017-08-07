using System;
using UnityEngine;

namespace MMGame.Level
{
    /// <summary>
    /// 关卡淡出控制接口。
    /// 注意应当在处理前打开黑屏，在 LevelTransition.OnLoadLevelCompleted 中关闭黑屏。
    /// </summary>
    abstract public class ASceneFadeOutProcessor : MonoBehaviour
    {
        /// <summary>
        /// 初始化 FadeOut 控制器以备下次执行。
        /// </summary>
        /// <param name="nextMap">下一个场景。</param>
        abstract public void InitFadeOut(ALevelMap nextMap);

        /// <summary>
        /// 执行淡出。
        /// </summary>
        /// <param name="nextMap"> 下一个场景。</param>
        /// <param name="onCompleted">Fade out 完成后的回调函数。</param>
        abstract public void FadeOut(ALevelMap nextMap, Action<ALevelMap> onCompleted);
    }
}