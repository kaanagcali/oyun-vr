using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameReadyCounter : MonoBehaviour
{
    protected TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.transform.localScale = Vector3.zero;
        GameManager.Instance.OnRoundCounterChangedEvent += OnRoundCounterChangedEvent;
    }

    private void OnRoundCounterChangedEvent(int obj)
    {
        Sequence seq = DOTween.Sequence();
        textMeshPro.transform.localScale = Vector3.zero;
        seq.Append(textMeshPro.transform.DOScale(1f, 0.7f).SetEase(Ease.InOutBack));
        textMeshPro.SetText(obj.ToString());
        if (obj == 0)
        {
            seq.OnComplete(() => 
            {
                Sequence anotherSeq = DOTween.Sequence();
                textMeshPro.transform.localScale = Vector3.zero;
                anotherSeq.Append(textMeshPro.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutBack));
                anotherSeq.Append(textMeshPro.transform.DOScale(0f, 0.3f).SetEase(Ease.InOutBack));
                textMeshPro.SetText("Round " + GameManager.Instance.CurrentRound + " Go!");
            });
            seq.Complete();
        }
        /* else if (obj == 3)
        {
            textMeshPro.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBack);
        } */
    }
}
