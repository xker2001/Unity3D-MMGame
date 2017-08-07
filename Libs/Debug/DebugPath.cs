using UnityEngine;
using System.Collections;

namespace MMGame {
[ExecuteInEditMode]
public class DebugPath : MonoBehaviour {
	[SerializeField] private bool _playOnStart = true;
	[SerializeField] private Color _color = Color.blue;
	[SerializeField] private bool _closed;
	[SerializeField] private float _nodeRadius = 50;
	[SerializeField] private Transform _mover;
	[SerializeField] private float _moveSpeed = 30;
	[SerializeField] private float _rotateSpeed = 1;

	private Transform[] _nodes;
	private Vector3 _targetPosition;
	private int _targetIndex;
	private bool _isPlaying;

	private void Start() {
		DebugPathNode[] nodes = GetComponentsInChildren<DebugPathNode>();
		_nodes = new Transform[nodes.Length];

		for (int i = 0; i < nodes.Length; i++) {
			_nodes[i] = nodes[i].transform;
		}

		if (_mover) {
			_mover.position = _nodes[0].position;

			if (_nodes.Length > 1) {
				_mover.forward = _nodes[1].position - _nodes[0].position;
			}
		}

		if (!Application.isPlaying) {
			return;
		}

		SetTargetIndex (1);

		if (_playOnStart) {
			Play();
		}
	}

	private void Update() {
		if (Application.isPlaying && _isPlaying && _mover) {
			Rotate();
			Move();
		}
	}

	public void Play() {
		_isPlaying = true;
	}

	private void Rotate() {
		Vector3 targetDirection = _targetPosition - _mover.position;
		Vector3 newDir = Vector3.RotateTowards (_mover.forward, targetDirection,
												_rotateSpeed * Time.deltaTime, 0.0f);
		_mover.rotation = Quaternion.LookRotation (newDir, Vector3.up);
	}

	private void Move() {
		float moveDist = _moveSpeed * Time.deltaTime;
		float distToTarget = (_targetPosition - _mover.position).magnitude;

		if (moveDist > distToTarget && !_closed && _targetIndex == _nodes.Length - 1) {
			_mover.position = _targetPosition;
			_isPlaying = false;
		}
		else {
			_mover.Translate (Vector3.forward * moveDist);

			if ( (_targetPosition - _mover.position).sqrMagnitude <= _nodeRadius * _nodeRadius) {
				if (_targetIndex == _nodes.Length - 1 && _closed) {
					SetTargetIndex (0);
				}
				else if (_targetIndex < _nodes.Length - 1) {
					SetTargetIndex (_targetIndex + 1);
				}
			}
		}
	}

	private void SetTargetIndex (int index) {
		_targetIndex = index;
		_targetPosition = _nodes[index].position;
	}

	private void OnDrawGizmos() {
		Gizmos.color = _color;

		for (int i = 0; i < _nodes.Length - 1; i++) {
			Gizmos.DrawLine (_nodes[i].position, _nodes[i + 1].position);
		}

		if (_closed && _nodes.Length > 2) {
			Gizmos.DrawLine (_nodes[_nodes.Length - 1].position, _nodes[0].position);
		}
	}
}
}
