using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField]
    protected ViewMode _enemyType;

	[SerializeField]
	protected float _speed;

	#region References
	protected MeshRenderer _sprite;

	protected Collider _col;

	protected Rigidbody _rb;

	protected GameObject _target;
	#endregion

	private Vector3 _prevVelocity;

	protected void Start()
	{
		_sprite = GetComponent<MeshRenderer>();
		_col = GetComponent<Collider>();
		_rb = GetComponent<Rigidbody>();
		_target = GameObject.FindGameObjectWithTag("Player");
		InitEnemy();
	}

	protected void Update()
	{
		if (GameManager.Instance.IsPaused)
		{
			if (_prevVelocity == Vector3.zero) _prevVelocity = _rb.velocity;
			_rb.velocity = Vector3.zero;
			return;
		} else
		{
			_rb.velocity = _prevVelocity;
			_prevVelocity = Vector3.zero;
		}
		_sprite.enabled = _col.enabled = (_enemyType == CameraMode.CurrentCamMode);
	}

	protected void FixedUpdate()
	{
        if (GameManager.Instance.IsPaused) return;
		if (_enemyType == CameraMode.CurrentCamMode)
		{
			UpdateMovement();
			if (PlayerController.Instance.CurrentPlayerState != PlayerState.Damaged &&
				Vector3.Distance(transform.position, _target.transform.position) <= 2)
			{
				Vector3 knockbackDir = _target.transform.position - transform.position;
				knockbackDir.y = 0;
				if (_enemyType == ViewMode.elevation) knockbackDir.z = 0;
				knockbackDir.Normalize();
				PlayerController.Instance.DoDamage(knockbackDir);
			}
		}
	}

	abstract protected void InitEnemy();

	abstract protected void UpdateMovement();

	protected Vector3 GetMovementDir()
	{
		Vector3 moveDir = (_target.transform.position - transform.position);
		return moveDir;
	}
}
