using System;
using System.Collections;
using UnityEngine;

namespace MMGame.Level.Example
{
    public class ExampleOutFader : ASceneFadeOutProcessor
    {
        public override void InitFadeOut(ALevelMap map)
        {
            Debug.LogFormat("<b>InitFadeOut</b>/{0}", map.SceneName);
        }

        public override void FadeOut(ALevelMap nextMap, Action<ALevelMap> onCompleted)
        {
            Debug.LogFormat("<b>FadeOut</b>/{0}", nextMap.SceneName);
            StartCoroutine(InvokeCompletedCallback(nextMap, onCompleted));
        }

        private IEnumerator InvokeCompletedCallback(ALevelMap nextMap, Action<ALevelMap> completedCallback)
        {
            yield return new WaitForSeconds(2);
            completedCallback(nextMap);
        }
    }
}