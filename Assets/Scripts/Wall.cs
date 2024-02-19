using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private Transform frontPos;
    [SerializeField] private Transform backPos;

    public void MixedRotate()
    {
        if(CameraMode.CurrentCamMode == ViewMode.mixed)
        {
            transform.Rotate(90f, 0f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
    }
}
