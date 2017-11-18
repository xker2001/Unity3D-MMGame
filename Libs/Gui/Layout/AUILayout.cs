using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MMGame.UI
{
    /// <summary>
    /// 布局器。
    /// </summary>
    abstract public class AUILayout : EasyUIBehaviour
    {
        /// <summary>
        /// 是否在需要时自动更新布局。
        /// </summary>
        [SerializeField]
        private bool autoLayout = true;

        /// <summary>
        /// 设置是否自动更新布局。
        /// </summary>
        /// <param name="value"></param>
        public void SetAutoLayout(bool value)
        {
            autoLayout = value;
        }

        /// <summary>
        /// 执行一次自动布局。
        /// 自动布局在派生类的相应方法中调用，如： 
        /// - Start()
        /// - OnTransformChildrenChanged()
        /// - Update() 
        /// - etc.
        /// </summary>
        protected void AutoLayout()
        {
            if (autoLayout)
            {
                Layout();
            }
        }

        /// <summary>
        /// 执行一次布局。
        /// </summary>
        abstract public void Layout();
    }
}