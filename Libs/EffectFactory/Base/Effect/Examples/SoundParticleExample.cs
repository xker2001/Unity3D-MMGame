using UnityEngine;

namespace MMGame.EffectFactory
{
    public class SoundParticleExample : MonoBehaviour
    {
        public SoundParticleParamFactory EffectParams;
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