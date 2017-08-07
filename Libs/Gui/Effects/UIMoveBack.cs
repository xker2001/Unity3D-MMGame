using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 控件从 anchor 位置移动到当前位置。
    /// </summary>
    public class UIMoveBack : AUISingleMove
    {
        [SerializeField]
        private Transform anchor;

        protected override void InitPlaying()
        {
            toPosition = GetOriginalPosition();
            transform.position = anchor.position;
        }
    }
}