using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CameraMode : MonoBehaviour
{
	public enum CamMode { plan, elevation, mixed };

	public static CamMode CurrentCamMode;

    public static event Action OnCameraModeChanged;

    #region References
    [SerializeField] 
    private Camera _camera;

    [SerializeField]
    private CinemachineVirtualCamera planViewVCam;
    [SerializeField]
    private CinemachineVirtualCamera elevationViewVCam;
    #endregion

    void Awake()
    {
        CurrentCamMode = CamMode.plan; 
	}
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CurrentCamMode != CamMode.plan)
        {
			CurrentCamMode = CamMode.plan;
            OnCameraModeChanged?.Invoke();
            planViewVCam.Priority = 1;
            elevationViewVCam.Priority = 0;
		} else if (Input.GetMouseButtonDown(1) && CurrentCamMode != CamMode.elevation)
        {
            CurrentCamMode = CamMode.elevation;
            OnCameraModeChanged?.Invoke();
            planViewVCam.Priority = 0;
            elevationViewVCam.Priority = 1;
        }
    }
}
