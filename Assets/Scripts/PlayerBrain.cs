using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private PlayerController planController;
    [SerializeField] private PlayerController elevationController;

    [SerializeField] private ControllerState currentActiveController = ControllerState.Topdown;

    private void Start()
    {
        currentActiveController = (CameraMode.CurrentCamMode == ViewMode.plan || CameraMode.CurrentCamMode == ViewMode.mixed) ? ControllerState.Topdown : ControllerState.Sidescroller;
    }

    private void OnEnable()
    {
        //CameraMode.OnCameraModeChanged += SwitchPlayerController;

        ModeSwitchPoint.OnModeSwitched += SwitchPlayerController;
    }

    private void OnDisable()
    {
        //CameraMode.OnCameraModeChanged -= SwitchPlayerController;

        ModeSwitchPoint.OnModeSwitched -= SwitchPlayerController;
    }

    private void SwitchPlayerController(Vector3 switchPosition)
    {
        if(CameraMode.CurrentCamMode == ViewMode.plan || CameraMode.CurrentCamMode == ViewMode.mixed)
        {
            planController.transform.position = switchPosition;
            currentActiveController = ControllerState.Topdown;
            planController.enabled = true;
            elevationController.enabled = false;
        }
        else if (CameraMode.CurrentCamMode == ViewMode.elevation)
        {
            elevationController.transform.position = switchPosition;
            currentActiveController = ControllerState.Sidescroller;
            planController.enabled = false;
            elevationController.enabled = true;
        }
        
    }
}
