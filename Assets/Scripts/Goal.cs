using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class Goal : MonoBehaviour
{
    [SerializeField] CapsuleCollider goalCollider;

    [Tooltip("Scene to load when the goal is collided with")]
    [SerializeField] private Scenes nextScene;

    [SerializeField] private GoalTrigger goalTrigger = GoalTrigger.Collision;

    public static event Action<Scenes> ActionOnGoalReached;
    public UnityEvent OnGoalReached;

    private void Start()
    {
        if(goalTrigger == GoalTrigger.Collision) GetComponent<Interactable>().enabled = false;
        else { goalCollider.enabled = false; }
    }

    public void InteractReachedGoal(KeyCode input)
    {
        Debug.Log("goal reached");
        GetComponent<Interactable>().InteractionVerified(true);
        GameManager.Instance.LoadSceneByEnum(nextScene);
        OnGoalReached?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("goal reached");
        GameManager.Instance.LoadSceneByEnum(nextScene);
        OnGoalReached?.Invoke();
    }
}

public enum GoalTrigger
{
    ButtonInteract,
    Collision
}
