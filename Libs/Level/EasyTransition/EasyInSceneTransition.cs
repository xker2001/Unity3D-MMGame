using UnityEngine;

namespace MMGame.Level
{
    /// <summary>
    /// 基于 enable/disable 场景节点实现切换的轻量级场景过渡效果。
    /// </summary>
    public class EasyInSceneTransition : AEasyTransition
    {
        /// <summary>
        /// 本次切换将被 disable 的 GameObjects。
        /// </summary>
        [SerializeField]
        private GameObject[] unloadNodes;

        /// <summary>
        /// 本次切换将被 enable 的 GameObject。
        /// </summary>
        [SerializeField]
        private GameObject[] loadNodes;

        protected override void SwitchScene()
        {
            foreach (GameObject node in unloadNodes)
            {
                node.SetActive(false);
            }

            foreach (GameObject node in loadNodes)
            {
                node.SetActive(true);
            }
        }
    }
}