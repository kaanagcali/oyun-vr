using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    private int _currentRound;
    private int _maxRound;

    private void Awake() {
        Instance = this;
    }

    public void ClearActiveGame()
    {
        _currentRound = 1;
    }

    public void StartGame(GameType gameType, GameDifficulty gameDifficulty, int roundCount)
    {
        _maxRound = roundCount;
    }
}
