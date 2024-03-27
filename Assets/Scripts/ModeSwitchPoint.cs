using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class ModeSwitchPoint : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private GameObject SwitchUIPrompt;
    [SerializeField] private LinkedSwitchPoints linkedSwitchPoint;

    public Transform teleportPos;

    #region Gizmo
    [SerializeField] private Vector3 boxSize = Vector3.one;
    [SerializeField] private Color teleportPosGizmoColor = Color.white;
    #endregion

    public UnityEvent OnModeSwitch;
    public static event Action<Vector3> OnModeSwitched;
    public static event Action OnSwitchAreaExited;

    private void Start()
    {
        if(interactable == null)
        {
            interactable = GetComponent<Interactable>();
        }
    }

    public void LinkPoint(LinkedSwitchPoints linkedSwitchPoint)
    {
        this.linkedSwitchPoint = linkedSwitchPoint;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        if (interactable.InteractState == InteractableState.Activated)
        {
            EnableSwitchUIPrompt();
        }
        
    }

    public void SwitchMode()
    {
        // check if the request for switching modes can be made
        if (CameraMode.Instance.CanSwitch)
        {
            interactable.InteractionVerified(true);

            // TEMPORARY: insert call to procedure of events to switch the camera mode here
            // just remember to invoke OnModeSwitch and OnModeSwitched somewhere when the camera is being switched so the player's new position
            // can be sent to the PlayerBrain when switching player controllers

            CameraMode.Instance.CanSwitch = false;
            CameraMode.Instance.SwitchCameraMode();
            OnModeSwitch.Invoke();
            OnModeSwitched.Invoke(linkedSwitchPoint.GetSwitchPointToTeleport());

        }
        // request denied, no changes
        else { interactable.InteractionVerified(false); }
    }

    public void EnableSwitchUIPrompt()
    {
        // check if the camera mode can currently be switched
        if(CameraMode.Instance.CanSwitch)
        {
            // set the UI to active if it isn't already, otherwise do nothing
            if (SwitchUIPrompt != null && !SwitchUIPrompt.activeSelf) { SwitchUIPrompt.SetActive(true); Debug.Log("displaying UI"); }
        }
        
    }

    public void DisableSwitchUIPrompt()
    {
        // set the UI to inactive if it is currently active, otherwise do nothing
        if ((SwitchUIPrompt != null && SwitchUIPrompt.activeSelf)) { SwitchUIPrompt.SetActive(false); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = teleportPosGizmoColor;
        Gizmos.DrawCube(teleportPos.position, boxSize);
    }
}
