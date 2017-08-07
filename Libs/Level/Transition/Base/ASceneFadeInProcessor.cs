using System;
using UnityEngine;

namespace MMGame.Level
{
    /// <summary>
    /// 关卡淡入控制接口。
    /// 注意应当在处理前打开黑屏，在 onCompleted() 中关闭黑屏。
    /// </summary>
    abstract public class ASceneFadeInProcessor : MonoBehaviour
    {
        /// <summary>
        /// 初始化 FadeIn 控制器以备下次执行。
        /// </summary>
        /// <param name="map">当前场景。</param>
        abstract public void InitFadeIn(ALevelMap map);

        /// <summary>
        /// 执行淡入。
        /// </summary>
        /// <param name="map">当前场景。</param>
        /// <param name="onCompleted">Fade in 完成后的回调函数。</param>
        abstract public void FadeIn(ALevelMap map, Action<ALevelMap> onCompleted);
    }
}