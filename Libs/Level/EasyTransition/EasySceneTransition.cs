using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMGame.Level
{
    /// <summary>
    /// 基于加载新场景进行切换的轻量级场景过渡效果。
    /// </summary>
    public class EasySceneTransition : AEasyTransition
    {
        /// <summary>
        /// 本次切换将被加载的新场景。
        /// </summary>
        [SerializeField]
        private string sceneName;

        protected override void SwitchScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}