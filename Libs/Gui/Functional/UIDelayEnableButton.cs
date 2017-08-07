using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace MMGame.UI
{
    /// <summary>
    /// OnEnable 后禁用按钮并在延迟后重新激活。
    /// </summary>
    public class UIDelayEnableButton : EasyUIBehaviour
    {
        [SerializeField]
        private bool executeOnEnable;

        [SerializeField]
        private float delay;

        private Button button;

        protected override void Awake()
        {
            base.Awake();
            button = gameObject.GetComponent<Button>();
            Assert.IsNotNull(button);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (executeOnEnable)
            {
                DisableButton();
            }
        }

        public void DisableButton()
        {
            if (delay <= Mathf.Epsilon)
            {
                return;
            }

            button.interactable = false;
            Invoke("EnableButton", delay);
        }

        private void EnableButton()
        {
            button.interactable = true;
        }
    }
}