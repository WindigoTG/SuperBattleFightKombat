using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomPlayerWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _crownIcon;

    public void FillInfo(string name, bool isMaster)
    {
        _nameText.text = name;
        _crownIcon.gameObject.SetActive(isMaster);
    }

    public void UpdateMasterStatus(bool isMaster) => _crownIcon.gameObject.SetActive(isMaster);
}
