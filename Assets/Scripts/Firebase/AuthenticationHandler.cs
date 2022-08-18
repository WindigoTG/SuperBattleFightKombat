using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class AuthenticationHandler
{
    #region Fields

    private FirebaseAuth _auth;

    private FirebaseUser _user;

    private Action _callback;
    private Action<string> _fallback;

    private string _password;
    private string _email;
    private string _username;

    private const string EMAIL_PREF = "Email";
    private const string PASSWORD_PREF = "Password";

    private bool _isLoggedIn;

    #endregion


    #region Properties

    public bool IsLoggedIn => _isLoggedIn;

    #endregion


    #region Methods

    public void Init()
    {
        Debug.Log("AuthenticationHandler Init()");
        _auth = FirebaseAuth.DefaultInstance;
    }

    public void SilentLogIn(Action callback, Action<string> fallback)
    {
        if (!PlayerPrefs.HasKey(EMAIL_PREF) || !PlayerPrefs.HasKey(PASSWORD_PREF))
        {
            fallback("");
            return;
        }

        LogIn(PlayerPrefs.GetString(EMAIL_PREF), PlayerPrefs.GetString(PASSWORD_PREF),
                                                                    callback, fallback);
    }

    public void LogIn(string email, string password, Action callback, Action<string> fallback)
    {
        _callback = callback;
        _fallback = fallback;

        _password = password;
        _email = email;

        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(OnLogInFinished);
    }

    private void OnLogInFinished(Task<FirebaseUser> result)
    {
        if (result.Exception != null)
        {
            FirebaseException firebaseEx = result.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Log in failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing E-mail";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "User Not Found";
                    break;
            }
            _fallback?.Invoke(message);
        }

        _user = result.Result;

        OnSuccess();
    }

    public void SignIn(string email, string password, string userName, Action callback, Action<string> fallback)
    {
        _callback = callback;
        _fallback = fallback;

        _password = password;
        _email = email;
        _username = userName;

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(OnSignInFinished);
    }

    private void OnSignInFinished(Task<FirebaseUser> result)
    {
        if (result.Exception != null)
        {
            FirebaseException firebaseEx = result.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Sign In Failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            _fallback?.Invoke(message);
        }

        _user = result.Result;

        OnSuccess();
    }

    private void OnSuccess()
    {
        PlayerPrefs.SetString(EMAIL_PREF, _email);
        PlayerPrefs.SetString(PASSWORD_PREF, _password);

        _isLoggedIn = true;

        if (string.IsNullOrEmpty(_username))
            FirebaseManager.Instance.UserProfileHandler.GetOrCreateUserProfile(_callback, _user.UserId);
        else
            FirebaseManager.Instance.UserProfileHandler.GetOrCreateUserProfile(_callback, _user.UserId, _username);
    }

    public void SignOut()
    {
        _isLoggedIn = false;
        _auth.SignOut();
        PlayerPrefs.DeleteKey(EMAIL_PREF);
        PlayerPrefs.DeleteKey(PASSWORD_PREF);
    }

    #endregion
}
