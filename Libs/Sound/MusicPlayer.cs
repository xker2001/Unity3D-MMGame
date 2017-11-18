using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Sound
{
    /// <summary>
    /// 简单的背景音乐播放组件，只能播放在 Inspector 中指定的音乐。
    /// </summary>
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField]
        private bool playOnStart;

        [SerializeField]
        private bool playOnEnable;

        [ShowIf("ShowDelayField")]
        [SerializeField]
        private float delay;

        private bool ShowDelayField()
        {
            return playOnEnable || playOnStart;
        }

        [SerializeField]
        private bool stopOnDisable;

        [SerializeField]
        private List<MusicParameters> musics;

        [ShowIf("ShowRandomOrder")]
        [SerializeField]
        private bool randomOrder;

        private bool ShowRandomOrder()
        {
            return musics.Count > 1;
        }

        [SerializeField]
        private bool loop;

        private bool isStarted;
        private int currentMusicIndex;
        private bool subscribed;

        private void Awake()
        {
            if (randomOrder)
            {
                musics.Shuffle();
            }
        }

        private void Start()
        {
            if (playOnStart)
            {
                DelayPlay();
            }

            isStarted = true;
        }

        private void OnEnable()
        {
            if (isStarted)
            {
                DelayPlay();
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (stopOnDisable)
            {
                Stop();
            }
        }

        public void Play(float? fadeInDuration = null)
        {
            if (musics.Count == 1 && loop)
            {
                Music.Loop(musics[0]);
                return;
            }

            currentMusicIndex = 0;

            if (!subscribed)
            {
                Music.PlayEnded += OnPlayEnded;
                subscribed = true;
            }

            Music.Play(musics[currentMusicIndex], fadeInDuration);
        }

        public void Stop(float? fadeOutDuration = null)
        {
            Music.PlayEnded -= OnPlayEnded;
            subscribed = false;
            Music.Stop(fadeOutDuration);
        }

        public void StopImmediately()
        {
            Music.PlayEnded -= OnPlayEnded;
            subscribed = false;
            Music.StopImmediately();
        }

        public void TurnDown(float percent, float? fadeOutDuration)
        {
            Music.TurnDown(percent, fadeOutDuration);
        }

        public void TurnDown(float? fadeOutDuration)
        {
            Music.TurnDown(fadeOutDuration);
        }

        public void TurnUp(float? fadeInDuration)
        {
            Music.TurnUp(fadeInDuration);
        }

        public void TurnOff(float? muteDuration = null)
        {
            Music.TurnOff(muteDuration);
        }

        public void TurnOffImmediately()
        {
            Music.TurnOffImmediately();
        }

        public void TurnOn(float? muteDuration = null)
        {
            Music.TurnOn(muteDuration);
        }

        public void TurnOnImmediately()
        {
            Music.TurnOnImmediately();
        }

        private void DelayPlay()
        {
            if (delay > Mathf.Epsilon)
            {
                Music.Stop(null);
                StartCoroutine(ExecuteDelayPlay(delay));
            }
            else
            {
                Play();
            }
        }

        private IEnumerator ExecuteDelayPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Play();
        }

        private void OnPlayEnded()
        {
            if (currentMusicIndex == musics.Count - 1 && loop)
            {
                currentMusicIndex = 0;
                Assert.IsNotNull(musics[currentMusicIndex]);
                Music.Play(musics[currentMusicIndex]);
            }
            else if (currentMusicIndex < musics.Count - 1)
            {
                currentMusicIndex += 1;
                Assert.IsNotNull(musics[currentMusicIndex]);
                Music.Play(musics[currentMusicIndex]);
            }
        }
    }
}