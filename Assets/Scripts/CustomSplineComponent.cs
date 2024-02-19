using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSplineComponent : MonoBehaviour
{
    [SerializeField] private SplineInputType inputType;

    public SplineInputType GetInputType() { return inputType; }
}
