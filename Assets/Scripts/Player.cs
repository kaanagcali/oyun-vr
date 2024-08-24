using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public event Action OnHit;

    public void ApplyDamage(SwordHitType hitType)
    {
        if (hitType == SwordHitType.Complete && GameManager.Instance.GameType == GameType.Sword)
        {
            OnHit?.Invoke();
        }
    }
}
