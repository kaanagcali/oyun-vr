using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    [SerializeField] private SwordHitType _hitType = SwordHitType.Complete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var player) && transform.root.gameObject != other.gameObject)
        {
            player.ApplyDamage(_hitType);
        }    
    }
}

public enum SwordHitType
{
    Complete,
    Point
}
