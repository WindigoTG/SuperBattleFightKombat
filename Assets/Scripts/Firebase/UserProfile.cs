using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class UserProfile
{
    [FirestoreProperty]
    public string UserName { get; set; }

    [FirestoreProperty]
    public int Level { get; set; }

    [FirestoreProperty]
    public int Exp { get; set; }

    [FirestoreProperty]
    public int MatchesPlayed { get; set; }

    [FirestoreProperty]
    public int MatchesWon { get; set; }

    [FirestoreProperty]
    public int TotalScore { get; set; }
}
