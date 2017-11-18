using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从 anchor 位置移动到当前位置。
    /// </summary>
    public class UIMoveBackFromPosition : AUISingleMove
    {
        [SerializeField]
        private Transform anchor;

        protected override bool ShowOffsetPercent()
        {
            return false;
        }

        protected override void PreparePlaying()
        {
            toPosition = GetOriginalPosition();
            rectTransform.position = anchor.position;
        }
    }
}