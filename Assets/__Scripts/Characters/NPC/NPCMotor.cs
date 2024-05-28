using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMotor : MonoBehaviour
{
    [Header("Motor Settings")]
    public NPCMovementState movementState = NPCMovementState.Idle;
    [HideInInspector]
    public NavMeshAgent nma;
    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public int _animIDSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");
        nma = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

    }

    public void GoToNewLocation(Vector3 newLocation)
    {
        movementState = NPCMovementState.Moving;
        nma.isStopped = false;
        nma.SetDestination(newLocation);
    }

    public void GoToNewLocation()
    {
        movementState = NPCMovementState.Moving;
        //nma.SetDestination(newLocation);
    }
}

public enum NPCMovementState { Idle, Moving, Crouching, InConversation, Broken }
