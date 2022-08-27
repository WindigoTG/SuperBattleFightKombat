using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PauseMenuPanel
{
    [SerializeField] private RectTransform _panel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _leaveButton;
    [SerializeField] private Button _quitButton;

    public RectTransform Panel => _panel;
    public Button ResumeButton => _resumeButton;
    public Button LeaveButton => _leaveButton;
    public Button QuitButton => _quitButton;
}
