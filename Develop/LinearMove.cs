using UnityEngine;

namespace MMGame.Develop
{
    public class LinearMove : MonoBehaviour
    {
        [SerializeField]
        private Vector3 direction = Vector3.right;

        [SerializeField]
        private float speed = 1;

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}