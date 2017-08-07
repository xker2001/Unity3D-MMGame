using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame
{
    /// <summary>
    /// Coroutine 封装类。
    /// </summary>
    public class UMCoroutine
    {
        public bool IsWaiting { get; private set; }
        public bool IsStopped { get; private set; }
        public bool IsPaused { get; private set; }
        public MonoBehaviour Owner { get; private set; }

        private IEnumerator enumerator;
        private IEnumerator waiter;

        public UMCoroutine(IEnumerator enumerator, MonoBehaviour owner)
        {
            this.enumerator = enumerator;
            Owner = owner;
            IsStopped = false;
        }

        public void Wait()
        {
            IsWaiting = waiter.MoveNext();
        }

        public void Stop()
        {
            IsStopped = true;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void MoveNext()
        {
            if (IsPaused)
            {
                return;
            }

            bool ended = !enumerator.MoveNext();

            if (ended)
            {
                IsStopped = true;
            }
            else if (enumerator.Current != null)
            {
                waiter = enumerator.Current as IEnumerator;
                Assert.IsTrue(waiter != null,
                    "UpdateManager: use UMWaitForSeconds instead of WaitForSeconds.");
                IsWaiting = true;
            }
        }
    }

    /// <summary>
    /// UMW ait for seconds.
    /// </summary>
    public class UMWaitForSeconds : IEnumerator
    {
        private float delay;
        private float elapse;

        public UMWaitForSeconds(float delay)
        {
            this.delay = delay;
            elapse = 0;
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            elapse += UpdateManager.DeltaTime;
            return delay > elapse;
        }

        public void Reset()
        {
            elapse = 0;
        }
    }

    /// <summary>
    /// MonoBehaviour 的 UMCoroutine 扩展方法。
    /// </summary>
    public static class ExtendMonoBehaviourCoroutine
    {
        public static void UMStartCoroutine(this MonoBehaviour self, IEnumerator enumerator)
        {
            UMCoroutine c = new UMCoroutine(enumerator, self);
            c.MoveNext();

            if (!c.IsStopped)
            {
                UMCoroutineManager.AddCoroutine(c);
            }
        }

        public static void UMStopCoroutine(this MonoBehaviour self, UMCoroutine coroutine)
        {
            UMCoroutineManager.StopCoroutine(coroutine);
        }

        public static void UMStopAllCoroutines(this MonoBehaviour self)
        {
            UMCoroutineManager.StopAllCoroutines(self);
        }

        public static void UMPauseCoroutine(this MonoBehaviour self, UMCoroutine coroutine)
        {
            UMCoroutineManager.PauseCoroutine(coroutine);
        }

        public static void UMPauseAllCoroutines(this MonoBehaviour self)
        {
            UMCoroutineManager.PauseAllCoroutines(self);
        }

        public static void UMResumeCoroutine(this MonoBehaviour self, UMCoroutine coroutine)
        {
            UMCoroutineManager.ResumeCoroutine(coroutine);
        }

        public static void UMResumeAllCoroutines(this MonoBehaviour self)
        {
            UMCoroutineManager.ResumeAllCoroutines(self);
        }
    }
}