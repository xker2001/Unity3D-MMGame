using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从当前位置移动到屏幕外。
    /// 注意控件总是从当前位置开始移动。如果需要从特定位置开始移动，
    /// 则需要自行将控件设置到起始位置。
    /// </summary>
    public class UIMoveOut : AUISingleMove
    {
        [SerializeField]
        private OutScreenAnchor anchor;

        protected override void PreparePlaying()
        {
            toPosition = GetOutScreenPosition(anchor, rectTransform.position);
        }
    }
}