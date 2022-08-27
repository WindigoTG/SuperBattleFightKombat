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

    private static string _alertSound = "Alert";
    public static string ALERT_SOUND => _alertSound;

    private static string _boomSound = "Boom";
    public static string BOOM_SOUND => _boomSound;

    private static string _boostSound = "Boost";
    public static string BOOST_SOUND => _boostSound;

    private static string _buttonSound = "Button";
    public static string BUTTON_SOUND => _buttonSound;

    private static string _button2Sound = "Button2";
    public static string BUTTON_2_SOUND => _button2Sound;

    private static string _clingSound = "Cling";
    public static string CLING_SOUND => _clingSound;

    private static string _deathSound = "Death";
    public static string DEATH_SOUND => _deathSound;

    private static string _fightSound = "Fight";
    public static string FIGHT_SOUND => _fightSound;

    private static string _gameOverSound = "GameOver";
    public static string GAME_OVER_SOUND => _gameOverSound;

    private static string _healthLargeSound = "HealthLarge";
    public static string HEALTH_LARGE_SOUND => _healthLargeSound;

    private static string _healthSmallSound = "HealthSmall";
    public static string HEALTH_SMALL_SOUND => _healthSmallSound;

    private static string _hurtSound = "Hurt";
    public static string HURT_SOUND => _hurtSound;

    private static string _lifeSound = "Life";
    public static string LIFE_SOUND => _lifeSound;

    private static string _pewSound = "Pew";
    public static string PEW_SOUND => _pewSound;

    private static string _powSound = "Pow";
    public static string POW_SOUND => _powSound;

    private static string _slashSound = "Slash1";
    public static string SLASH_SOUND => _slashSound;

    private static string _slash2Sound = "Slash2";
    public static string SLASH_2_SOUND => _slash2Sound;

    private static string _swishSound = "Swish";
    public static string SWISH_SOUND => _swishSound;

    #endregion
}
