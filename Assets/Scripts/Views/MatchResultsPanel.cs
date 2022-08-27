using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MatchResultsPanel : MonoBehaviour
{
    #region Fields

    [SerializeField] RectTransform _scoreSection;
    [SerializeField] TextMeshProUGUI _scoreAmountText;
    [SerializeField] TextMeshProUGUI _scoreExpValueText;
    [SerializeField] TextMeshProUGUI _livesAmountText;
    [SerializeField] TextMeshProUGUI _livesExpValueText;
    [SerializeField] TextMeshProUGUI _totalExpValueText;
    [Space]
    [SerializeField] RectTransform _progressSection;
    [SerializeField] TextMeshProUGUI _currentLevelText;
    [SerializeField] TextMeshProUGUI _currentExpText;
    [SerializeField] Slider _expSlider;
    [SerializeField] float _progressSpeed = 0.5f;
    [Space]
    [SerializeField] RectTransform _playersSection;
    [SerializeField] PlayerResultWidget _playerWidgetPrefab;
    [SerializeField] RectTransform _playerWidgetParent;
    [Space]
    [SerializeField] RectTransform _buttonSection;
    [SerializeField] Button _leaveMatchButton;
    [SerializeField] Button _quitGameButton;
    [SerializeField] Button _rematchButton;
    [Space]
    [SerializeField] int _expForScore = 100;
    [SerializeField] int _expForLife = 150;
    [Space]
    [SerializeField] float _displayDelay;

    private Dictionary<string, PlayerResultWidget> _playerWidgetsById = new Dictionary<string, PlayerResultWidget>();

    int _currentScore;
    int _currentLives;

    int _expLeftToGive;
    int _currentExp;
    int _currentLevel;

    #endregion


    #region Properties

    public Button LeaveMatchButton => _leaveMatchButton;
    public Button QuitGameButton => _quitGameButton;
    public Button RematchButton => _rematchButton;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _rematchButton.onClick.AddListener(() => _rematchButton.interactable = false);
    }

    #endregion


    #region Methods

    public void OpenResults(int playerLives, int playerScore)
    {
        _currentLives = playerLives;
        _currentScore = playerScore;

        _scoreSection.gameObject.SetActive(false);
        _progressSection.gameObject.SetActive(false);
        _playersSection.gameObject.SetActive(false);
        _buttonSection.gameObject.SetActive(false);

        if (FirebaseManager.Instance != null)
        {
            _currentLevel = FirebaseManager.Instance.UserProfileHandler.UserLevel;
            _currentExp = FirebaseManager.Instance.UserProfileHandler.UserExp;
        }
        else
        {
            _currentExp = 0;
            _currentLevel = 1;
        }

        gameObject.SetActive(true);

        CreatePlayerWidgets();
        StartCoroutine(DisplayResults());
    }

    private void CreatePlayerWidgets()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            var widget = Instantiate(_playerWidgetPrefab, _playerWidgetParent);
            widget.SetPlayerName(player.Value.NickName);
            widget.SetRequestingRematch(false);
            _playerWidgetsById.Add(player.Value.UserId, widget);
        }
    }

    private IEnumerator DisplayResults()
    {
        _scoreAmountText.text = _currentScore.ToString();
        _scoreExpValueText.text = (_currentScore * _expForScore).ToString() + " XP";
        _livesAmountText.text = _currentLives.ToString();
        _livesExpValueText.text = (_currentLives * _expForLife).ToString() + " XP";
        _expLeftToGive = (_currentScore * _expForScore) + (_currentLives * _expForLife);
        _totalExpValueText.text = _expLeftToGive.ToString() + " XP";

        yield return new WaitForSeconds(_displayDelay);

        _scoreSection.gameObject.SetActive(true);

        yield return new WaitForSeconds(_displayDelay);

        StartCoroutine(DisplayExpGain());
    }

    private IEnumerator DisplayExpGain()
    {
        _currentLevelText.text = _currentLevel.ToString();
        _currentExpText.text = _currentExp.ToString();

        var minExp = CalculateXpForLevel(_currentLevel - 1);
        var maxExp = CalculateXpForLevel(_currentLevel);

        _expSlider.minValue = minExp;
        _expSlider.maxValue = maxExp;
        _expSlider.value = _currentExp;

        _progressSection.gameObject.SetActive(true);

        while (_expLeftToGive > 0)
        {
            var targetExp = _currentExp + _expLeftToGive;
            if (targetExp > maxExp)
            {
                _expLeftToGive = targetExp - maxExp;
                targetExp = maxExp;
            }
            else
                _expLeftToGive = 0;

            var startingvalue = _currentExp;
            var progress = 0f;

            while (_currentExp < targetExp)
            {
                progress += Time.deltaTime * _progressSpeed;
                Debug.Log(progress);
                _currentExp = (int)Mathf.Lerp(startingvalue, targetExp, progress);
                _currentExpText.text = _currentExp.ToString() + " XP";
                _expSlider.value = _currentExp;
                yield return null;
            }

            if (_currentExp == maxExp)
            {
                _currentLevel++;
                _currentLevelText.text = "Lvl. " + _currentLevel.ToString();

                minExp = CalculateXpForLevel(_currentLevel - 1);
                maxExp = CalculateXpForLevel(_currentLevel);

                _expSlider.minValue = minExp;
                _expSlider.maxValue = maxExp;
                _expSlider.value = _currentExp;
            }
        }

        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.UserProfileHandler.SetUserExp(_currentExp);
            FirebaseManager.Instance.UserProfileHandler.SetUserLevel(_currentLevel);
        }

        _playersSection.gameObject.SetActive(true);
        _rematchButton.interactable = _playerWidgetsById.Count > 1;
        _buttonSection.gameObject.SetActive(true);
    }

    public void SetPlayerRequestingRematch(string playerId)
    {
        if (!_playerWidgetsById.ContainsKey(playerId))
            return;

        _playerWidgetsById[playerId].SetRequestingRematch(true);
    }

    public void RemovePlayer(string playerId)
    {
        if (!_playerWidgetsById.ContainsKey(playerId))
            return;

        Destroy(_playerWidgetsById[playerId].gameObject);
        _playerWidgetsById.Remove(playerId);

        if (_rematchButton.interactable && _playerWidgetsById.Count < 2)
            _rematchButton.interactable = false;
    }

    public void CloseResults()
    {
        foreach (var widget in _playerWidgetsById)
            Destroy(widget.Value.gameObject);
        _playerWidgetsById.Clear();
        gameObject.SetActive(false);
    }
    
    private int CalculateXpForLevel(int level)
    {
        return level * level * 250 + level * 250;
    }

    #endregion
}
