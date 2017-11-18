using System;
using UnityEngine;

namespace MMGame.Sound
{
    [Serializable]
    public class MusicParameters
    {
        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        [RangeAttribute(0, 1)]
        private float volume = 0.6f;

        [SerializeField]
        private float fadeInDuration = 0.8f;

        [SerializeField]
        private float fadeOutDuration = 0.8f;

        public AudioClip Clip
        {
            get { return clip; }
        }

        public float Volume
        {
            get { return volume; }
        }

        public float FadeInDuration
        {
            get { return fadeInDuration; }
        }

        public float FadeOutDuration
        {
            get { return fadeOutDuration; }
        }

        public bool IsNull()
        {
            return clip == null || volume <= Mathf.Epsilon;
        }
    }
}