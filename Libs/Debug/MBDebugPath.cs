using UnityEngine;

namespace MMGame
{
    [ExecuteInEditMode]
    public class MBDebugPath : MonoBehaviour
    {
        [SerializeField]
        private bool playOnStart = true;
        [SerializeField]
        private Color color = Color.blue;
        [SerializeField]
        private bool closed;
        [SerializeField]
        private float nodeRadius = 50;
        [SerializeField]
        private Transform mover;
        [SerializeField]
        private float moveSpeed = 30;
        [SerializeField]
        private float rotateSpeed = 1;

        private Transform[] nodes;
        private Vector3 targetPosition;
        private int targetIndex;
        private bool isPlaying;

        private void Start()
        {
            MBDebugPathNode[] nodes = GetComponentsInChildren<MBDebugPathNode>();
            this.nodes = new Transform[nodes.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                this.nodes[i] = nodes[i].transform;
            }

            if (mover)
            {
                mover.position = this.nodes[0].position;

                if (this.nodes.Length > 1)
                {
                    mover.forward = this.nodes[1].position - this.nodes[0].position;
                }
            }

            if (!Application.isPlaying)
            {
                return;
            }

            SetTargetIndex(1);

            if (playOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
            if (Application.isPlaying && isPlaying && mover)
            {
                Rotate();
                Move();
            }
        }

        public void Play()
        {
            isPlaying = true;
        }

        private void Rotate()
        {
            Vector3 targetDirection = targetPosition - mover.position;
            Vector3 newDir = Vector3.RotateTowards(mover.forward, targetDirection,
                                                   rotateSpeed * Time.deltaTime, 0.0f);
            mover.rotation = Quaternion.LookRotation(newDir, Vector3.up);
        }

        private void Move()
        {
            float moveDist = moveSpeed * Time.deltaTime;
            float distToTarget = (targetPosition - mover.position).magnitude;

            if (moveDist > distToTarget && !closed && targetIndex == nodes.Length - 1)
            {
                mover.position = targetPosition;
                isPlaying = false;
            }
            else
            {
                mover.Translate(Vector3.forward * moveDist);

                if ((targetPosition - mover.position).sqrMagnitude <= nodeRadius * nodeRadius)
                {
                    if (targetIndex == nodes.Length - 1 && closed)
                    {
                        SetTargetIndex(0);
                    }
                    else if (targetIndex < nodes.Length - 1)
                    {
                        SetTargetIndex(targetIndex + 1);
                    }
                }
            }
        }

        private void SetTargetIndex(int index)
        {
            targetIndex = index;
            targetPosition = nodes[index].position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }

            if (closed && nodes.Length > 2)
            {
                Gizmos.DrawLine(nodes[nodes.Length - 1].position, nodes[0].position);
            }
        }
    }
}