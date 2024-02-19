using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private Vector2 moveInput;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float speedChangeRate = 10f;

    [SerializeField] private bool active = true;

    [Header("References")]
    [SerializeField] private float previousZPos = 0.0f;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private bool canMoveSidescrollZ = false;

    [Tooltip("The current controller active, set to Topdown by default")]
    [SerializeField] private ControllerState controlState = ControllerState.Topdown;

    public static PlayerController Instance;

    //player
    private float _speed;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CameraMode.OnCameraModeChanged += EnableController;
    }

    private void OnDisable()
    {
        CameraMode.OnCameraModeChanged += EnableController;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        moveInput = input.moveInput;
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
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, (controlState == ControllerState.Topdown || canMoveSidescrollZ) ? moveInput.y : 0.0f).normalized;
        if(controlState == ControllerState.Sidescroller && !canMoveSidescrollZ)
        {
            SnapZToXYLine();
        }

        Physics.SyncTransforms();
        // move the player controller
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime));
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

    private void SnapZToXYLine()
    {
        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, _line.transform.position.z), Quaternion.Euler(_line.transform.eulerAngles.x, _line.transform.eulerAngles.y-90f, _line.transform.eulerAngles.z));
    }

    private void ReturnToPreviousZPos()
    {
        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, previousZPos), Quaternion.Euler(0f,0f,0f));
    }
}

public enum ControllerState
{
    Topdown,
    Sidescroller
}
