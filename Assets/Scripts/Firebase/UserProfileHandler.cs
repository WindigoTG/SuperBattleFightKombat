using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class UserProfileHandler : MonoBehaviour
{
    #region Fields

    FirebaseFirestore _database;

    private string _currentUserId;

    private UserProfile _userProfile;

    #endregion


    #region Properties

    public string UserName => _userProfile.UserName;

    #endregion


    #region Methods

    public void Init()
    {
        Debug.Log("UserProfileHandler Init()");
        _database = FirebaseFirestore.DefaultInstance;
        _database.ClearPersistenceAsync();
    }

    public async void GetOrCreateUserProfile(Action callback, string currentUserId, string userName = "Anonymous")
    {
        _currentUserId = currentUserId;

        var getTask = _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).GetSnapshotAsync();

        await getTask;

        if (getTask.Result.Exists)
        {
            _userProfile = getTask.Result.ConvertTo<UserProfile>();
        }
        else
        {
            _userProfile = new UserProfile();

            _userProfile.UserName = userName;

            SetUserDataInDatabase();
        }

        callback();
    }

    public async void SetUserDataInDatabase()
    {
        if (_userProfile == null)
            return;

        Task setTask;

        do
        {
            setTask = _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).SetAsync(_userProfile);
            await setTask;
        } while (setTask.IsFaulted);
    }

    public void SetUserName(string userName)
    {
        _userProfile.UserName = userName;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.UserName), _userProfile.UserName);
    }

    #endregion
}
