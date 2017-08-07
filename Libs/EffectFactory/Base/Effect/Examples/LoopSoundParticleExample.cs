using UnityEngine;

namespace MMGame.EffectFactory
{
    public class LoopSoundParticleExample : MonoBehaviour
    {
        public SoundParticleLoopParamFactory EffectParams;
        public bool SmoothStop;
        private PlayLoopParamObject effectObj;

        void OnEnable()
        {
            if (!EffectParams.IsNull())
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