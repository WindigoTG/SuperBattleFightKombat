using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class UserProfile
{
    [FirestoreProperty]
    public string UserName { get; set; }
}
