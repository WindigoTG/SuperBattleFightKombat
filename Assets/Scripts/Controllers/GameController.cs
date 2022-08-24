using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Space]
    [SerializeField] private TellyController _tellyController;

    List<Player> _currentPlayers = new List<Player>();
    Dictionary<string, bool> _readyPlayers = new Dictionary<string, bool>();
    Dictionary<string, bool> _playersSelectedCharacter = new Dictionary<string, bool>();
    Dictionary<string, int> _livesPerPlayer = new Dictionary<string, int>();
    Dictionary<string, int> _scorePerPlayer = new Dictionary<string, int>();

    private string _localPlayerID;
    private PlayerController _localPlayerContoller;

    private bool _isGameStarted;
    private bool _isReadySequenceStarted;
    private bool _areCharactersSelected;

    private Coroutine _readyUpCoroutine;
    private Coroutine _startUpCoroutine;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _characterSelectionUI.Init();
        _characterSelectionUI.SetEnabled(false);
    }

    void Start()
    {

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
        }
        ResetLivesAndScore();

        foreach (var player in _currentPlayers)
            _gameUI.AddPlayer(player.UserId, player.NickName, _livesPerPlayer[player.UserId], _scorePerPlayer[player.UserId]);

        _localPlayerContoller = 
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0).GetComponent<PlayerController>();

        _localPlayerContoller.OnReady += OnLocalPlayerReady;
        _localPlayerContoller.OnDeath += OnLocalPlayerDead;

        _characterSelectionUI.SetEnabled(true);
        _characterSelectionUI.OnCharacterSelected += OnCharacterSelected;
    }

    private void Update()
    {
        _tellyController.UpdateRegular();
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

        _localPlayerContoller.Spawn(character);
        photonView.RPC(nameof(SetPlayerMadeSelectionRPC), RpcTarget.All, _localPlayerID);
    }

    [PunRPC]
    private void SetPlayerMadeSelectionRPC(string playerId)
    {
        if (_playersSelectedCharacter.ContainsKey(playerId))
            _playersSelectedCharacter[playerId] = true;

        CheckIfAllCharactersSelected();
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
        yield return new WaitForSeconds(_preHenshinDelay);

        photonView.RPC(nameof(StartHenshinRPC), RpcTarget.All);
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

    private IEnumerator StartUp()
    {
        yield return new WaitForSeconds(_preHenshinDelay);

        photonView.RPC(nameof(StartGameRPC), RpcTarget.All);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        _isGameStarted = true;
        _localPlayerContoller.StartGame();

        if (PhotonNetwork.IsMasterClient)
            _tellyController.StartSpawningTellys();
    }

    private void OnLocalPlayerDead(string attackerID) => photonView.RPC(nameof(OnPlayerDeathRPC), RpcTarget.All, _localPlayerID, attackerID);

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

        if (playerID.Equals(_localPlayerID))
            StartCoroutine(RespawnPlayer());

        
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(_respawnTime);

        _localPlayerContoller.RespawnPlayer();
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
    }

    #endregion
}
