using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 界面特效组播放控制类。
    /// 注意，这里要求效果组件和组播放控制组件在同一个物体上。
    /// TODO: 由于现在效果组件可以不放在目标物体上，这里有待增强。
    /// </summary>
    public class UIEffectGroupPlayer : MonoBehaviour
    {
        private AUIEffect[] effects;

        /// <summary>
        /// 播放 game object 及其子物体中指定 group 的 UI 效果。
        /// 注意 disable 状态的特效组件不会被播放。
        /// </summary>
        /// <param name="group">特效 group。</param>
        public void PlayGroup(int group)
        {
            effects = gameObject.GetComponents<AUIEffect>();

            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].enabled && (int) (effects[i].Group) == group)
                {
                    effects[i].Play();
                }
            }
        }

        /// <summary>
        /// 停止播放 game object 及其子物体中指定 group 的 UI 效果。
        /// </summary>
        /// <param name="group"></param>
        public void StopGroup(int group)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].enabled && (int) (effects[i].Group) == group)
                {
                    effects[i].Stop();
                }
            }
        }
    }
}