using UnityEngine;

namespace MMGame.EffectFactory.FlashLight
{
    public class FlashLightExample : MonoBehaviour
    {
        public FlashLightParamFactory EffectParams;
        private PlayOneShotParamObject effectObj;

        void OnEnable()
        {
            if (!EffectParams.IsNull())
            {
                effectObj = EffectParams.Create(transform);
            }

            effectObj.PlayAndDestroy();
        }
    }
}