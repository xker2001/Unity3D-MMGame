using UnityEngine;
using EasyEditor;
using MMGame.EffectFactory.Explosion;

namespace MMGame.EffectFactory.FlashLight
{
    [System.Serializable]
    public class FlashLightSettings : Settings<FlashLightSettings>
    {
        /// <summary>
        /// 特效模型片离地面的高度。
        /// </summary>
        [SerializeField] private float groundOffset = 0.01f;

        /// <summary>
        /// 在 Game 命名空间中实现 Agent 组件。
        /// 将 Agent 组件添加到 FlashLightSettings 所在物体，再将组件拖放到本字段。
        /// </summary>
        [Message(text = "IFlashLightAgent only!", method = "IsNotIFlashLightAgent")]
        [SerializeField] private MonoBehaviour agent;

        public IFlashLightAgent Agent
        {
            get { return agent.GetComponent<IFlashLightAgent>(); }
        }

        public float GroundOffset
        {
            get { return groundOffset; }
        }

        private bool IsNotIFlashLightAgent()
        {
            return agent != null && agent.GetComponent<IExplosionParamAgent>() == null;
        }
    }
}