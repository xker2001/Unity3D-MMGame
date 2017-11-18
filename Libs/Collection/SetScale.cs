using UnityEngine;

namespace MMGame.Collection
{
    public class SetScale : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 scale = Vector3.one;

        public void Set()
        {
            if (target)
            {
                target.transform.localScale = scale;
            }
            else
            {
                transform.localScale = scale;
            }
        }
    }
}