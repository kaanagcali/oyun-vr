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

    public float attackSpeedMultiplier = 1f;

    private Vector3 playerPosition => Player.Instance.transform.position;
    private float distanceToPlayer => Vector2.Distance(playerPosition, transform.position);

    public event Action OnHit;

    private RagdollHandler ragdollHandler;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        ragdollHandler = GetComponentInChildren<RagdollHandler>();
    }

    private Vector3 hitPos;

    float lastHitTime;
    Collider coll;

    public void AssignHitPosition(Vector3 hitPos)
    {
        this.hitPos = hitPos;
    }

    bool isHit = false;

    public void ApplyDamage(SwordHitType hitType)
    {
        if (hitType == SwordHitType.Complete && GameManager.Instance.GameType == GameType.Sword)
        {
            OnHit?.Invoke();
            if (Time.time >= lastHitTime + 5)
            {
                coll.enabled = false;
                movementController.NavMeshAgent.enabled = false;
                isHit = true;
                ragdollHandler.OpenColliders();
                ragdollHandler.ApplyForceBy(hitPos);
                lastHitTime = Time.time;
            }
        }
    }

    private void OnEnable()
    {
        if (!movementController) movementController = GetComponent<AIMovementController>();
        coll.enabled = true;
        movementController.NavMeshAgent.enabled = true;
        ragdollHandler.CloseColliders();
        isHit = false;
    }

    public float[] attackDurations;

    int lastRandom = -1;

    void Start()
    {
        movementController = GetComponent<AIMovementController>();
        animator = GetComponentInChildren<AIAnimationController>();

        animator.SetAnimatorParameter(AnimatorParameterType.Float, "attackSpeed", false, attackSpeedMultiplier);

        for (int i = 0; i < attackDurations.Length; i++)
        {
            attackDurations[i] = attackDurations[i] / attackSpeedMultiplier;
        }

        fsm = new StateMachine();

        // Fight FSM
        var fightFsm = new HybridStateMachine(
            afterOnLogic: state => movementController.StopMovement(),
            beforeOnLogic: state => MoveTowards(playerPosition, chaseSpeed),
            needsExitTime: true
        );

        fightFsm.AddState("Wait", onEnter: state => animator.SetAnimatorParameter(AnimatorParameterType.Bool, "move", false));

        for (int i = 0; i < attackDurations.Length; i++)
        {
            int rand = i;
            fightFsm.AddState("Telegraph" + rand.ToString(), onEnter: state => animator.SetAnimatorParameter(AnimatorParameterType.Trigger, "attack" + (rand + 1).ToString()));
            fightFsm.AddTransition(new TransitionAfter("Telegraph" + i.ToString(), "Wait", attackDurations[rand]));
        }

        fightFsm.AddState("Telegraph", onEnter: state =>
        {
            int random = UnityEngine.Random.Range(0, attackDurations.Length);
            while (random == lastRandom)
            {
                random = UnityEngine.Random.Range(0, attackDurations.Length);
            }
            lastRandom = random;
            Debug.Log("telegraph " + random);
            fightFsm.RequestStateChange("Telegraph" + random);
        });

        // Because the exit transition should have the highest precedence,
        // it is added before the other transitions.
        fightFsm.AddExitTransition("Wait");

        fightFsm.AddTransition(new TransitionAfter("Wait", "Telegraph", 0.1f));

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
        if (!isHit)
            transform.rotation = Quaternion.LookRotation((playerPosition - transform.position).normalized);
    }



    private void MoveTowards(Vector3 target, float speed)
    {
        movementController.MoveToPositionWithNavMesh(target, speed);
    }



}

