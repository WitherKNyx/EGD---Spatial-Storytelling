using Cinemachine;
using System;
using UnityEngine;

public enum ViewMode { plan, elevation, mixed };

[RequireComponent(typeof(Camera))]
public class CameraMode : MonoBehaviour
{
	public static ViewMode CurrentCamMode;

	public static event Action OnCameraModeChanged;

	#region References
	[SerializeField] 
    private Camera _camera;

    [SerializeField]
    private CinemachineVirtualCamera _planViewVCam, _elevationViewVCam;
    #endregion

    void Awake()
    {
        CurrentCamMode = ViewMode.plan; 
	}
    

    void Update()
    {
		if (Input.GetMouseButtonDown(0) && CurrentCamMode != ViewMode.plan)
		{
			CurrentCamMode = ViewMode.plan;
			OnCameraModeChanged?.Invoke();
			_planViewVCam.Priority = 10;
			_elevationViewVCam.Priority = 1;
		}
		else if (Input.GetMouseButtonDown(1) && CurrentCamMode != ViewMode.elevation)
		{
			CurrentCamMode = ViewMode.elevation;
			OnCameraModeChanged?.Invoke();
			_planViewVCam.Priority = 1;
			_elevationViewVCam.Priority = 10;
		}
	}
}
