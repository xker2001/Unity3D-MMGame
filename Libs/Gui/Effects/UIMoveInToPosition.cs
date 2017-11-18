using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从屏幕外移动到指定位置。
    /// </summary>
    public class UIMoveInToPosition : AUISingleMove
    {
        [SerializeField]
        private OutScreenAnchor from;

        [SerializeField]
        private Transform to;

        protected override bool ShowUseAwakePosition()
        {
            return false;
        }

        protected override void PreparePlaying()
        {
            toPosition = to.position;
            rectTransform.position = GetOutScreenPosition(from, to.position);
        }
    }
}