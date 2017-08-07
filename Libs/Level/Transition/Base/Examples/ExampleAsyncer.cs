using System.Collections;
using UnityEngine;

namespace MMGame.Level.Example
{
    /// <summary>
    /// 异步加载控制器范例。
    /// </summary>
    public class ExampleAsyncer : AAsyncProcessor
    {
        private float lastProgress = -1;

        public override bool AllowLevelActivation { get; set; }

        public override void StartProgress(ALevelMap map)
        {
            AllowLevelActivation = false;
            Debug.LogFormat("<b>StartProgress</b>/{0}/{1}", map.ReferencedLevel.SceneName, map.SceneName);
        }

        public override void UpdateProgress(float progress, ALevelMap map)
        {
            if (!Mathf.Approximately(lastProgress, progress))
            {
                Debug.LogFormat("<b>UpdateProgress</b>/{0}/{1}", progress, map.SceneName);
                this.lastProgress = progress;
            }
        }

        public override void PromptToActivate(ALevelMap map)
        {
            Debug.LogFormat("<b>PromptToActivate</b>/{0}", map.SceneName);
            StartCoroutine(CheckInput());
        }

        private IEnumerator CheckInput()
        {
            Debug.Log("Precess any key to continue...");

            while (!AllowLevelActivation)
            {
                if (Input.anyKeyDown)
                {
                    AllowLevelActivation = true;
                }

                yield return null;
            }
        }
    }
}