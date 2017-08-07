using UnityEngine;

namespace MMGame.EffectFactory.FlashLight
{
    public class LoopFlashLightExample : MonoBehaviour
    {
        public FlashLightLoopParamFactory EffectParams;
        public bool SmoothStop;
        private PlayLoopParamObject effectObj;

        void OnEnable()
        {
            if (!effectObj && !EffectParams.IsNull())
            {
                effectObj = EffectParams.Create(transform);
            }

            effectObj.Loop();
        }

        void OnDisable()
        {
            if (effectObj)
            {
                if (SmoothStop)
                {
                    effectObj.SmoothStop();
                }
                else
                {
                    effectObj.Stop();
                }
            }
        }
    }
}