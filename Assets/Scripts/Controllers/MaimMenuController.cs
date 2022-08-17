using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button _quitButton;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        Application.targetFrameRate = 60;

        _quitButton.onClick.AddListener(QuitGame);
    }

    #endregion


    #region Methods

    private void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
    }

    #endregion
}
