using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviourPunCallbacks
{
    #region Fields

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _preHenshinDelay = 2.5f;
    [SerializeField] private float _postHenshinDelay = 2.5f;
    [SerializeField] private float _respawnTime = 2.0f;

    [SerializeField] private int _startingLives = 5;
    [Space]
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private CharacterSelectionUI _characterSelectionUI;
    [SerializeField] private MatchResultsPanel _matchResultsPanel;
    [Space]
    [SerializeField] private TellyController _tellyController;
    [Space]
    [SerializeField] private SpawnPointHandler _spawnPointHandler;
    [Space]
    [SerializeField] private Animator _matchStartMessagesAnimator;
    [SerializeField] private string _readyAnimationTrigger = "Ready";
    [SerializeField] private string _figthAnimationTrigger = "Fight";
    [Space]
    [SerializeField] private PickupHandler _pickupHandler;
    [Space]
    [SerializeField] private PauseMenuPanel _pauseMenu;

    List<Player> _currentPlayers = new List<Player>();
    Dictionary<string, bool> _readyPlayers = new Dictionary<string, bool>();
    Dictionary<string, bool> _playersSelectedCharacter = new Dictionary<string, bool>();
    Dictionary<string, bool> _playersRequestingRematch = new Dictionary<string, bool>();
    Dictionary<string, int> _livesPerPlayer = new Dictionary<string, int>();
    Dictionary<string, int> _scorePerPlayer = new Dictionary<string, int>();

    private string _localPlayerID;
    private PlayerController _localPlayerContoller;

    private bool _isGameStarted;
    private bool _isReadySequenceStarted;
    private bool _areCharactersSelected;
    private bool _isGameFinished;

    private Coroutine _readyUpCoroutine;
    private Coroutine _startUpCoroutine;

    private Character _selectedCharacter;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _characterSelectionUI.Init();
        _characterSelectionUI.SetEnabled(false);

        _matchResultsPanel.CloseResults();
        _matchResultsPanel.LeaveMatchButton.onClick.AddListener(LeaveRoom);
        _matchResultsPanel.LeaveMatchButton.onClick.AddListener(PlayButtonSound);
        _matchResultsPanel.QuitGameButton.onClick.AddListener(QuitGame);
        _matchResultsPanel.QuitGameButton.onClick.AddListener(PlayButtonSound);
        _matchResultsPanel.RematchButton.onClick.AddListener(SendRematchRequest);
        _matchResultsPanel.RematchButton.onClick.AddListener(PlayButtonSound);

        _pauseMenu.ResumeButton.onClick.AddListener(OpenOrClosePauseMenu);
        _pauseMenu.ResumeButton.onClick.AddListener(PlayButtonSound);
        _pauseMenu.LeaveButton.onClick.AddListener(LeaveRoom);
        _pauseMenu.LeaveButton.onClick.AddListener(PlayButtonSound);
        _pauseMenu.QuitButton.onClick.AddListener(QuitGame);
        _pauseMenu.QuitButton.onClick.AddListener(PlayButtonSound);
    }

    void Start()
    {
        MusicManager.Instance.StopMusic();

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(1);

            return;
        }

        _localPlayerID = PhotonNetwork.LocalPlayer.UserId;

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            _currentPlayers.Add(player.Value);
            _readyPlayers.Add(player.Value.UserId, false);
            _playersSelectedCharacter.Add(player.Value.UserId, false);
            _playersRequestingRematch.Add(player.Value.UserId, false);
        }
        ResetLivesAndScore();

        foreach (var player in _currentPlayers)
            _gameUI.AddPlayer(player.UserId, player.NickName, _livesPerPlayer[player.UserId], _scorePerPlayer[player.UserId]);

        _localPlayerContoller =
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0).GetComponent<PlayerController>();

        _localPlayerContoller.OnReady += OnLocalPlayerReady;
        _localPlayerContoller.OnDeath += OnLocalPlayerDead;
        _localPlayerContoller.OnLifePickup += OnLocalPlayerLifePickup;

        _characterSelectionUI.SetEnabled(true);
        _characterSelectionUI.OnCharacterSelected += OnCharacterSelected;
    }

    private void Update()
    {
        _tellyController.UpdateRegular();
        if (PhotonNetwork.IsMasterClient && _isGameStarted && !_isGameFinished)
            _pickupHandler.UpdateRegular();

        if (Input.GetKeyDown(KeyCode.Escape) && _isGameStarted && !_isGameFinished)
            OpenOrClosePauseMenu();
    }

    private void OnDestroy()
    {
        if (_readyUpCoroutine != null)
            StopCoroutine(_readyUpCoroutine);

        if (_startUpCoroutine != null)
            StopCoroutine(_startUpCoroutine);
    }

    #endregion


    #region Methods

    private void OpenOrClosePauseMenu()
    {
        _pauseMenu.Panel.gameObject.SetActive(!_pauseMenu.Panel.gameObject.activeSelf);
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(References.BUTTON_SOUND);
    }

    private void ResetLivesAndScore()
    {
        _livesPerPlayer.Clear();

        foreach (var player in _currentPlayers)
            _livesPerPlayer.Add(player.UserId, _startingLives);

        _scorePerPlayer.Clear();

        foreach (var player in _currentPlayers)
            _scorePerPlayer.Add(player.UserId, 0);
    }

    private void OnCharacterSelected(Character character)
    {
        _characterSelectionUI.SetEnabled(false);
        _characterSelectionUI.OnCharacterSelected -= OnCharacterSelected;

        _selectedCharacter = character;

        photonView.RPC(nameof(RequestSpawnPointRPC), RpcTarget.MasterClient, _localPlayerID);
    }

    [PunRPC]
    private void SetPlayerMadeSelectionRPC(string playerId)
    {
        if (_playersSelectedCharacter.ContainsKey(playerId))
            _playersSelectedCharacter[playerId] = true;

        CheckIfAllCharactersSelected();
    }

    [PunRPC]
    private void RequestSpawnPointRPC(string playerId)
    {
        photonView.RPC(nameof(ReceiveSpawnPointRPC), RpcTarget.All, playerId, _spawnPointHandler.GetRandomSpawnPoint());
    }

    [PunRPC]
    private void ReceiveSpawnPointRPC(string playerID, Vector3 position)
    {
        if (!playerID.Equals(_localPlayerID))
            return;

        _localPlayerContoller.Spawn(_selectedCharacter, position);
        photonView.RPC(nameof(SetPlayerMadeSelectionRPC), RpcTarget.All, _localPlayerID);
    }

    private void OnLocalPlayerReady()
    {
        photonView.RPC(nameof(SetPlayerReadyRPC), RpcTarget.All, _localPlayerID);
    }

    [PunRPC]
    private void SetPlayerReadyRPC(string playerId)
    {
        if (_readyPlayers.ContainsKey(playerId))
            _readyPlayers[playerId] = true;

        CheckIfAllPlayersAreReady();
    }

    [PunRPC]
    private void StartHenshinRPC()
    {
        _isReadySequenceStarted = true;
        _localPlayerContoller.StartHenshin();
    }

    private IEnumerator StartHenshin()
    {
        photonView.RPC(nameof(SetAnimationTriggerRPC), RpcTarget.All, _readyAnimationTrigger);

        yield return new WaitForSeconds(_preHenshinDelay);

        photonView.RPC(nameof(StartHenshinRPC), RpcTarget.All);
        var musicIndex = Random.Range(0, MusicManager.Instance.MatchMusicCount);
        photonView.RPC(nameof(StartPlayingMusic), RpcTarget.All, musicIndex);
    }

    private void CheckIfAllPlayersAreReady()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        foreach (var player in _readyPlayers)
            if (!player.Value)
                return;

        _startUpCoroutine = StartCoroutine(StartUp());
    }

    private void CheckIfAllCharactersSelected()
    {
        foreach (var player in _playersSelectedCharacter)
            if (!player.Value)
                return;

        _areCharactersSelected = true;

        if (PhotonNetwork.IsMasterClient)
            _readyUpCoroutine = StartCoroutine(StartHenshin());
    }

    [PunRPC]
    private void SetAnimationTriggerRPC(string trigger)
    {
        _matchStartMessagesAnimator.SetTrigger(trigger);
        if (trigger.Equals(_readyAnimationTrigger))
            SoundManager.Instance.PlaySound(References.ALERT_SOUND);
        else if (trigger.Equals(_figthAnimationTrigger))
            SoundManager.Instance.PlaySound(References.FIGHT_SOUND);
    }

    private IEnumerator StartUp()
    {
        photonView.RPC(nameof(SetAnimationTriggerRPC), RpcTarget.All, _figthAnimationTrigger);

        yield return new WaitForSeconds(_preHenshinDelay);

        photonView.RPC(nameof(StartGameRPC), RpcTarget.All);
    }

    [PunRPC]
    private void StartPlayingMusic(int index) => MusicManager.Instance.PlayMatchMusic(index);

    [PunRPC]
    private void StartGameRPC()
    {
        _isGameStarted = true;
        _localPlayerContoller.StartGame();

        if (PhotonNetwork.IsMasterClient)
            _tellyController.StartSpawningTellys();

        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.UserProfileHandler.AddMatchPlayed();
        }
    }

    private void OnLocalPlayerDead(string attackerID) => photonView.RPC(nameof(OnPlayerDeathRPC), RpcTarget.All, _localPlayerID, attackerID);

    private void OnLocalPlayerLifePickup() => photonView.RPC(nameof(OnPlayerLifePickupRPC), RpcTarget.All, _localPlayerID);

    [PunRPC] 
    private void OnPlayerLifePickupRPC(string playerID)
    {
        if (_livesPerPlayer.ContainsKey(playerID))
        {
            _livesPerPlayer[playerID]++;
            _gameUI.UpdateLivesForPLayer(_livesPerPlayer[playerID], playerID);
        }
    }

    [PunRPC]
    private void OnPlayerDeathRPC(string playerID, string attackerID)
    {
        if (_livesPerPlayer.ContainsKey(playerID))
        {
            _livesPerPlayer[playerID]--;
            _gameUI.UpdateLivesForPLayer(_livesPerPlayer[playerID], playerID);
        }

        if (_scorePerPlayer.ContainsKey(attackerID))
        {
            _scorePerPlayer[attackerID]++;
            _gameUI.UpdateScoreForPlayer(_scorePerPlayer[attackerID], attackerID);
        }

        if (playerID.Equals(_localPlayerID) && _livesPerPlayer[playerID] > 0)
            StartCoroutine(RespawnPlayer());

        CheckGameState();
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(_respawnTime);

        if (!_isGameFinished)
            _localPlayerContoller.RespawnPlayerAtPosition(_spawnPointHandler.GetRandomSpawnPoint());
    }

    private void CheckGameState()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int alivePlayers = 0;

        foreach (var playerLives in _livesPerPlayer)
            if (playerLives.Value > 0)
                alivePlayers++;

        if (alivePlayers <= 1)
            photonView.RPC(nameof(StopGame), RpcTarget.All);
    }

    [PunRPC]
    private void StopGame()
    {
        if (PhotonNetwork.IsMasterClient)
            _tellyController.KillAndStopSpawningTellys();

        MusicManager.Instance.StopMusic();
        _localPlayerContoller.StopGame();
        _isGameFinished = true;
        OpenResultsPanel();

        if (FirebaseManager.Instance != null)
        {
            if (_livesPerPlayer[_localPlayerID] > 0)
                FirebaseManager.Instance.UserProfileHandler.AddMatchWon();

            FirebaseManager.Instance.UserProfileHandler.AddTotalScore(_scorePerPlayer[_localPlayerID]);
        }
            
    }

    private void OpenResultsPanel()
    {
        SoundManager.Instance.PlaySound(References.GAME_OVER_SOUND);
        if (_isGameStarted)
            _matchResultsPanel.OpenResults(_livesPerPlayer[_localPlayerID], _scorePerPlayer[_localPlayerID]);
        else
            _matchResultsPanel.OpenResults(0, 0);
    }

    private void SendRematchRequest()
    {
        photonView.RPC(nameof(RematchRequestedRPC), RpcTarget.All, _localPlayerID);
    }

    [PunRPC]
    private void RematchRequestedRPC(string playerId)
    {
        if (!_playersRequestingRematch.ContainsKey(playerId))
            return;

        _playersRequestingRematch[playerId] = true;
        _matchResultsPanel.SetPlayerRequestingRematch(playerId);

        CheckIfAllPLayersRequestedRematch();
    }

    private void CheckIfAllPLayersRequestedRematch()
    {
        if (!PhotonNetwork.IsMasterClient || _playersRequestingRematch.Count < 2)
            return;

        foreach (var player in _playersRequestingRematch)
            if (!player.Value)
                return;

        photonView.RPC(nameof(RestartRPC), RpcTarget.All);
    }

    [PunRPC]
    private void RestartRPC()
    {
        ResetLivesAndScore();
        foreach (var player in _currentPlayers)
        {
            _gameUI.UpdateLivesForPLayer(_startingLives, player.UserId);
            _gameUI.UpdateScoreForPlayer(0, player.UserId);
        }

        _isGameStarted = false;
        _isReadySequenceStarted = false;
        _areCharactersSelected = false;
        _isGameFinished = false;


        foreach (var player in _currentPlayers)
        {
            _readyPlayers[player.UserId] = false;
            _playersRequestingRematch[player.UserId] = false;
            _playersSelectedCharacter[player.UserId] = false;
        }

        _characterSelectionUI.SetEnabled(true);
        _characterSelectionUI.OnCharacterSelected += OnCharacterSelected;

        _matchResultsPanel.CloseResults();
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
        PhotonNetwork.Disconnect();
        
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    #endregion


    #region PUN Callbacks

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!_areCharactersSelected)
        {
            CheckIfAllCharactersSelected();
            return;
        }

        if (!_isReadySequenceStarted)
        {
            _readyUpCoroutine = StartCoroutine(StartHenshin());
            return;
        }

        if (!_isGameStarted)
        {
            CheckIfAllPlayersAreReady();
            return;
        }

        _tellyController.TakeOver();

        if (_isGameFinished)
        {
            _tellyController.KillAndStopSpawningTellys();
            CheckIfAllPLayersRequestedRematch();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _gameUI.RemovePlayer(otherPlayer.UserId);

        if (_currentPlayers.Contains(otherPlayer))
            _currentPlayers.Remove(otherPlayer);

        if (_readyPlayers.ContainsKey(otherPlayer.UserId))
            _readyPlayers.Remove(otherPlayer.UserId);

        if (_livesPerPlayer.ContainsKey(otherPlayer.UserId))
            _livesPerPlayer.Remove(otherPlayer.UserId);

        if (_scorePerPlayer.ContainsKey(otherPlayer.UserId))
            _scorePerPlayer.Remove(otherPlayer.UserId);

        if (_playersSelectedCharacter.ContainsKey(otherPlayer.UserId))
            _playersSelectedCharacter.Remove(otherPlayer.UserId);

        if (_playersRequestingRematch.ContainsKey(otherPlayer.UserId))
        {
            _playersRequestingRematch.Remove(otherPlayer.UserId);
            _matchResultsPanel.RemovePlayer(otherPlayer.UserId);
        }

        if (_currentPlayers.Count < 2 && !_isGameFinished)
            StopGame();
    }

    #endregion
}
