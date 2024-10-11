using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public TextMeshPro playerScoreTMP;
    public TextMeshPro aiScoreTMP;

    private void Awake() {
        playerScoreTMP.SetText("0");
        aiScoreTMP.SetText("0");
    }

    private void Start() {
        GameManager.Instance.OnRoundEndEvent += OnRoundEnd;
        GameManager.Instance.OnGameStartEvent += OnRoundEnd;
    }

    private void OnRoundEnd()
    {
        playerScoreTMP.SetText(GameManager.Instance.HumanScore.ToString());
        aiScoreTMP.SetText(GameManager.Instance.AiScore.ToString());
    }
}
