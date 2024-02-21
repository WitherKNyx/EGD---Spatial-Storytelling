using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Splines;

[RequireComponent(typeof(Interactable))]
public class SplineJunction : MonoBehaviour
{
    //[SerializeField] private SplineContainer[] connectedSplines = new SplineContainer[3];
    [SerializeField] private SplineContainer backwardSpline, sideSpline, forwardSpline; 

    [SerializeField] private SplineContainer currentSpline;

    public static event Action<SplineContainer> OnSplineJunctionChanged;

    public static SplineJunction Instance;

    private Interactable interactable;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpline = sideSpline;
        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {

    }

    public void SwitchSpline(KeyCode input)
    {
        SplineContainer nextSpline = currentSpline;
        if (currentSpline.GetComponent<CustomSplineComponent>().GetInputType() == SplineInputType.ADInput)
        {
            if(input == KeyCode.A || input == KeyCode.D)
            {
                interactable.InteractionVerified(false);
                return;
            }
        }
        else
        {
            if ((input == KeyCode.S && (backwardSpline == null || currentSpline == backwardSpline)) || (input == KeyCode.W && (forwardSpline == null || currentSpline == forwardSpline)))
            {
                interactable.InteractionVerified(false);
                return;
            }
        }
        nextSpline = (backwardSpline != null && input == KeyCode.S) ? backwardSpline : (forwardSpline != null && input == KeyCode.W) ? forwardSpline : sideSpline;
        if(currentSpline == nextSpline) { return; }
        interactable.InteractionVerified(true);
        currentSpline = nextSpline;
        OnSplineJunctionChanged?.Invoke(currentSpline);
    }
}
