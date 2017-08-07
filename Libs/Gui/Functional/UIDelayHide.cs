using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// OnEnable 后延迟禁用自身。
    /// </summary>
    public class UIDelayHide : EasyUIBehaviour
    {
        [SerializeField]
        private bool hideOnEnable;

        [SerializeField]
        private float delay;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (hideOnEnable)
            {
                Hide();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke();
        }

        public void Hide()
        {
            if (delay <= Mathf.Epsilon)
            {
                HideSelf();
            }
            else
            {
                Invoke("HideSelf", delay);
            }
        }

        private void HideSelf()
        {
            gameObject.SetActive(false);
        }
    }
}