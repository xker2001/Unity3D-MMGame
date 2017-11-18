using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从指定位置移动到屏幕外。
    /// </summary>
    public class UIMoveOutFromPosition : AUISingleMove
    {
        [SerializeField]
        private Transform from;

        [SerializeField]
        private OutScreenAnchor to;

        protected override bool ShowUseAwakePosition()
        {
            return false;
        }

        protected override void PreparePlaying()
        {
            toPosition = GetOutScreenPosition(to, from.position);
            rectTransform.position = from.position;
        }
    }
}