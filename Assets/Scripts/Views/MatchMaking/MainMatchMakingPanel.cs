using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MainMatchMakingPanel : BasePanel
{
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomButton;

    public Button CreateRoomButton => _createRoomButton;
    public Button JoinRoomButton => _joinRoomButton;
}

public class BasePanel
{
    [SerializeField] protected RectTransform _panel;

    public virtual void SetEnabled(bool isEnabled)
    {
        _panel.gameObject.SetActive(isEnabled);
    }
}
