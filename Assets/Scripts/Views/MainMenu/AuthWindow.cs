using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AuthWindow
{
    #region Fields

    [SerializeField] protected RectTransform _windowPanel;
    [SerializeField] protected Button _logInButton;
    [SerializeField] protected Button _SignInButton;

    #endregion


    #region Properties

    public RectTransform WindowPanel => _windowPanel;
    public Button LogInButton => _logInButton;
    public Button SignInButton => _SignInButton;

    #endregion
}
