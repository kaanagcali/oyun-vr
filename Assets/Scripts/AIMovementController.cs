using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class AIMovementController : MonoBehaviour
{
    private float _defaultMovementSpeed = 0;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float arriveDist = 0.1f;
    public float remDist;
    [SerializeField] private AIAnimationController _aiAnimationController;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    private Vector3 _currentMovePosition;
    private Action _movementCompletedCallback;
    private Transform _rotateTransform;
    private Tween _rotateTween;
    private bool _isPaused;
    private bool _isMoving;
    private bool _hasPath;

    public bool IsMoving => _isMoving;
    public bool IsPaused => _isPaused;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    private bool _isTargetAlive;
    private Transform _followTarget;

    private void Awake()
    {
        if (!_navMeshAgent)
            _navMeshAgent = GetComponent<NavMeshAgent>();
        if (!_aiAnimationController)
            _aiAnimationController = GetComponent<AIAnimationController>();

    }


    private void Update()
    {
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = _isPaused;
            _aiAnimationController.SetAnimatorParameter(AnimatorParameterType.Bool, AnimatorParams.Move, boolValue: !_navMeshAgent.isStopped);
        }

        if (_isPaused) return;

        if (_currentMovePosition == Vector3.zero)
        {
            StopMovement();
            return;
        }

        _navMeshAgent.enabled = true;
        if (!_navMeshAgent.isOnNavMesh) return;
        if (_isTargetAlive) _currentMovePosition = _followTarget.position;
        _navMeshAgent.SetDestination(_currentMovePosition);
        _aiAnimationController.SetAnimatorParameter(AnimatorParameterType.Bool, AnimatorParams.Move, boolValue: true);

        if (!_isMoving)
        {
            _isMoving = true;

        }

        if (AtEndOfPath())
            CompleteMovement();
    }

    private bool AtEndOfPath()
    {
        _hasPath |= _navMeshAgent.hasPath;
        if (_hasPath && _navMeshAgent.remainingDistance <= arriveDist)
        {
            // Arrived
            _hasPath = false;
            return true;
        }
        return false;
    }

    public void StopMovement()
    {
        _rotateTween?.Kill(true);
        if (_navMeshAgent.isOnNavMesh && _navMeshAgent.enabled)
        {
            _navMeshAgent.SetDestination(transform.position);
            _navMeshAgent.enabled = false;
            _aiAnimationController.SetAnimatorParameter(AnimatorParameterType.Bool, AnimatorParams.Move, boolValue: false);
        }

        if (_isMoving)
        {
            _isMoving = false;
        }
    }

    public void StopMovementCreative()
    {
        _rotateTween?.Kill(true);
        if (_navMeshAgent.isOnNavMesh && _navMeshAgent.enabled)
        {
            _navMeshAgent.SetDestination(transform.position);
            _aiAnimationController.SetAnimatorParameter(AnimatorParameterType.Bool, AnimatorParams.Move, boolValue: false);
        }
        _isTargetAlive = false;
        _currentMovePosition = transform.position;
        if (_isMoving)
        {
            _isMoving = false;
        }
    }

    public void MoveToPositionWithNavMesh(Vector3 position, float speed = 0f, Transform rotateTransform = null, Action movementCompletedCallback = null)
    {
        _isTargetAlive = false;
        _currentMovePosition = position;
        // _currentMovePosition.y = transform.position.y;
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed <= 0f ? movementSpeed : speed;
        _rotateTransform = rotateTransform;
        _movementCompletedCallback = movementCompletedCallback;
        _isMoving = true;
    }

    public void MoveToPositionWithNavMesh(Transform targetTransform, float speed = 0f, Transform rotateTransform = null, Action movementCompletedCallback = null)
    {
        _followTarget = targetTransform;
        _isTargetAlive = true;
        _currentMovePosition = targetTransform.position;
        // _currentMovePosition.y = transform.position.y;
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed <= 0f ? movementSpeed : speed;
        _rotateTransform = rotateTransform;
        _movementCompletedCallback = movementCompletedCallback;
        _isMoving = true;
    }

    public void RotateToDirection(Vector3 dir, float duration, Action rotationCompletedCallback = null)
    {
        /* if (dir == Vector3.zero) return; */
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        lookRotation.x = 0f;
        lookRotation.z = 0f;
        _rotateTween = transform.DORotate(lookRotation.eulerAngles, duration).SetEase(Ease.Linear)
            .OnComplete(() => rotationCompletedCallback?.Invoke());
    }

    public void RotateToDirectionEuler(Vector3 eulerAngles, float duration, Action rotationCompletedCallback = null)
    {
        _rotateTween = transform.DORotate(eulerAngles, duration).SetEase(Ease.Linear)
            .OnComplete(() => rotationCompletedCallback?.Invoke());
    }


    public bool WarpPosition = true;

    public void CompleteMovement()
    {
        if (WarpPosition)
            _navMeshAgent.Warp(_currentMovePosition);
        if (_rotateTransform == null)
        {
            _isMoving = false;
            _movementCompletedCallback?.Invoke();
            _movementCompletedCallback = null;
            _currentMovePosition = Vector3.zero;
        }
        else
        {
            Vector3 loadPointPosition = _rotateTransform.position;
            loadPointPosition.y = 0;

            // Calculate the direction vector from the current position to the load point
            Vector3 direction = loadPointPosition - transform.position;

            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            RotateToDirection(direction, 0.1f, () =>
            {
                _movementCompletedCallback?.Invoke();
                _movementCompletedCallback = null;
            });
            _isMoving = false;
            _currentMovePosition = Vector3.zero;
        }
    }

    public void SetMovementSpeed(float s)
    {
        if (_defaultMovementSpeed == 0)
            _defaultMovementSpeed = _navMeshAgent ? _navMeshAgent.speed : 3f;
        movementSpeed = s;
        _navMeshAgent.speed = s;
        _aiAnimationController.SetAnimationSpeedMultiplier(s / _defaultMovementSpeed);
    }
}
