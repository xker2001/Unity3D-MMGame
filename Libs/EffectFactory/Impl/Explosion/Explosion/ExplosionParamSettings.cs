using UnityEngine;
using EasyEditor;

namespace MMGame.EffectFactory.Explosion
{
    [System.Serializable]
    public class ExplosionParamSettings : Settings<ExplosionParamSettings>
    {
        /// <summary>
        /// 爆炸地面痕迹特效放置离地面的距离。
        /// </summary>
        [SerializeField] private float groundOffset = 0.03f;

        /// <summary>
        /// 在 Game 命名空间中实现 Agent 组件。
        /// 将 Agent 组件添加到 ExplosionParamSettings 所在物体，再将组件拖放到本字段。
        /// </summary>
        [Message(text = "IExplosionParamAgent only!", method = "IsNotIExplosionParamAgent")]
        [SerializeField] private MonoBehaviour agent;

        public IExplosionParamAgent Agent
        {
            get { return agent.GetComponent<IExplosionParamAgent>(); }
        }

        public float GroundOffset
        {
            get { return groundOffset; }
        }

        private bool IsNotIExplosionEnergyOutput()
        {
            return agent != null && agent.GetComponent<IExplosionParamAgent>() == null;
        }
    }
}