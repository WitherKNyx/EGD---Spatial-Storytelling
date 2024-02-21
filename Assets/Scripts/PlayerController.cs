using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region References
    public static PlayerController Instance;
    [SerializeField] private InputReader input;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private SplineContainer currentSpline;
    #endregion

    #region Movement Variables
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float speedChangeRate = 10f;
    [SerializeField] private float splineChangeRate = 0.05f;
    [SerializeField] private Vector3 moveDirection;
    
    [SerializeField] private float previousZPos = 0.0f;
    #endregion

    #region State
    [Tooltip("The current controller active, set to Topdown by default")]
    [SerializeField] private ControllerState controlState = ControllerState.Topdown;
    [SerializeField] private SplineInputType splineInputType = SplineInputType.ADInput;
    #endregion

    #region private variables
    private float _speed;
    private CharacterController _controller;
    [SerializeField] private float splineLength;
    [SerializeField] private float splineDistanceRatio = 0.0f;
    [SerializeField] private bool active = true;
    #endregion

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if(currentSpline == null)
        {
            currentSpline = GetComponent<SplineContainer>();
        }
        splineLength = currentSpline.CalculateLength();
        splineDistanceRatio = (transform.position.x - currentSpline.transform.position.x) / splineLength;
    }

    private void OnEnable()
    {
        CameraMode.OnCameraModeChanged += EnableController;
        SplineJunction.OnSplineJunctionChanged += SetActiveSpline;
    }

    private void OnDisable()
    {
        CameraMode.OnCameraModeChanged += EnableController;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { SplineJunction.Instance.SwitchSpline(KeyCode.Space); }
        HandleMovement();
    }

    private void HandleMovement()
    {
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
        if (moveInput == Vector2.zero) { targetSpeed = 0f; }

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

        Physics.SyncTransforms();
        // move the player controller
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime));
    }

    private void SidescrollerMovement()
    {
        moveInput = InputReader.moveInput;

        float inputValue = (splineInputType == SplineInputType.ADInput) ? moveInput.x : moveInput.y;
        //calculate speed to move charactercontroller with
        float targetSpeed = moveSpeed;

        //if no input, set target speed to 0
        if (moveInput == Vector2.zero) { targetSpeed = 0f; }

        //accleration and deceleration
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = Mathf.Abs(inputValue);

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
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition;
        if (inputMagnitude > 0.0f)
        {
            currentPosition = transform.position;
            if (inputValue > 0.0f)
            {
                splineDistanceRatio = Mathf.Clamp(splineDistanceRatio + _speed * Time.deltaTime / splineLength, 0f, 1f);
                nextPosition = currentSpline.EvaluatePosition(splineDistanceRatio + ((inputMagnitude > 0.0f) ? splineChangeRate : 0f));
                nextPosition = new Vector3(nextPosition.x, transform.position.y, nextPosition.z);
            }
            else if (inputValue < 0.0f)
            {
                splineDistanceRatio = Mathf.Clamp(splineDistanceRatio - _speed * Time.deltaTime / splineLength, 0f, 1f);
                nextPosition = currentSpline.EvaluatePosition(splineDistanceRatio - ((inputMagnitude > 0.0f) ? splineChangeRate : 0f));
                nextPosition = new Vector3(nextPosition.x, transform.position.y, nextPosition.z);
            }
        }

        //normalize movement direction along the spline
        moveDirection = /*(splineDistanceRatio == 0f || splineDistanceRatio == 1f)? Vector3.zero :*/ (nextPosition - currentPosition).normalized;
        //Debug.Log("move Direction: " + moveDirection);

        Debug.DrawRay(currentPosition, moveDirection * 5f, Color.red);

        Physics.SyncTransforms();
        // move the player controller
        _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime));
    }

    private void EnableController()
    {
        if (CameraMode.CurrentCamMode != ViewMode.elevation)
        {
            ReturnToPreviousZPos();
            controlState = ControllerState.Topdown;
        }
        else {
            previousZPos = transform.position.z;
            controlState = ControllerState.Sidescroller;
            
        }
    }

    public SplineContainer GetCurrentSpline()
    {
        return currentSpline;
    }

    public void SetActiveSpline(SplineContainer spline)
    {
        Debug.Log("Changing splines");
        currentSpline = spline;
        splineLength = currentSpline.CalculateLength();
        splineInputType = spline.GetComponent<CustomSplineComponent>().GetInputType();

        splineDistanceRatio = (transform.position.x - currentSpline.transform.position.x) / splineLength;
        

        
    }

    private void SnapZToXYLine()
    {
        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, currentSpline.transform.position.z), Quaternion.identity);
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

public enum SplineInputType
{
    ADInput,
    WSInput
}
