using Cinemachine;
using System;
using System.Collections;
using Unity.VisualScripting;
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

    public static CameraMode Instance;

    [SerializeField] private float switchCD = 5f;
    public bool CanSwitch { get { return _canSwitch; } set { _canSwitch = value; if (!_canSwitch) StartCoroutine(ModeSwitchCD()); } }
    [SerializeField] private bool _canSwitch = true;
    #endregion

    void Awake()
    {
        CurrentCamMode = ViewMode.plan; 
        Instance = this;
	}
    

    void Update()
    {
	}

	public void SwitchCameraMode()
	{
        if (CurrentCamMode != ViewMode.plan)
        {
            CurrentCamMode = ViewMode.plan;
            OnCameraModeChanged?.Invoke();
            _planViewVCam.Priority = 10;
            _elevationViewVCam.Priority = 1;
        }
        else if (CurrentCamMode != ViewMode.elevation)
        {
            CurrentCamMode = ViewMode.elevation;
            OnCameraModeChanged?.Invoke();
            _planViewVCam.Priority = 1;
            _elevationViewVCam.Priority = 10;
        }
    }

    private IEnumerator ModeSwitchCD()
    {
        yield return new WaitForSeconds(switchCD);
        CanSwitch = true;
    }
}
