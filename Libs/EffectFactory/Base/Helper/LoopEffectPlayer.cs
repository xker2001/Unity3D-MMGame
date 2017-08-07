using EasyEditor;
using UnityEngine;

namespace MMGame.EffectFactory
{
    public class LoopEffectPlayer<T> : MonoBehaviour, ILoopEffect where T : PlayLoopParamFactory
    {
        [SerializeField]
        private bool playOnEnable;

        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private T effectParams;

        private PlayLoopParamObject effectObj;
        private bool isLooping;

        void OnEnable()
        {
            if (playOnEnable)
            {
                Loop();
            }
        }

        public void Loop()
        {
            if (isLooping || effectParams.IsNull())
            {
                return;
            }

            effectObj = effectParams.Create(transform);
            effectObj.Loop();
            isLooping = true;
        }

        public void Stop()
        {
            if (effectObj)
            {
                effectObj.Destroy();
            }

            effectObj = null;
            isLooping = false;
        }

        public void SmoothStop()
        {
            if (effectObj)
            {
                effectObj.SmoothDestroy();
            }

            effectObj = null;
            isLooping = false;
        }
    }
}