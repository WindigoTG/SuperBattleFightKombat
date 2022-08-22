using UnityEngine;
using TMPro;

public class PlayerStatsWidget : MonoBehaviour
{
    #region Fields

    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _livesAmountText;
    [SerializeField] private TextMeshProUGUI _scoreAmountText;

    public void SetPlayerName(string name) => _playerNameText.text = name;

    public void SetLivesAmount(int lives) => _livesAmountText.text = lives.ToString();

    public void SetScoreAmount(int score) => _scoreAmountText.text = score.ToString();

    #endregion
}
