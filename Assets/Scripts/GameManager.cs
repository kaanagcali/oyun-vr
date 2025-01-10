using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameType GameType { get; private set; }
    public GameDifficulty GameDifficulty { get; private set; }
    private int _currentRound;
    public int CurrentRound => _currentRound;
    private int _maxRound;

    private bool _isRoundEnd;
    public bool IsGameEnd { get; private set; }
    public bool IsGameStarted { get; private set; }

    private int _humanScore;
    public int HumanScore => _humanScore;
    private int _aiScore;
    public int AiScore => _aiScore;

    public GameObject confetti;

    public AudioClip winClip;
    public AudioClip loseClip;

    [SerializeField] private BaseEnemy _aiEasy;
    [SerializeField] private BaseEnemy _aiNormal;
    [SerializeField] private BaseEnemy _aiHard;
    [SerializeField] private Player _human;

    protected Vector3 _startPosOfAI;
    protected Vector3 _startAngleOfAI;

    public Action OnGameStartEvent;
    public Action OnGameEndEvent;
    public Action OnRoundEndEvent;

    [SerializeField] private GameObject[] objArrayToToggleOnStart;

    private void Awake()
    {
        Instance = this;
        GameType = GameType.Sword;
    }

    private void Start()
    {
        if (!_aiEasy) return;

        _startPosOfAI = _aiEasy.transform.position;
        _startAngleOfAI = _aiEasy.transform.eulerAngles;
        _aiEasy.gameObject.SetActive(false);
        _aiNormal.gameObject.SetActive(false);
        _aiHard.gameObject.SetActive(false);

        _aiEasy.OnHit += OnAiHit;
        _aiNormal.OnHit += OnAiHit;
        _aiHard.OnHit += OnAiHit;
        _human.OnHit += OnHumanHit;
    }

    private void Update()
    {
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
        newRoundStartDelay = aiNewRoundDelay;
        OnRoundEnd();
    }

    float aiNewRoundDelay = 3;
    float newRoundStartDelay = 2;
    float playerNewRoundDelay = 2;

    private void OnHumanHit()
    {
        if (_isRoundEnd) return;
        _aiScore++;
        newRoundStartDelay = playerNewRoundDelay;
        OnRoundEnd();
    }

    private void OnRoundEnd()
    {
        if (newRoundStartDelay == aiNewRoundDelay)
        {
            Time.timeScale = 0.5f;
            transform.DOScale(transform.localScale, 2).OnComplete(() =>
            {
                _aiEasy.gameObject.SetActive(false);
                _aiNormal.gameObject.SetActive(false);
                _aiHard.gameObject.SetActive(false);
                Time.timeScale = 1;
            });
        }
        else
        {
            _aiEasy.gameObject.SetActive(false);
            _aiNormal.gameObject.SetActive(false);
            _aiHard.gameObject.SetActive(false);
        }
        _isRoundEnd = true;
        OnRoundEndEvent?.Invoke();
        if (_currentRound == _maxRound)
            EndGame();
        else
            StartNewRound(newRoundStartDelay);
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
            _aiEasy.transform.position = _startPosOfAI;
            _aiEasy.transform.eulerAngles = _startAngleOfAI;

            _aiNormal.transform.position = _startPosOfAI;
            _aiNormal.transform.eulerAngles = _startAngleOfAI;

            _aiHard.transform.position = _startPosOfAI;
            _aiHard.transform.eulerAngles = _startAngleOfAI;

            _isRoundEnd = false;
            if (GameDifficulty == GameDifficulty.Easy)
                _aiEasy.gameObject.SetActive(true);
            else if (GameDifficulty == GameDifficulty.Normal)
                _aiNormal.gameObject.SetActive(true);
            else
                _aiHard.gameObject.SetActive(true);
        }
    }

    private void EndGame()
    {
        IsGameEnd = true;
        IsGameStarted = false;
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
        IsGameEnd = false;
        IsGameStarted = false;
        _currentRound = 1;
        _humanScore = 0;
        _aiScore = 0;
    }

    public void StartGame(GameType gameType, GameDifficulty gameDifficulty, int roundCount)
    {
        IsGameEnd = false;
        IsGameStarted = true;
        foreach (var obj in objArrayToToggleOnStart)
        {
            obj.SetActive(!obj.activeSelf);
        }

        _maxRound = roundCount;
        GameType = gameType;
        GameDifficulty = gameDifficulty;

        OnGameStartEvent?.Invoke();

        _aiEasy.gameObject.SetActive(false);
        _aiNormal.gameObject.SetActive(false);
        _aiHard.gameObject.SetActive(false);

        _aiEasy.transform.position = _startPosOfAI;
        _aiEasy.transform.eulerAngles = _startAngleOfAI;

        _aiNormal.transform.position = _startPosOfAI;
        _aiNormal.transform.eulerAngles = _startAngleOfAI;

        _aiHard.transform.position = _startPosOfAI;
        _aiHard.transform.eulerAngles = _startAngleOfAI;

        StartNewRound(1);
    }
}
