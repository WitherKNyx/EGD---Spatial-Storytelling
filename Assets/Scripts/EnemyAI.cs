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
		_sprite.enabled = _col.enabled = (_enemyType == CameraMode.CurrentCamMode);
	}

	protected void FixedUpdate()
	{
		if (_enemyType == CameraMode.CurrentCamMode)
			UpdateMovement();
	}

	abstract protected void InitEnemy();

	abstract protected void UpdateMovement();
}
