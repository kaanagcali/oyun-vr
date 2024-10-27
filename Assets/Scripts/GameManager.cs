using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    public GameType GameType { get; private set; }
    public GameDifficulty GameDifficulty { get; private set; }
    private int _currentRound;
    public int CurrentRound => _currentRound;
    private int _maxRound;

    private bool _isRoundEnd;
    private bool _isGameEnd;

    private int _humanScore;
    public int HumanScore => _humanScore;
    private int _aiScore;
    public int AiScore => _aiScore;

    public GameObject confetti;

    public AudioClip winClip;
    public AudioClip loseClip;

    [SerializeField] private BaseEnemy _ai;
    [SerializeField] private Player _human;
    
    protected Vector3 _startPosOfAI;
    protected Vector3 _startAngleOfAI;

    public Action OnGameStartEvent;
    public Action OnGameEndEvent;
    public Action OnRoundEndEvent;
    
    [SerializeField] private GameObject[] objArrayToToggleOnStart;

    private void Awake() {
        Instance = this;
        GameType = GameType.Sword;
    }

    private void Start()
    {
        if (!_ai) return;

        _startPosOfAI = _ai.transform.position;
        _startAngleOfAI = _ai.transform.eulerAngles;
        _ai.gameObject.SetActive(false);

        _ai.OnHit += OnAiHit;
        _human.OnHit += OnHumanHit;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ClearActiveGame();
            StartGame(GameType.Sword, GameDifficulty.Easy, 5);
        }
    }

    private void OnAiHit()
    {
        if (_isRoundEnd) return;
        _humanScore++;
        OnRoundEnd();
    }

    private void OnHumanHit()
    {
        if (_isRoundEnd) return;
        _aiScore++;
        OnRoundEnd();
    }

    private void OnRoundEnd()
    {
        _ai.gameObject.SetActive(false);
        _isRoundEnd = true;
        OnRoundEndEvent?.Invoke();
        if (_currentRound == _maxRound)
            EndGame();
        else
            StartNewRound(2f);
        _currentRound++;
    }

    public Action<int> OnRoundCounterChangedEvent;

    private void StartNewRound(float delay)
    {
        StartCoroutine(NewRoundCoroutine());
        IEnumerator NewRoundCoroutine()
        {
            yield return new WaitForSeconds(delay);
            int counter = 3;
            OnRoundCounterChangedEvent?.Invoke(counter);
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
                OnRoundCounterChangedEvent?.Invoke(counter);
            }
            _ai.transform.position = _startPosOfAI;
            _ai.transform.eulerAngles = _startAngleOfAI;
            _isRoundEnd = false;
            _ai.gameObject.SetActive(true);
        }
    }

    private void EndGame()
    {
        foreach (var obj in objArrayToToggleOnStart)
        {
            obj.SetActive(!obj.activeSelf);
        }

        if (AiScore > HumanScore)
        {
            AudioSource.PlayClipAtPoint(loseClip, Camera.main.transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(winClip, Camera.main.transform.position);
            confetti.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3f;
            confetti.gameObject.SetActive(false);
            confetti.gameObject.SetActive(true);
        }

        OnGameEndEvent?.Invoke();
    }

    public void ClearActiveGame()
    {
        _currentRound = 1;
        _humanScore = 0;
        _aiScore = 0;
    }

    public void StartGame(GameType gameType, GameDifficulty gameDifficulty, int roundCount)
    {
        foreach (var obj in objArrayToToggleOnStart)
        {
            obj.SetActive(!obj.activeSelf);
        }

        _maxRound = roundCount;
        GameType = gameType;
        GameDifficulty = gameDifficulty;

        OnGameStartEvent?.Invoke();

        StartNewRound(1);
    }
}
