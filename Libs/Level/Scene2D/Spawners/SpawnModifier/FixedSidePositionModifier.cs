using UnityEngine;

namespace MMGame.Scene2D
{
    public class FixedSidePositionModifier : ASpawnElementModifier
    {
        /// <summary>
        /// 场景是水平卷轴还是垂直卷轴。
        /// </summary>
        [SerializeField]
        private ScrollAxis scrollAxis;

        /// <summary>
        /// 水平滚动时的 Y 轴位置；垂直滚动时的 X 轴位置。
        /// </summary>
        [SerializeField]
        private float sidePosition;

        public override void Modify(ASceneElement element)
        {
            Vector3 elementPos;

            if (scrollAxis == ScrollAxis.Horizontal)
            {
                elementPos =
                    new Vector3(element.Position.x, sidePosition, element.Position.z);
            }
            else
            {
                elementPos =
                    new Vector3(sidePosition, element.Position.y, element.Position.z);
            }

            element.SetPosition(elementPos);
        }
    }
}