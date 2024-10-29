using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.IO.Compression;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _headerTMP;
    [SerializeField] private TextMeshProUGUI _roundCountTMP;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private List<PanelData> _panelList;
    [SerializeField] private Slider _roundCountSlider;
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Button _showNextPanelBtn;
    [SerializeField] private Button _showPrevPanelBtn;

    private int _activePanelIndex = 0;
    private GameDifficulty _gameDifficulty = GameDifficulty.Easy;
    private GameType _gameType = GameType.Sword;
    private int _roundCount;
    private bool _isOpen = true;
    private Vector3 _defaultScale;

    private void Awake()
    {
        Instance = this;

        _defaultScale = transform.localScale;

        for (int i = 0; i < _panelList.Count; i++)
        {
            var panel = _panelList[i];
            if (panel.ActiveAtStart)
            {
                panel.SetActiveInstant(true);
                _headerTMP.SetText(panel.PanelName);
                _activePanelIndex = i;
            }
            else
            {
                panel.SetActiveInstant(false);
            }
            panel.PanelCanvasGroup.gameObject.SetActive(true);
        }
        UpdatePanelBtns();
        
        _roundCount = (int)_roundCountSlider.value;
        _roundCountTMP.SetText(_roundCount.ToString());
    }

    private void Start()
    {
        GameManager.Instance.OnGameEndEvent += OpenUI;
        GameManager.Instance.OnGameStartEvent += CloseUI;
    }

    private void OnEnable()
    {
        _roundCountSlider.onValueChanged.AddListener(OnRoundCountChanged);
        _dropdown.onValueChanged.AddListener(OnDifficultyChanged);
        _startGameBtn.onClick.AddListener(StartGame);
        _showNextPanelBtn.onClick.AddListener(ActivateNextPanel);
        _showPrevPanelBtn.onClick.AddListener(ActivatePreviousPanel);
    }

    private void OnDifficultyChanged(int arg0)
    {
        _gameDifficulty = (GameDifficulty)arg0;
    }

    private void OnDisable()
    {
        _roundCountSlider.onValueChanged.RemoveListener(OnRoundCountChanged);
        _dropdown.onValueChanged.RemoveListener(OnDifficultyChanged);
        _startGameBtn.onClick.RemoveListener(StartGame);
        _showNextPanelBtn.onClick.RemoveListener(ActivateNextPanel);
        _showPrevPanelBtn.onClick.RemoveListener(ActivatePreviousPanel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isOpen)
                CloseUI();
            else
                OpenUI();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ActivateNextPanel();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ActivatePreviousPanel();
        }
    }

    private void StartGame()
    {
        GameManager.Instance.ClearActiveGame();
        GameManager.Instance.StartGame(_gameType, _gameDifficulty, _roundCount);
    }

    public void CloseUI()
    {
        _isOpen = false;
        DOTween.Kill(transform, false);
        transform.DOScale(Vector3.zero, 0.5f);
    }

    public void OpenUI()
    {
        _isOpen = true;
        DOTween.Kill(transform, false);
        transform.DOScale(_defaultScale, 1).SetEase(Ease.InOutBack);
    }

    private void OnRoundCountChanged(float roundCount)
    {
        _roundCount = (int)roundCount;
        _roundCountTMP.SetText(_roundCount.ToString());
    }

    private void UpdatePanelBtns()
    {
        _showPrevPanelBtn.gameObject.SetActive(_activePanelIndex != 0);
        _showNextPanelBtn.gameObject.SetActive(_activePanelIndex != _panelList.Count - 1);
    }

    private void ActivatePreviousPanel()
    {
        if (_activePanelIndex <= 0) return;

        ActivatePanel(_activePanelIndex - 1);
    }

    private void ActivateNextPanel()
    {
        if (_activePanelIndex >= _panelList.Count - 1) return;

        ActivatePanel(_activePanelIndex + 1);
    }

    public void ActivatePanel(int panelIndex)
    {
        ActivatePanel(_panelList[panelIndex]);
    }

    public void ActivatePanel(string panelName)
    {
        ActivatePanel(_panelList.Find(x => x.PanelName.Equals(panelName)));
    }

    public void ActivatePanel(PanelData panelData)
    {
        for (int i = 0; i < _panelList.Count; i++)
        {
            var panel = _panelList[i];
            if (panelData == panel)
            {
                panel.SetActive(true);
                _headerTMP.SetText(panel.PanelName);
                _activePanelIndex = i;
            }
            else
            {
                panel.SetActive(false);
            }
        }
        UpdatePanelBtns();
    }
}

[System.Serializable]
public class PanelData
{
    public string PanelName;
    public CanvasGroup PanelCanvasGroup;
    public bool ActiveAtStart;
    public bool IsActive => PanelCanvasGroup.alpha == 1;

    public void SetActive(bool isActive)
    {
        PanelCanvasGroup.DOFade(isActive ? 1 : 0, 0.3f);
    }

    public void SetActiveInstant(bool isActive)
    {
        PanelCanvasGroup.alpha = isActive ? 1 : 0;
    }
}


public enum GameDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2
}

public enum GameType
{
    Flöre,
    Epe,
    Sword
}