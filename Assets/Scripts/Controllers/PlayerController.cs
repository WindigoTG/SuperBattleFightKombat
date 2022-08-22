using Photon.Pun;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Fields

    PlayerModel _player;
    PlayerFactory _playerfactory;

    private bool _isJumpPressed;
    private bool _isAttackPressed;
    private float _horisontalInput;

    private bool _isReady;

    public Action OnReady;
    public Action<string> OnDeath;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _playerfactory = new PlayerFactory();
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;

        _player = _playerfactory.CreatePlayer();

        _player.NonHenshinStartAtPosition(new Vector3(Random.Range(-2f,2f), 0, 0));
        _player.OnDeath += OnPlayerDeath;
    }

    public void Update()
    {
        if (photonView.IsMine)
        {
            if (_isReady)
            {
                _horisontalInput = Input.GetAxisRaw("Horizontal");
                _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
                _isAttackPressed = Input.GetMouseButtonDown(0);
            }

            _player.Update(new CurrentInputs
            {
                Horisontal = _horisontalInput,
                IsJumpPressed = _isJumpPressed,
                IsAttackPressed = _isAttackPressed
            });
        }

        _horisontalInput = default;
        _isJumpPressed = default;
        _isAttackPressed = default;
    }

    #endregion


    #region Methods

    private void OnHenshinFinished()
    {
        OnReady?.Invoke();
        _player.OnReady -= OnHenshinFinished;
    }

    private void OnPlayerDeath(string attackerID)
    {
        OnDeath?.Invoke(attackerID);
    }

    public void StartHenshin()
    {
        _player.OnReady += OnHenshinFinished;
        _player.SetState(CharacterState.Henshin);
    }

    public void StartGame()
    {
        _isReady = true;
    }

    public void RespawnPlayer() => _player.Respawn();

    #endregion


    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    #endregion
}


public struct CurrentInputs
{
    public float Horisontal;
    public bool IsJumpPressed;
    public bool IsAttackPressed;
}

