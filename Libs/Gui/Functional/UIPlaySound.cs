using UnityEngine;
using MMGame.EffectFactory;

namespace MMGame.UI
{
    public class UIPlaySound : EasyUIBehaviour
    {
        [SerializeField]
        private SoundParamFactory sound;

        [SerializeField]
        private bool playOnEnable;

        [SerializeField]
        private float delay;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (playOnEnable)
            {
                Play();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke();
        }

        public void Play()
        {
            if (sound.IsNull())
            {
                return;
            }

            if (delay <= Mathf.Epsilon)
            {
                PlaySound();
            }
            else
            {
                Invoke("PlaySound", delay);
            }
        }

        private void PlaySound()
        {
            sound.Create().PlayAndDestroy();
        }
    }
}