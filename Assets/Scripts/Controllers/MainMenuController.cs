using UnityEngine.UI;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    #region Fields

    [SerializeField] private RectTransform _menuGroup;
    [Space]
    [SerializeField] private AuthWindow _authWindow;
    [SerializeField] private LoginInputWindow _loginWindow;
    [SerializeField] private SigninInputWindow _signinWindow;
    [SerializeField] private StartGameWindow _startGameWindow;
    [SerializeField] private ChangeNameWindow _changeNameWindow;
    [Space]
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;
    [Space]
    [SerializeField] private AudioSettings _audionSettings;

    private string _password;
    private string _confirmPassword;
    private string _email;
    private string _userName;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        Application.targetFrameRate = 60;

        _menuGroup.gameObject.SetActive(false);

        AddListeners();

        if (FirebaseManager.Instance != null && FirebaseManager.Instance.Auth != null &&
            FirebaseManager.Instance.Auth.IsLoggedIn)
        {
            OnLogInSuccess();
        }
        else
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                Debug.Log($"Dependency status: {dependencyStatus}");
                if (dependencyStatus == DependencyStatus.Available)
                {
                    OnFirebaseResolved();
                }
                else
                {
                    Debug.Log("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
    }

    private void Start()
    {
        MusicManager.Instance.PLayMenuMusic();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    #endregion


    #region Methods

    private void AddListeners()
    {
        _quitButton.onClick.AddListener(QuitGame);
        _quitButton.onClick.AddListener(PlayButtonSound);

        _authWindow.LogInButton.onClick.AddListener(ShowLogInPanel);
        _authWindow.LogInButton.onClick.AddListener(PlayButtonSound);
        _authWindow.SignInButton.onClick.AddListener(ShowSignInPanel);
        _authWindow.SignInButton.onClick.AddListener(PlayButtonSound);

        _loginWindow.CancelButton.onClick.AddListener(ShowAuthPanel);
        _loginWindow.CancelButton.onClick.AddListener(PlayButtonSound);
        _signinWindow.CancelButton.onClick.AddListener(ShowAuthPanel);
        _signinWindow.CancelButton.onClick.AddListener(PlayButtonSound);
        _loginWindow.ConfirmButton.onClick.AddListener(BeginLogIn);
        _loginWindow.ConfirmButton.onClick.AddListener(PlayButtonSound);
        _signinWindow.ConfirmButton.onClick.AddListener(BeginSignIn);
        _signinWindow.ConfirmButton.onClick.AddListener(PlayButtonSound);
        _loginWindow.NewAccButton.onClick.AddListener(ShowSignInPanel);
        _loginWindow.NewAccButton.onClick.AddListener(PlayButtonSound);

        _loginWindow.EmailInput.onValueChanged.AddListener(OnEmailInputValueChanged);
        _signinWindow.EmailInput.onValueChanged.AddListener(OnEmailInputValueChanged);
        _loginWindow.PasswordInput.onValueChanged.AddListener(OnPasswordInputValueChanged);
        _signinWindow.PasswordInput.onValueChanged.AddListener(OnPasswordInputValueChanged);
        _signinWindow.ConfirmPasswordInput.onValueChanged.AddListener(OnConfirmPasswordInputValueChanged);
        _signinWindow.UsernameInput.onValueChanged.AddListener(OnUserNameInputValueChanged);

        _settingsButton.onClick.AddListener(() => _audionSettings.SetSettingsWindowActive(true));
        _settingsButton.onClick.AddListener(PlayButtonSound);

        _changeNameWindow.AddListeners();
        _changeNameWindow.SetOnCloseCallback(_startGameWindow.FillWelcomeText);

        _startGameWindow.ChangeNameButton.onClick.AddListener(() => _changeNameWindow.SetWindowActive(true));
        _startGameWindow.ChangeNameButton.onClick.AddListener(PlayButtonSound);
        _startGameWindow.SwitchAccountButton.onClick.AddListener(OnSwithAccountButtonClick);
        _startGameWindow.SwitchAccountButton.onClick.AddListener(PlayButtonSound);
        _startGameWindow.StartGameButton.onClick.AddListener(ProceedToMatchMaking);
        _startGameWindow.StartGameButton.onClick.AddListener(PlayButtonSound);
    }

    private void RemoveListeners()
    {
        _quitButton.onClick.RemoveAllListeners();

        _authWindow.LogInButton.onClick.RemoveAllListeners();
        _authWindow.SignInButton.onClick.RemoveAllListeners();

        _loginWindow.CancelButton.onClick.RemoveAllListeners();
        _signinWindow.CancelButton.onClick.RemoveAllListeners();
        _loginWindow.ConfirmButton.onClick.RemoveAllListeners();
        _signinWindow.ConfirmButton.onClick.RemoveAllListeners();
        _loginWindow.NewAccButton.onClick.RemoveAllListeners();

        _loginWindow.EmailInput.onValueChanged.RemoveAllListeners();
        _signinWindow.EmailInput.onValueChanged.RemoveAllListeners();
        _loginWindow.PasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.PasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.ConfirmPasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.UsernameInput.onValueChanged.RemoveAllListeners();

        _settingsButton.onClick.RemoveAllListeners();

        _changeNameWindow.RemoveListeners();

        _startGameWindow.ChangeNameButton.onClick.RemoveAllListeners();
        _startGameWindow.SwitchAccountButton.onClick.RemoveAllListeners();
        _startGameWindow.StartGameButton.onClick.RemoveAllListeners();
    }

        private void PlayButtonSound() => SoundManager.Instance.PlaySound(References.BUTTON_SOUND);

    private void OnFirebaseResolved()
    {
        _infoText.text = "Connecting...";
        FirebaseManager.Instance.Init();
        FirebaseManager.Instance.Auth.SilentLogIn(OnLogInSuccess, _ => EnableMenu());
    }

    private void ProceedToMatchMaking()
    {
        SceneManager.LoadScene(1);
    }

    private void EnableMenu()
    {
        _menuGroup.gameObject.SetActive(true);
        ShowAuthPanel();
        ClearInfoMessage();
    }

    private void OnLogInSuccess()
    {
        _menuGroup.gameObject.SetActive(true);
        ClearInfoMessage();
        ShowStartGamePanel();
    }

    private void ShowAuthPanel()
    {
        _authWindow.WindowPanel.gameObject.SetActive(true);
        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _startGameWindow.WindowPanel.gameObject.SetActive(false);
    }

    private void ShowLogInPanel()
    {
        _loginWindow.WindowPanel.gameObject.SetActive(true);
        _loginWindow.ClearInputs();
        _authWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _startGameWindow.WindowPanel.gameObject.SetActive(false);
    }

    private void ShowSignInPanel()
    {
        _signinWindow.WindowPanel.gameObject.SetActive(true);
        _signinWindow.ClearInputs();
        _authWindow.WindowPanel.gameObject.SetActive(false);
        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _startGameWindow.WindowPanel.gameObject.SetActive(false);
    }

    private void ShowStartGamePanel()
    {
        _startGameWindow.WindowPanel.gameObject.SetActive(true);
        _startGameWindow.FillWelcomeText();
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _authWindow.WindowPanel.gameObject.SetActive(false);
        _loginWindow.WindowPanel.gameObject.SetActive(false);
    }

    private void BeginLogIn()
    {
        _infoText.text = "Logging in...";
        _loginWindow.SetElementsInteractable(false);

        FirebaseManager.Instance.Auth.LogIn(_email, _password, OnLogInSuccess, OnLogInFailed);
    }

    private void OnLogInFailed(string message)
    {
        _loginWindow.SetElementsInteractable(true);
        SetConfirmButtonsInteractable();
        _infoText.text = message;
    }

    private void BeginSignIn()
    {
        if (!_password.Equals(_confirmPassword))
        {
            _infoText.text = "Passwords don't match";
            return;
        }

        FirebaseManager.Instance.Auth.SignIn(_email, _password, _userName, OnLogInSuccess, OnSignInFailed);

        _infoText.text = "Signing in...";
        _signinWindow.SetElementsInteractable(false);
    }

    private void OnSignInFailed(string message)
    {
        _signinWindow.SetElementsInteractable(true);
        SetConfirmButtonsInteractable();
        _infoText.text = message;
    }

    private void OnSwithAccountButtonClick()
    {
        FirebaseManager.Instance.Auth.SignOut();
        ShowAuthPanel();
    }

    private void ClearInfoMessage()
    {
        _infoText.text = "";
    }

    private void OnEmailInputValueChanged(string value)
    {
        _email = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnUserNameInputValueChanged(string value)
    {
        _userName = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnPasswordInputValueChanged(string value)
    {
        _password = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnConfirmPasswordInputValueChanged(string value)
    {
        _confirmPassword = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void SetConfirmButtonsInteractable()
    {
        _loginWindow.ConfirmButton.interactable = !(string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password));
        _signinWindow.ConfirmButton.interactable = !(string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password) ||
            string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_confirmPassword));

        ClearInfoMessage();
    }

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
