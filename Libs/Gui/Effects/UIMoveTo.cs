using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从当前位置移动到 anchor 位置。
    /// </summary>
    public class UIMoveTo : AUISingleMove
    {
        [SerializeField]
        private Transform anchor;

        protected override void InitPlaying()
        {
            toPosition = anchor.position;
        }
    }
}