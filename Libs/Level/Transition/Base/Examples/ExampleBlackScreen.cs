using System;
using System.Collections;
using UnityEngine;

namespace MMGame.Level.Example
{
    public class ExampleBlackScreen : ASceneBlackScreenProcessor
    {
        public override void InitBlackScreenProcessor(ALevelMap map)
        {
            Debug.LogFormat("<b>InitBlackScreenProcessor</b>/{0}", map.SceneName);
        }

        public override void ProcessBlackScreen(ALevelMap nextMap, Action<ALevelMap> onCompleted)
        {
            Debug.LogFormat("<b>ProcessBlackScreen</b>/{0}", nextMap.SceneName);
            StartCoroutine(InvokeCompletedCallback(nextMap, onCompleted));
        }

        private IEnumerator InvokeCompletedCallback(ALevelMap nextMap, Action<ALevelMap> completedCallback)
        {
            yield return new WaitForSeconds(2);
            completedCallback(nextMap);
        }
    }
}