using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Collider), typeof(Rigidbody))]
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
			UpdateMovement();
	}

	abstract protected void InitEnemy();

	abstract protected void UpdateMovement();

	protected Vector3 GetMovementDir()
	{
		Vector3 moveDir = (_target.transform.position - transform.position);
		return moveDir;
	}
}
