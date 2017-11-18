using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MMGame.UI
{
    /// <summary>
    /// 支持按下动画的按钮。
    /// 增加适当响应延迟以改善操作体验。
    /// 思路来自 GUI Pack Cartoon 中的相同实现。
    /// </summary>
    public class UIAnimatedButton : UIBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private enum RespondType
        {
            OnPointerDown,
            OnPointerClick
        }

        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}

        [SerializeField]
        private RespondType type = RespondType.OnPointerClick;

        public bool interactable = true;
        public ButtonClickedEvent onClick = new ButtonClickedEvent();

        [SerializeField, Required]
        private Animator animator;

        /// <summary>
        /// 按钮按下的动画。
        /// </summary>
        [SerializeField]
        private string animationTriggerName = "Pressed";

        /// <summary>
        /// 按下按钮后的响应时间延迟。
        /// </summary>
        [SerializeField]
        private float delay = 0.1f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (type != RespondType.OnPointerDown || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Press();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (type != RespondType.OnPointerClick || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Press();
        }

        private void Press()
        {
            if (!IsActive() || !interactable)
            {
                return;
            }

            if (animator)
            {
                animator.SetTrigger(animationTriggerName);
            }

            if (delay > Mathf.Epsilon)
            {
                Invoke("InvokeOnClick", delay);
            }
            else
            {
                onClick.Invoke();
            }
        }

        private void InvokeOnClick()
        {
            onClick.Invoke();
        }
    }
}