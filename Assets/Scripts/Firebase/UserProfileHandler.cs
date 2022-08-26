using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class UserProfileHandler
{
    #region Fields

    FirebaseFirestore _database;

    private string _currentUserId;

    private UserProfile _userProfile;

    #endregion


    #region Properties

    public string UserName => _userProfile.UserName;
    public int UserLevel => _userProfile.Level;
    public int UserExp => _userProfile.Exp;
    public int MatchesPlayed => _userProfile.MatchesPlayed;
    public int MatchesWon => _userProfile.MatchesWon;
    public int TotalScore => _userProfile.TotalScore;

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
            if (_userProfile.Level == 0)
                _userProfile.Level = 1;
        }
        else
        {
            _userProfile = new UserProfile();

            _userProfile.UserName = userName;
            _userProfile.Level = 1;

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

    public void SetUserLevel(int level)
    {
        _userProfile.Level = level;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.Level), _userProfile.Level);
    }

    public void SetUserExp(int exp)
    {
        _userProfile.Exp = exp;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.Exp), _userProfile.Exp);
    }

    public void AddMatchPlayed()
    {
        _userProfile.MatchesPlayed++;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.MatchesPlayed), _userProfile.MatchesPlayed);
    }

    public void AddMatchWon()
    {
        _userProfile.MatchesWon++;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.MatchesWon), _userProfile.MatchesWon);
    }

    public void AddTotalScore(int score)
    {
        _userProfile.TotalScore += score;
        _database.Collection(References.USERS_COLLECTION).Document(_currentUserId).UpdateAsync(nameof(_userProfile.TotalScore), _userProfile.TotalScore);
    }

    #endregion
}
