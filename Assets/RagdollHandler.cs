using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    Animator animator;
    Collider[] colliders;
    List<Rigidbody> rigidbodies;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbodies = new List<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            var rb = collider.GetComponent<Rigidbody>();
            rigidbodies.Add(rb);


            if (collider.GetComponent<SwordHit>()) continue;
            rb.useGravity = false;
            rb.isKinematic = true;
            collider.enabled = false;
        }
    }

    public void OpenColliders()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            var rb = rigidbodies[i];
            var coll = colliders[i];

            if (coll.GetComponent<SwordHit>()) continue;
            rb.isKinematic = false;
            rb.useGravity = true;

            coll.enabled = true;
        }
        animator.enabled = false;
    }

    public void CloseColliders()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            var rb = rigidbodies[i];
            var coll = colliders[i];

            if (coll.GetComponent<SwordHit>()) continue;
            rb.isKinematic = true;
            rb.useGravity = false;

            coll.enabled = false;
        }
        animator.enabled = true;
    }

    public void ApplyForceBy(Vector3 hitPos)
{
    foreach (var rb in rigidbodies)
    {
        if (rb == null) continue; // Ensure the rigidbody exists
        if (rb.GetComponent<SwordHit>()) continue;

        // Calculate direction from hit position to the rigidbody's position
        Vector3 forceDirection = (rb.position - hitPos).normalized;

        // Calculate force magnitude (you can tweak the multiplier to control the intensity)
        float forceMagnitude = 10; // Replace with your desired force value

        // Apply force to the rigidbody
        rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
    }
}

}
