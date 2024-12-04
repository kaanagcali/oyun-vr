using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    [SerializeField] private SwordHitType _hitType = SwordHitType.Complete;
    public SwordHitType HitType => _hitType;

    public AudioClip[] swordHitClips;
    public AudioClip someoneDamagedClip;

    protected float cantDamageWhenHitSword = 0.5f;
    protected float lastHitSwordTime;

    public GameObject hitPF;

    public Transform fakeRoot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var player) && transform.root.gameObject != other.gameObject)
        {
            if (fakeRoot && fakeRoot.gameObject == other.gameObject) return;
            
            if (other.gameObject.GetComponent<CharacterController>() != null)
            {
                if (transform.root.childCount == 5) return;
            }
            if (Time.time < lastHitSwordTime + cantDamageWhenHitSword)
            {
                return;
            }
            player.ApplyDamage(_hitType);
            if (someoneDamagedClip)
                AudioSource.PlayClipAtPoint(someoneDamagedClip, transform.position);
            Vector3 hitPosition = other.ClosestPoint(transform.position);
            if (hitPF)
                Instantiate(hitPF, hitPosition, Quaternion.identity);
        }
        else if (other.TryGetComponent<SwordHit>(out var hit))
        {
            if (hit.HitType == _hitType)
            {
                lastHitSwordTime = Time.time;
                if (swordHitClips.Length > 0)
                    AudioSource.PlayClipAtPoint(swordHitClips[Random.Range(0, swordHitClips.Length)], transform.position, 0.3f);
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                if (hitPF)
                    Instantiate(hitPF, hitPosition, Quaternion.identity);
            }
        }
    }
}

public enum SwordHitType
{
    Complete,
    Point
}
