using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionOfTooFar : MonoBehaviour
{
    Vector3 startPos;
    Vector3 startEuler;

    private void Start() {
        startPos = transform.position;
        startEuler = transform.eulerAngles;
    }

    bool reset;

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Ground"))
        {
            if (!reset)
            {
                reset = true;
                Invoke(nameof(ResetPosition), 5f);
            }
        }
    }

    private void LateUpdate() {
        if (transform.parent == null)
        {
            if (reset) return;
            if (Mathf.Abs(transform.position.y - startPos.y) > 3)
            {
                reset = true;
                Invoke(nameof(ResetPosition), 3f);
            }
        }
        else
        {
            reset = false;
        }
    }

    public void ResetPosition()
    {
        if (transform.parent == null)
        {
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                Invoke(nameof(ResetRb), 0.5f);
            }
            transform.position = startPos;
            transform.eulerAngles = startEuler;
            reset = false;
        }
    }

    private void ResetRb()
    {
        if (TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = false;
    }
}
