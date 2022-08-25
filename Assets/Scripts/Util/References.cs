using UnityEngine;

public class References
{
    private static float _inputThreshold = 0.1f;
    public static float InputThreshold => _inputThreshold;

    private static float _fallThreshold = -1f;
    public static float FallThreshold => _fallThreshold;

    private static Vector3 _leftScale = new Vector3(-1, 1, 1);
    private static Vector3 _rightScale = new Vector3(1, 1, 1);
    public static Vector3 LeftScale => _leftScale;
    public static Vector3 RightScale => _rightScale;


    #region Firestore

    private static string _usersCollection = "USERS";
    public static string USERS_COLLECTION => _usersCollection;

    #endregion


    #region Sounds

    private static string _buttonSound = "Button";
    public static string BUTTON_SOUND => _buttonSound;

    private static string _hurtSound = "Hurt";
    public static string HURT_SOUND => _hurtSound;

    private static string _deathSound = "Death";
    public static string DEATH_SOUND => _deathSound;

    private static string _pewSound = "Pew";
    public static string PEW_SOUND => _pewSound;

    private static string _slashSound = "Slash1";
    public static string SLASH_SOUND => _slashSound;

    #endregion
}
