using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomWidgetView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Toggle _isOpenToggle;
    [SerializeField] private TextMeshProUGUI _capacityText;

    private string _roomName;
    private Action<string> _onClickCallback;

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickCallback?.Invoke(_roomName);
    }

    public void SetNameAndOnClickCallback(string roomName, Action<string> onClickCallback)
    {
        _roomName = roomName;
        _onClickCallback = onClickCallback;
        _roomNameText.text = roomName;
    }

    public void SetSelected(bool isSelected)
    {
        _background.color = isSelected ? _selectedColor : _defaultColor;
    }

    public void SetIsOpen(bool isOpen)
    {
        _isOpenToggle.isOn = isOpen;
    }

    public void SetCapacity(int currentPlayers, int maxPLayers)
    {
        _capacityText.text = $"{currentPlayers}/{maxPLayers}";
    }
}
