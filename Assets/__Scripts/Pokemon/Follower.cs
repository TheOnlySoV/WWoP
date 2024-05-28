using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Follower : MonoBehaviour
{
    public FollowerState currentState;
    [Range(0.1f, 5f)] public float delayBeforeChase = 3f;
    float chaseTimer = 0f;

    public Transform myTrainer;

    [SerializeField]
    private float distanceToTrainer;

    NavMeshAgent agent;
    Animator animator;

    PlayerController controllerLink;

    private int speed;

    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    private void Awake()
    {
        myTrainer = GameManager.instance.trainer.transform;
        chaseTimer = delayBeforeChase;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        controllerLink = myTrainer.GetComponent<PlayerController>();
    }

    private void Start()
    {
        speed = Animator.StringToHash("Speed");
    }

    private void Update()
    {
        distanceToTrainer = Vector3.Distance(myTrainer.position, transform.position);
        switch(currentState)
        {
            case FollowerState.CaughtUp:

                break;

            case FollowerState.Chasing:

                break;

            case FollowerState.Exploring:

                break;
        }

        if (controllerLink.sprintToggled)
            agent.speed = runSpeed;
        else 
            agent.speed = walkSpeed;


        if (distanceToTrainer > 3f)
            agent.SetDestination(myTrainer.position);

        animator.SetFloat(speed, agent.velocity.magnitude);
    }
}

public enum FollowerState { CaughtUp, Chasing, Exploring }