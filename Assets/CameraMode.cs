using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMode : MonoBehaviour
{
	public enum CamMode { plan, elevation, mixed };

	public static CamMode CurrentCamMode;

    #region References
    [SerializeField] 
    private Camera _camera;

    [SerializeField]
    private Transform _planTrans, _elevationTrans;
    #endregion

    void Awake()
    {
        CurrentCamMode = CamMode.plan; 
        _camera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        _camera.transform.SetParent(_planTrans, false);
	}
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			CurrentCamMode = CamMode.plan;
			_camera.transform.SetParent(_planTrans, false);
		} else if (Input.GetMouseButtonDown(1))
        {
            CurrentCamMode = CamMode.elevation;
			_camera.transform.SetParent(_elevationTrans, false);
		}
    }
}
