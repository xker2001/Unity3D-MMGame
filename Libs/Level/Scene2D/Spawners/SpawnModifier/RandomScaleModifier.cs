using UnityEngine;

namespace MMGame.Scene2D
{
    public class RandomScaleModifier : ASpawnElementModifier
    {
        [SerializeField]
        private float minRelativeScale = 1;

        [SerializeField]
        private float maxRelativeScale = 1;

        public override void Modify(ASceneElement element)
        {
            if (Mathf.Approximately(minRelativeScale, maxRelativeScale))
            {
                element.SetRelativeScale(minRelativeScale * Vector3.one);
            }
            else
            {
                element.SetRelativeScale(Random.Range(minRelativeScale, maxRelativeScale) * Vector3.one);
            }
        }
    }
}