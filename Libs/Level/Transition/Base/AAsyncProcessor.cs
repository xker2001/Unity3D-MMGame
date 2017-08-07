using UnityEngine;

namespace MMGame.Level
{
    /// <summary>
    /// 异步加载控制器。
    /// 注意应当在处理前打开黑屏，在 LevelTransition.OnLoadLevelCompleted 中关闭黑屏。
    /// </summary>
    abstract public class AAsyncProcessor : MonoBehaviour
    {
        /// <summary>
        /// 是否允许激活被加载的关卡激活。
        /// LevelTransition 查询该属性来决定是否激活关卡。
        /// </summary>
        abstract public bool AllowLevelActivation { get; set; }

        /// <summary>
        /// 进行更新加载进度前的准备工作。
        /// </summary>
        /// <param name="map">正在加载的场景。</param>
        abstract public void StartProgress(ALevelMap map);

        /// <summary>
        /// 更新加载进度。
        /// </summary>
        /// <param name="progress">当前加载进度。</param>
        /// <param name="map">正在加载的场景。</param>
        abstract public void UpdateProgress(float progress, ALevelMap map);

        /// <summary>
        /// 提示激活被加载的关卡。
        /// 激活的方式为设置 AllowLevelActivation 为 true。
        /// </summary>
        /// <param name="map">正在加载的场景。</param>
        abstract public void PromptToActivate(ALevelMap map);
    }
}