using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

[RequireComponent(typeof(AIMovementController))]
public class BaseEnemy : MonoBehaviour, IDamageable
{
    // Declare the finite state machine
    private StateMachine fsm;

    // Parameters (can be changed in the inspector)
    public float searchSpotRange = 10;
    public float attackRange = 3;

    public float searchTime = 20;  // in seconds

    public float patrolSpeed = 2;
    public float chaseSpeed = 4;
    public float attackDuration = 2.5f;

    // Internal fields
    private AIMovementController movementController;
    private AIAnimationController animator;

    private Vector3 playerPosition => Player.Instance.transform.position;
    private float distanceToPlayer => Vector2.Distance(playerPosition, transform.position);

    public event Action OnHit;

    public void ApplyDamage(SwordHitType hitType)
    {
        if (hitType == SwordHitType.Complete && GameManager.Instance.GameType == GameType.Sword)
        {
            OnHit?.Invoke();
        }
    }

    void Start()
    {
        movementController = GetComponent<AIMovementController>();
        animator = GetComponentInChildren<AIAnimationController>();

        fsm = new StateMachine();

        // Fight FSM
        var fightFsm = new HybridStateMachine(
            afterOnLogic: state => movementController.StopMovement(),
            beforeOnLogic: state => MoveTowards(playerPosition, chaseSpeed),
            needsExitTime: true
        );

        fightFsm.AddState("Wait", onEnter: state => animator.SetAnimatorParameter(AnimatorParameterType.Bool, "move", false));
        fightFsm.AddState("Telegraph", onEnter: state => animator.SetAnimatorParameter(AnimatorParameterType.Trigger, "attack1"));

        // Because the exit transition should have the highest precedence,
        // it is added before the other transitions.
        fightFsm.AddExitTransition("Wait");

        fightFsm.AddTransition(new TransitionAfter("Wait", "Telegraph", 0.5f));
        fightFsm.AddTransition(new TransitionAfter("Telegraph", "Wait", attackDuration));

        // Root FSM
        fsm.AddState("Chase", new State(
            onExit: state => movementController.StopMovement(),
            onLogic: state => MoveTowards(playerPosition, chaseSpeed)
        ));
        fsm.AddState("Fight", fightFsm);

        fsm.AddTwoWayTransition("Chase", "Fight", t => distanceToPlayer <= attackRange);

        fsm.Init();
    }

    void Update()
    {
        fsm.OnLogic();
    }



    private void MoveTowards(Vector3 target, float speed)
    {
        movementController.MoveToPositionWithNavMesh(target, speed);
    }



}

