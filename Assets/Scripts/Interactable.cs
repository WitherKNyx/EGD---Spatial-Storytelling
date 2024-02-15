using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Transform interactCenter;
    [SerializeField] private float interactRadius = 2f;
    [Tooltip("Layers that the Interactable should check for")]
    [SerializeField] private LayerMask interactDetectLayers;

    [SerializeField] private Color radiusGizmoColor;
    [SerializeField] private Color idleRadiusColor = Color.green;
    [SerializeField] private Color interactingRadiusColor = Color.cyan;
    [SerializeField] private Color activeRadiusColor = Color.blue;

    [Tooltip("How triggers should be treated when detecting for interaction, set to Ignore by default")]
    [SerializeField] private QueryTriggerInteraction interactionTriggers = QueryTriggerInteraction.Ignore;

    [SerializeField] private InteractableState state {  get; set; }

    [SerializeField] private List<GameObject> detectedObjects;

    public UnityEvent OnInteractRadiusStay;
    public UnityEvent OnInteractionStart;
    public UnityEvent OnInteractionEnd;
    public UnityEvent OnInteractRadiusExited;

    private void Start()
    {
        if (!interactCenter) interactCenter = transform;
    }

    private void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        RaycastHit hit;
        if(Physics.SphereCast(interactCenter.position, interactRadius, Vector3.up, out hit, 0.1f,  interactDetectLayers, interactionTriggers))
        {
            
        }
        else
        {

        }
    }

    private void OnDrawGizmos()
    {
        if(state == InteractableState.Inactive) { return; }
        switch (state)
        {
            case InteractableState.Active:
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

public enum InteractableState
{
    Idle,
    Active,
    Interacting,
    Inactive
}
