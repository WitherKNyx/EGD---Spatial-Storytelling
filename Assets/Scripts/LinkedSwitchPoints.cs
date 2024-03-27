using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedSwitchPoints : MonoBehaviour
{
    [SerializeField] private ModeSwitchPoint planSwitchPoint;
    [SerializeField] private ModeSwitchPoint elevationSwitchPoint;

    private void Start()
    {
        if(planSwitchPoint != null)
        {
            planSwitchPoint.LinkPoint(this);
        }
        if(elevationSwitchPoint != null)
        {
            elevationSwitchPoint.LinkPoint(this);
        }
    }

    public Vector3 GetSwitchPointToTeleport()
    {
        if(CameraMode.CurrentCamMode == ViewMode.elevation)
        {
            return elevationSwitchPoint.teleportPos.position;
        }
        else
        {
            return planSwitchPoint.teleportPos.position;
        }
    }
}
