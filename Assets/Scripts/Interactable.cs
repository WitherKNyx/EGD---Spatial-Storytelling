using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using Unity.Properties;
using UnityEngine.InputSystem.Controls;

public class Interactable: MonoBehaviour
{
    #region Physics Raycast
    [SerializeField] private Transform interactCenter;
    [SerializeField] private float interactRadius = 2f;
    [Tooltip("Layers that the Interactable should check for")]
    [SerializeField] private LayerMask interactDetectLayers;
    [Tooltip("How triggers should be treated when detecting for interaction, set to Ignore by default")]
    [SerializeField] private QueryTriggerInteraction interactionTriggers = QueryTriggerInteraction.Ignore;
    #endregion

    #region Gizmo
    [SerializeField] private Color radiusGizmoColor;
    [SerializeField] private Color idleRadiusColor = Color.green;
    [SerializeField] private Color interactingRadiusColor = Color.cyan;
    [SerializeField] private Color activeRadiusColor = Color.blue;
    #endregion

    #region State
    [SerializeField] InteractableState InteractState { get { return _state; } set { _state = value; } }
    [SerializeField] private InteractableState _state = InteractableState.Idle;
    [SerializeField] private List<GameObject> detectedObjects;
    #endregion

    #region Interactable General Settings
    [Tooltip("The view mode in which the interactable will start checking for events")]
    [SerializeField] private ViewMode activeViewMode = ViewMode.elevation;
    [SerializeField] private float interactDelay = 0.5f;
    [Tooltip("A list of player inputs that can be used to trigger interaction state")]
    [SerializeField] private List<KeyCode> interactInputs;
    #endregion

    #region Events
    public UnityEvent OnActivated;
    public UnityEvent<KeyCode> OnInteractionInputDetected;
    public UnityEvent OnInteractionStart;
    public UnityEvent OnInteractionEnd;
    public UnityEvent OnDeactivated;
    #endregion

    private void OnEnable()
    {
        CameraMode.OnCameraModeChanged += ChangeInteractStateWithCameraMode;
    }

    private void ChangeInteractStateWithCameraMode()
    {
        if (activeViewMode == ViewMode.mixed) { return; }
        else if (CameraMode.CurrentCamMode == ViewMode.plan) { InteractState = InteractableState.Inactive; }
        else { InteractState = InteractableState.Idle; }
    }

    private void Start()
    {
        if (!interactCenter) interactCenter = transform;
        ChangeInteractStateWithCameraMode();
    }

    private void Update()
    {
        switch (InteractState)
        {
            case InteractableState.Idle:
                break;
            case InteractableState.Activated:
                CheckForInteraction();
                break;
            case InteractableState.Interacting:
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if(InteractState != InteractableState.Inactive && InteractState != InteractableState.Interacting)
        {
            CheckForActivation();
        }
    }

    private void CheckForActivation()
    {
        Collider[] colliders = Physics.OverlapSphere(interactCenter.position, interactRadius, interactDetectLayers, interactionTriggers);
        if (colliders.Length > 0)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                if (!detectedObjects.Contains(colliders[i].gameObject)) detectedObjects.Add(colliders[i].gameObject);
            }
            if(InteractState == InteractableState.Idle)
            {
                InteractState = InteractableState.Activated;
                OnActivated?.Invoke();
            }
        }
        else
        {
            ClearInteractedObjects();
            OnDeactivated?.Invoke();
        }
    }

    private void CheckForInteraction()
    {
        if(Input.anyKeyDown)
        {
            foreach(KeyCode input in interactInputs)
            {
                if (Input.GetKey(input)) {
                    Debug.Log("interaction detected, checking for success");
                    InteractState = InteractableState.Verifying;
                    OnInteractionInputDetected?.Invoke(input);
                    StartCoroutine(VerifyInteraction());
                    if(InteractState == InteractableState.Interacting)
                    {
                        StartCoroutine(ResetInteractions());
                    }
                    break;
                }
            }
            
        }
    }

    private IEnumerator VerifyInteraction()
    {
        yield return new WaitWhile(() => InteractState == InteractableState.Verifying);
    }

    private IEnumerator ResetInteractions()
    {
        yield return new WaitForSeconds(interactDelay);
        InteractState = InteractableState.Idle;
    }

    public void ClearInteractedObjects()
    {
        detectedObjects.Clear();
        InteractState = InteractableState.Idle;
    }

    public void InteractionVerified(bool verified)
    {
        Debug.Log("interaction success, invoking events");
        InteractState = verified ? InteractableState.Interacting : InteractableState.Idle;
    }

    private void OnDrawGizmos()
    {
        if(_state == InteractableState.Inactive) { return; }
        switch (_state)
        {
            case InteractableState.Activated:
                radiusGizmoColor = activeRadiusColor;
                break;
            case InteractableState.Interacting:
                radiusGizmoColor = interactingRadiusColor;
                break;
            default:
                radiusGizmoColor = idleRadiusColor; 
                break;
        }
        radiusGizmoColor.a = 0.5f;
        Gizmos.color = radiusGizmoColor;
        Gizmos.DrawSphere(interactCenter.position, interactRadius);
        
    }

}

public enum InteractType
{
    PlayerInput,
    Collision
}

public enum InteractableState
{
    Idle,
    Activated,
    Verifying,
    Interacting,
    Inactive
}

