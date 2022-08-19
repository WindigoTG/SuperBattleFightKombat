using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LoginInputWindow : CredentialsInputWindowBase
{
    #region Fields

    [SerializeField] protected Button _newAccButton;

    #endregion


    #region Properties
    public Button NewAccButton => _newAccButton;

    #endregion


    #region Methods

    public override void SetElementsInteractable(bool isInteractable)
    {
        base.SetElementsInteractable(isInteractable);

        _newAccButton.interactable = isInteractable;
    }

    #endregion
}
