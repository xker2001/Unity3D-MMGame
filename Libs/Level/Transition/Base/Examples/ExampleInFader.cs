using System;
using System.Collections;
using UnityEngine;

namespace MMGame.Level.Example
{
    public class ExampleInFader : ASceneFadeInProcessor
    {
        public override void InitFadeIn(ALevelMap map)
        {
            Debug.LogFormat("<b>InitFadeIn</b>/{0}", map.SceneName);
        }

        public override void FadeIn(ALevelMap map, Action<ALevelMap> onCompleted)
        {
            Debug.LogFormat("<b>FadeIn</b>/{0}", map.SceneName);
            StartCoroutine(InvokeCompletedCallback(map, onCompleted));
        }

        private IEnumerator InvokeCompletedCallback(ALevelMap nextMap, Action<ALevelMap> completedCallback)
        {
            yield return new WaitForSeconds(2);
            completedCallback(nextMap);
        }
    }
}