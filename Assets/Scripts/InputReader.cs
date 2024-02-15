using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.SceneView;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    private PlayerInputActions playerActions;
    private InputAction move;
    public Vector2 moveInput;

    private void Awake()
    {
        playerActions = new PlayerInputActions();

        move = playerActions.Player.Move;
    }

    private void OnEnable()
    {
        move.Enable();

        move.performed += Move;
        move.canceled += Move;
    }

    private void OnDisable()
    {
        move.Disable(); 
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

}
