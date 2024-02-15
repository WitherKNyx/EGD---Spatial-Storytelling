using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public void MixedRotate()
    {
        if(CameraMode.CurrentCamMode == CameraMode.CamMode.mixed)
        {
            transform.Rotate(90f, 0f, 0f);
        }
    }
}
