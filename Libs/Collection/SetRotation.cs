using UnityEngine;

namespace MMGame.Collection
{
    public class SetRotation : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 rotation = Vector3.zero;

        [SerializeField]
        private bool local;

        public void Set()
        {
            Transform xform = target ? target.transform : transform;

            if (local)
            {
                xform.localEulerAngles = rotation;
            }
            else
            {
                xform.eulerAngles = rotation;
            }
        }
    }
}