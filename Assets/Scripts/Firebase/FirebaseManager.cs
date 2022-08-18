using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    #region Fields

    public static FirebaseManager Instance { get; private set; }

    public AuthenticationHandler Auth { get; private set; }
    public UserProfileHandler UserProfileHandler { get; private set; }

    #endregion


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Auth = new AuthenticationHandler();
            UserProfileHandler = new UserProfileHandler();
        }
        else if (Instance != this)
            Destroy(gameObject);
    }


    #region Methods

    public void Init()
    {
        Debug.Log("FirebaseManager.Init()");
        Auth.Init();
        UserProfileHandler.Init();
    }

    #endregion
}
