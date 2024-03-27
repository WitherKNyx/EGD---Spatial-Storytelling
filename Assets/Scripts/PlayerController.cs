using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region References
    public static PlayerController Instance;
    [SerializeField] private InputReader input;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Animator planAnimator;
    [SerializeField] private Animator elevationAnimator;
    [SerializeField] private SpriteRenderer elevationSprite;
    #endregion

    #region Movement Variables
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float speedChangeRate = 10f;
    [SerializeField] private float splineChangeRate = 0.05f;
    [SerializeField] private float rotationSpeed = 40f;
    [SerializeField] private Vector3 moveDirection;
    
    [SerializeField] private float previousZPos = 0.0f;
    [SerializeField] private float previousYRot = 0.0f;
    #endregion

    #region State
    [Tooltip("The current controller active, set to Topdown by default")]
    [SerializeField] private ControllerState controlState = ControllerState.Topdown;
    public PlayerState CurrentPlayerState { get { return _playerState; } private set { _playerState = value; OnPlayerStateChanged(); } }
    [SerializeField] private PlayerState _playerState;

    [SerializeField] private Color damagedColor = Color.red;
    private int _health = 3;
    #endregion

    #region private variables
    private float _speed;
    private CharacterController _controller;
    #endregion

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Instance = this;
        if(controlState == ControllerState.Topdown)
        {
            planAnimator.enabled = true;
            elevationAnimator.enabled = false;
        }
        else
        {
            planAnimator.enabled = false;
            elevationAnimator.enabled = true;
        }
    }

    private void OnEnable()
    {
        //CameraMode.OnCameraModeChanged += EnableController;
    }

    private void OnDisable()
    {
        //CameraMode.OnCameraModeChanged -= EnableController;
    }

    private void OnPlayerStateChanged()
    {
        if(controlState == ControllerState.Topdown)
        {
            planAnimator.SetInteger("PlayerState", (int)CurrentPlayerState);
        }
        else
        {
            elevationAnimator.SetInteger("PlayerState", (int)CurrentPlayerState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPaused) return;
        if(CurrentPlayerState != PlayerState.Damaged)
        {
            HandleMovement();
        }
    }

    public void DoDamage(Vector3 dir)
    {
        --_health;
        UIManager.Instance.HealthUI.SetHealth(_health);
		if (_health == 0)
        {
            GameManager.Instance.GameOver();
        } else
        {
			StartCoroutine(DoKnockback(dir));
		}
    }

    private IEnumerator DoKnockback(Vector3 dir)
    {
        CurrentPlayerState = PlayerState.Damaged;
        Vector3 currentPos = transform.position;
        Vector3 desiredPos = transform.position + 7.5f * dir;
        int steps = 20;
        for (int i = 0; i < steps + 1; ++i)
        {
            Vector3 movePos = Vector3.Lerp(currentPos, desiredPos, EaseInOutExp((float)i / steps));
            if (Physics.Raycast(
                    new Ray(transform.position, movePos), 
                    out RaycastHit info, 
                    Vector3.Distance(transform.position, movePos)
                )) {
                Debug.Log("HIT");
				transform.position = info.point;
                break;
            }
            transform.position = movePos;
			yield return new WaitForSeconds(1f / steps);
        }
		CurrentPlayerState = PlayerState.Idle;
	}

    private float EaseInOutExp(float t)
    {
        if (t == 0 || t == 1) return t;
        return (t < 0.5 
                ? Mathf.Pow(2, 20 * t - 10)
                : 2 - Mathf.Pow(2, -20 * t + 10)
            ) / 2;
    }

    private void HandleMovement()
    {
        if (GameManager.Instance._state != GameState.Playing) return;
        switch(controlState)
        {
            case ControllerState.Sidescroller:
                SidescrollerMovement();
                break;
            default:
                TopdownMovement();
                break;
        }
    }

    private void TopdownMovement()
    {
        moveInput = InputReader.moveInput;
        //calculate speed to move charactercontroller with
        float targetSpeed = moveSpeed;

        //if no input, set target speed to 0
        if (moveInput == Vector2.zero)
        { 
            targetSpeed = 0f;
            CurrentPlayerState = PlayerState.Idle;
        }
        else { CurrentPlayerState = PlayerState.Walking; }

        //accleration and deceleration
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = moveInput.magnitude;

        //accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            //curved result rather than linear, for more organic speed change
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        //normalize input direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

        if(moveInput.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDirection.normalized), Time.deltaTime*rotationSpeed);
        }

        Physics.SyncTransforms();
        // move the player controller

        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime));
    }

    private void SidescrollerMovement()
    {
        moveInput = InputReader.moveInput;
        //calculate speed to move charactercontroller with
        float targetSpeed = moveSpeed;

        //if no input, set target speed to 0
        if (moveInput == Vector2.zero)
        { 
            targetSpeed = 0f;
            CurrentPlayerState = PlayerState.Idle;
        }
        else { CurrentPlayerState = PlayerState.Walking; }

        //accleration and deceleration
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = moveInput.magnitude;

        //accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            //curved result rather than linear, for more organic speed change
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        //normalize input direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, 0.0f).normalized;

        /*
        if(moveInput.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDirection.normalized), Time.deltaTime*rotationSpeed);
        }*/

        Physics.SyncTransforms();
        // move the player controller

        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime));
        if(inputDirection.x < 0f)
        {
            elevationSprite.flipX = true;
        }
        else if(inputDirection.x > 0f)
        {
            elevationSprite.flipX = false;
        }
    }

    private void EnableController()
    {
        if (CameraMode.CurrentCamMode != ViewMode.elevation)
        {
            ReturnToPreviousZPos();
            controlState = ControllerState.Topdown;
            planAnimator.enabled = true;
            elevationAnimator.enabled = false;
        }
        else {
            previousZPos = transform.position.z;
            //if (splineInputType == SplineInputType.WSInput) { previousZPos = int.MaxValue; }
            transform.rotation = Quaternion.LookRotation(Vector3.zero);
            controlState = ControllerState.Sidescroller;
            planAnimator.enabled = false;
            elevationAnimator.enabled = true;
        }
    }

    private void ReturnToPreviousZPos()
    {
        if(previousZPos != int.MaxValue) {
            transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, previousZPos), Quaternion.Euler(0f, 0f, 0f));
        }
        else
        {

        }
    }

    private void OnDrawGizmos()
    {

    }
}

public enum PlayerState
{
    Idle,
    Walking,
    Damaged
}

public enum ControllerState
{
    Topdown,
    Sidescroller
}

