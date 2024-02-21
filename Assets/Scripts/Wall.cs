using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private Transform frontPos;
    [SerializeField] private Transform backPos;

    [SerializeField] private BoxCollider m_collider;

    private void Start()
    {
        TogglePlanWalls();
    }

    private void OnEnable()
    {
        CameraMode.OnCameraModeChanged += TogglePlanWalls;
    }

    private void OnDisable()
    {
        CameraMode.OnCameraModeChanged -= TogglePlanWalls;
    }

    public void TogglePlanWalls()
    {
        m_collider.enabled = (CameraMode.CurrentCamMode == ViewMode.plan);
    }
}
