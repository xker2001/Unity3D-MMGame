using EasyEditor;
using UnityEngine;

namespace MMGame.EffectFactory
{
    public class EffectPlayer<T> : MonoBehaviour, IEffect where T : PlayOneShotParamFactory
    {
        [Inspector(rendererType = "InlineClassRenderer")]
        [SerializeField]
        private T effectParams;
        private PlayOneShotParamObject effectObj;

        public void PlayOneShot()
        {
            if (!effectParams.IsNull())
            {
                effectObj = effectParams.Create(transform);
                effectObj.PlayAndDestroy();
            }
        }

        public void Stop()
        {
            if (effectObj)
            {
                effectObj.Destroy();
                effectObj = null;
            }
        }
    }
}