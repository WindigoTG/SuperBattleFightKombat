using UnityEngine;
using TMPro;

public class PlayerResultWidget : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _waitingText;
    [SerializeField] TextMeshProUGUI _rematchText;

    public void SetPlayerName(string name) => _playerNameText.text = name;

    public void SetRequestingRematch(bool isRequesting)
    {
        _waitingText.gameObject.SetActive(!isRequesting);
        _rematchText.gameObject.SetActive(isRequesting);
    }
}
