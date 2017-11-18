using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从屏幕外移动到当前位置。
    /// </summary>
    public class UIMoveIn : AUISingleMove
    {
        [SerializeField]
        private OutScreenAnchor anchor;

        protected override void PreparePlaying()
        {
            toPosition = GetOriginalPosition();
            rectTransform.position = GetOutScreenPosition(anchor, rectTransform.position);
        }
    }
}