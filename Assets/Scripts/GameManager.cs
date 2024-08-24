using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    public GameType GameType { get; private set; }
    public GameDifficulty GameDifficulty { get; private set; }
    private int _currentRound;
    private int _maxRound;

    private Player _ai;
    private Player _human;

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        if (!_ai) return;

        _ai.OnHit += OnAiHit;
        _human.OnHit += OnHumanHit;
    }

    private void OnAiHit()
    {

    }

    private void OnHumanHit()
    {
        
    }

    public void ClearActiveGame()
    {
        _currentRound = 1;
    }

    public void StartGame(GameType gameType, GameDifficulty gameDifficulty, int roundCount)
    {
        _maxRound = roundCount;
        GameType = gameType;
        GameDifficulty = gameDifficulty;
    }
}
