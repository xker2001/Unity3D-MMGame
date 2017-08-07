using UnityEngine;
using UnityEngine.UI;

namespace MMGame.UI
{
    /// <summary>
    /// 简单的切换按钮。
    ///
    /// 使用说明：
    /// 1. 创建一个 Toggle 控件。
    /// 2. 删除 Toggle 控件的所有子控件。
    /// 3. 为 Toggle 控件创建两个子控件，分别用于显示开启状态和关闭状态。
    /// 4. 将子控件指为 IconOn 和 IconOff。
    /// 5. 将任一子控件指为 Toggle 的 Target Graphic。
    ///
    /// 使用场景范例：背景音乐开关按钮。
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class UISwitchButton : EasyUIBehaviour
    {
        [Tooltip("开启状态的按钮图片。")]
        [SerializeField]
        private GameObject iconOn;

        [Tooltip("关闭状态的按钮图片。")]
        [SerializeField]
        private GameObject iconOff;

        [SerializeField]
        private bool isOn;

        protected override void Awake()
        {
            base.Awake();
            SwitchIcon(IsOn);

            Toggle toggle = gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);
            toggle.isOn = IsOn;
        }

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                gameObject.GetComponent<Toggle>().isOn = isOn;
                SwitchIcon(isOn);
            }
        }

        private void OnClickToggle(bool status)
        {
            IsOn = status;
            SwitchIcon(status);
        }

        private void SwitchIcon(bool status)
        {
            iconOn.SetActive(status);
            iconOff.SetActive(!status);
        }
    }
}