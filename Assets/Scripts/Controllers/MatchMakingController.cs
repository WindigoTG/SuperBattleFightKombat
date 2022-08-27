using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchMakingController : MonoBehaviourPunCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [Space]
    [SerializeField] private Canvas _roomsCanvas;
    [SerializeField] private MainMatchMakingPanel _mainPanel;
    [SerializeField] private CreateRoomPanel _createRoomPanel;
    [SerializeField] private JoinRoomPanel _joinRoomPanel;
    [SerializeField] private InRoomPanel _inRoomPanel;
    [SerializeField] private PlayerListPanel _playerListPanel;
    [SerializeField] private Button _backToMenuButton;
    [Space]
    [SerializeField] private LogManager _logManager;

    private TypedLobby _customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

    #region UnityMethods

    void Start()
    {
        _roomsCanvas.enabled = false;

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings(_serverSettings.AppSettings);
        else
            OnConnectedToMaster();

        AddListeners();

        PhotonNetwork.AutomaticallySyncScene = true;

        MusicManager.Instance.PLayMenuMusic();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    #endregion


    #region Methods

    private void AddListeners()
    {
        _mainPanel.CreateRoomButton.onClick.AddListener(ShowCreateRoomPanel);
        _mainPanel.CreateRoomButton.onClick.AddListener(PlayButtonSound);
        _mainPanel.JoinRoomButton.onClick.AddListener(ShowJoinRoomPanel);
        _mainPanel.JoinRoomButton.onClick.AddListener(PlayButtonSound);

        _createRoomPanel.CreateRoom.onClick.AddListener(CreateRoom);
        _createRoomPanel.CreateRoom.onClick.AddListener(PlayButtonSound);
        _createRoomPanel.BackButton.onClick.AddListener(ShowMainMenuPanel);
        _createRoomPanel.BackButton.onClick.AddListener(PlayButtonSound);

        _joinRoomPanel.BackButton.onClick.AddListener(ShowMainMenuPanel);
        _joinRoomPanel.BackButton.onClick.AddListener(PlayButtonSound);
        _joinRoomPanel.JoinSelectedButton.onClick.AddListener(JoinSelectedRoom);
        _joinRoomPanel.JoinSelectedButton.onClick.AddListener(PlayButtonSound);
        _joinRoomPanel.JoinByNameButton.onClick.AddListener(JoinRoomByName);
        _joinRoomPanel.JoinByNameButton.onClick.AddListener(PlayButtonSound);

        _inRoomPanel.LeaveRoomButton.onClick.AddListener(LeaveRoom);
        _inRoomPanel.LeaveRoomButton.onClick.AddListener(PlayButtonSound);
        _inRoomPanel.IsOpenToggle.onValueChanged.AddListener(SetRoomOpen);
        _inRoomPanel.IsVisibleToggle.onValueChanged.AddListener(SetRoomVisible);
        _inRoomPanel.StartGameButton.onClick.AddListener(StartGame);
        _inRoomPanel.StartGameButton.onClick.AddListener(PlayButtonSound);

        _backToMenuButton.onClick.AddListener(BackToMainMenu);
        _backToMenuButton.onClick.AddListener(PlayButtonSound);
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(References.BUTTON_SOUND);
    }

    private void RemoveListeners()
    {
        _mainPanel.CreateRoomButton.onClick.RemoveAllListeners();
        _mainPanel.JoinRoomButton.onClick.RemoveAllListeners();

        _createRoomPanel.CreateRoom.onClick.RemoveAllListeners();
        _createRoomPanel.BackButton.onClick.RemoveAllListeners();

        _joinRoomPanel.BackButton.onClick.RemoveAllListeners();
        _joinRoomPanel.JoinSelectedButton.onClick.RemoveAllListeners();
        _joinRoomPanel.JoinByNameButton.onClick.RemoveAllListeners();

        _inRoomPanel.LeaveRoomButton.onClick.RemoveAllListeners();
        _inRoomPanel.IsOpenToggle.onValueChanged.RemoveAllListeners();
        _inRoomPanel.IsVisibleToggle.onValueChanged.RemoveAllListeners();
        _inRoomPanel.StartGameButton.onClick.RemoveAllListeners();

        _backToMenuButton.onClick.RemoveAllListeners();
    }

    private void BackToMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    private void ShowMainMenuPanel()
    {
        _mainPanel.SetEnabled(true);
        _createRoomPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
        _playerListPanel.SetEnabled(false);
    }

    private void ShowCreateRoomPanel()
    {
        _createRoomPanel.SetEnabled(true);
        _mainPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
        _playerListPanel.SetEnabled(false);

        _createRoomPanel.IsOpenToggle.isOn = true;
        _createRoomPanel.IsVisibleToggle.isOn = true;
        _createRoomPanel.Input.text = "";
    }

    private void ShowJoinRoomPanel()
    {
        _joinRoomPanel.SetEnabled(true);
        _mainPanel.SetEnabled(false);
        _createRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
        _playerListPanel.SetEnabled(false);

        _joinRoomPanel.Input.text = "";
    }

    private void ShowInRoomPanel()
    {
        _inRoomPanel.SetEnabled(true);
        _createRoomPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _mainPanel.SetEnabled(false);

        _inRoomPanel.SetRoomName(PhotonNetwork.CurrentRoom.Name); 
        UpdateRoomControl();

        _playerListPanel.SetEnabled(true);
        var masterId = PhotonNetwork.CurrentRoom.Players[PhotonNetwork.CurrentRoom.masterClientId].UserId;
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
            _playerListPanel.AddPlayer(player.Value.UserId, player.Value.NickName,
                player.Value.UserId.Equals(masterId));
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)_createRoomPanel.MaxPlayersSlider.value;
        roomOptions.IsOpen = _createRoomPanel.IsOpenToggle.isOn;
        roomOptions.IsVisible = _createRoomPanel.IsVisibleToggle.isOn;
        roomOptions.PublishUserId = true;

        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        if (!string.IsNullOrWhiteSpace(_createRoomPanel.Input.text))
            enterRoomParams.RoomName = _createRoomPanel.Input.text;

        PhotonNetwork.CreateRoom(_createRoomPanel.Input.text, roomOptions);
    }

    private void JoinSelectedRoom()
    {
        if (string.IsNullOrEmpty(_joinRoomPanel.SelectedRoomName))
            return;

        JoinRoom(_joinRoomPanel.SelectedRoomName);
    }

    private void JoinRoomByName()
    {
        if (string.IsNullOrEmpty(_joinRoomPanel.Input.text))
            return;

        JoinRoom(_joinRoomPanel.Input.text);
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    private void SetRoomOpen(bool isOpen)
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsOpen = isOpen;
    }

    private void SetRoomVisible(bool isVisible)
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible = isVisible;
    }

    private void UpdateRoomControl()
    {
        var isMasterClient = PhotonNetwork.IsMasterClient;

        _inRoomPanel.IsOpenToggle.interactable = isMasterClient;
        _inRoomPanel.IsOpenToggle.isOn = PhotonNetwork.CurrentRoom.IsOpen;

        _inRoomPanel.IsVisibleToggle.interactable = isMasterClient;
        _inRoomPanel.IsVisibleToggle.isOn = PhotonNetwork.CurrentRoom.IsVisible;

        _inRoomPanel.StartGameButton.interactable = isMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >1;
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsOpen = false;

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(2);
    }

    #endregion


    #region IConnectionCallbacks

    public void OnConnected()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        if (FirebaseManager.Instance != null)
        {
            PhotonNetwork.NickName = FirebaseManager.Instance.UserProfileHandler.UserName;
        }
        else
        {
            PhotonNetwork.NickName = Random.Range(0, 10).ToString();
        }
        PhotonNetwork.JoinLobby(_customLobby);
        _roomsCanvas.enabled = true;
        ShowMainMenuPanel();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"OnDisconnected: {cause}");
        _logManager?.LogMessage($"OnDisconnected: {cause}");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        
    }

    #endregion


    #region ILobbyCallbacks

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    public void OnLeftLobby()
    {
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                _cachedRoomList.Remove(info.Name);
                _joinRoomPanel.RemoveRoom(info.Name);
            }
            else
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                    _cachedRoomList[info.Name] = info;
                else
                {
                    _cachedRoomList.Add(info.Name, info);
                    _joinRoomPanel.AddRoom(info.Name);
                }
                _joinRoomPanel.UpdateRoomStatus(info.Name, info.IsOpen, info.PlayerCount, info.MaxPlayers);
            }
        }
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        
    }

    #endregion


    #region IMatchmakingCallbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created");
        _logManager?.LogMessage("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create room failed: {message}");
        _logManager?.LogMessage($"Create room failed: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        _logManager?.LogMessage($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        ShowInRoomPanel();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join room failed: {message}");
        _logManager?.LogMessage($"Join room failed: {message}");
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        _logManager?.LogMessage($"Left room");
        ShowMainMenuPanel();
    }

    #endregion


    #region IInRoomCallbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} entered the room");
        _logManager?.LogMessage($"{newPlayer.NickName} entered the room");
        var masterId = PhotonNetwork.CurrentRoom.Players[PhotonNetwork.CurrentRoom.masterClientId].UserId;
        _playerListPanel.AddPlayer(newPlayer.UserId, newPlayer.NickName,
                newPlayer.UserId.Equals(masterId));
        UpdateRoomControl();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room");
        _logManager?.LogMessage($"{otherPlayer.NickName} left the room");
        _playerListPanel.RemovePlayer(otherPlayer.UserId);
        UpdateRoomControl();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        UpdateRoomControl();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"{newMasterClient.NickName} is now Master Client");
        _logManager?.LogMessage($"{newMasterClient.NickName} is now Master Client");
        UpdateRoomControl();
        var masterId = PhotonNetwork.CurrentRoom.Players[PhotonNetwork.CurrentRoom.masterClientId].UserId;
        _playerListPanel.UpdateMasterStatus(masterId);
    }

    #endregion
}
