using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从当前位置移动到 anchor 位置。
    /// </summary>
    public class UIMoveToPosition : AUISingleMove
    {
        [SerializeField]
        private Transform anchor;

        protected override bool ShowOffsetPercent()
        {
            return false;
        }

        protected override void PreparePlaying()
        {
            toPosition = anchor.position;
        }
    }
}