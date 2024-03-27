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
	[SerializeField]
	protected SpriteRenderer _sprite;

	protected Collider _col;

	protected Rigidbody _rb;

	[SerializeField] protected GameObject _target;

	[SerializeField] private GameObject returnLocation;

	#endregion

	private Vector3 _prevVelocity;

    #region Physics Raycast
    [SerializeField] private float damageRadius = 2f;
    [Tooltip("Layers that the Enemy should check for when inflicting")]
    [SerializeField] private LayerMask damageableLayers;
    [Tooltip("How triggers should be treated when detecting for damage, set to Ignore by default")]
    [SerializeField] private QueryTriggerInteraction damageTriggers = QueryTriggerInteraction.Ignore;

	[SerializeField] private float targetRadius = 8f;
    [Tooltip("Layers that the Enemy should check for when searching for targets")]
    [SerializeField] private LayerMask targetLayers;
    [Tooltip("How triggers should be treated when detecting for damage, set to Ignore by default")]
    [SerializeField] private QueryTriggerInteraction targetTriggers = QueryTriggerInteraction.Ignore;
    #endregion

    #region Gizmos
    [SerializeField] private Color damageRadiusColor = Color.green;
    [SerializeField] private Color targetRadiusColor = Color.cyan;
    #endregion

    #region State
    public EnemyState CurrentEnemyState { get { return _state; } private set { _state = value; } }
	[SerializeField] private EnemyState _state;

	[SerializeField] private float waitDuration = 2f;
    #endregion

    protected void Start()
	{
		//_sprite = GetComponent<SpriteRenderer>();
		_col = GetComponent<Collider>();
		_rb = GetComponent<Rigidbody>();
		//_target = GameObject.FindGameObjectWithTag("Player");
		InitEnemy();
	}

    protected void Update()
	{
		if (GameManager.Instance._state != GameState.Playing)
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
        if (GameManager.Instance._state != GameState.Playing) return;
		if (_enemyType == CameraMode.CurrentCamMode && FindTarget())
		{
			UpdateMovement();
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, damageRadius, Vector3.up, 1.0f, damageableLayers, damageTriggers);
			// use a sphere cast for the damage hitbox
			if (hits.Length > 0)
			{
				Debug.Log("hit something, checking if player");
                foreach (RaycastHit hit in hits)
				{
                    // check that the collider hit was a PlayerController
                    if (hit.collider.GetComponent<PlayerController>() != null)
                    {
                        PlayerController _player = hit.collider.GetComponent<PlayerController>();
                        // check that the player can be damaged
                        if (_player.CurrentPlayerState != PlayerState.Damaged)
                        {
                            Debug.Log("inflicting damage to player");
                            Vector3 knockbackDir = _player.transform.position - transform.position;
                            knockbackDir.y = 0;
                            if (_enemyType == ViewMode.elevation) knockbackDir.z = 0;
                            knockbackDir.Normalize();
                            _player.DoDamage(knockbackDir);
                        }
                    }
                }
				
			}
			else if(CurrentEnemyState == EnemyState.Chasing)
			{
				StartCoroutine(WaitForTarget());
			}
			Debug.Log("did not hit anything");
		}
		else if(_enemyType == CameraMode.CurrentCamMode && CurrentEnemyState == EnemyState.Returning)
		{
			Debug.Log("returning");
			_target = returnLocation;
			UpdateMovement();
			if(Vector3.Distance(returnLocation.transform.position, transform.position) < 0.05f)
			{
				CurrentEnemyState = EnemyState.Idle;
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

    protected bool FindTarget()
    {
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, targetRadius, Vector3.up, 1.0f, targetLayers, targetTriggers);
		if(hits.Length > 0)
		{
			bool targetFound = false;
			foreach(RaycastHit hit in hits)
			{
                // check that the collider hit was a PlayerController
                if (hit.collider.GetComponent<PlayerController>() != null)
                {
					targetFound = true;
                    PlayerController _player = hit.collider.GetComponent<PlayerController>();
                    // set the player controller as the target if target is currently null
					if(_target == null || CurrentEnemyState == EnemyState.Waiting || CurrentEnemyState == EnemyState.Returning)
					{
						_target = _player.gameObject;
					}
                }
            }
			if(targetFound) {
                StopCoroutine(WaitForTarget());
                CurrentEnemyState = EnemyState.Chasing; 
			}
			
			return targetFound;
		}
        _target = null;
        return false;
    }

	private IEnumerator WaitForTarget()
	{
		CurrentEnemyState = EnemyState.Waiting;
		yield return new WaitForSeconds(waitDuration);
		CurrentEnemyState = EnemyState.Returning;
	}

    protected void OnDrawGizmos()
    {
		Gizmos.color = damageRadiusColor;
		Gizmos.DrawSphere(transform.position, damageRadius);

        Gizmos.color = targetRadiusColor;
        Gizmos.DrawSphere(transform.position, targetRadius);
    }
}

public enum EnemyState
{
	Idle,
	Chasing,
	Waiting,
	Returning
}
