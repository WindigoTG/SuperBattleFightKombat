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
    private float _verticalInput;
    private bool _isDashPressed;

    private bool _isReady;

    public Action OnReady;
    public Action<string> OnDeath;
    public Action OnLifePickup;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _playerfactory = new PlayerFactory();
    }

    public void Update()
    {
        if (photonView.IsMine)
        {
            if (_player == null)
                return;

            if (_isReady)
            {
                _horisontalInput = Input.GetAxisRaw("Horizontal");
                _verticalInput = Input.GetAxisRaw("Vertical");
                _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
                _isAttackPressed = Input.GetMouseButtonDown(0);
                _isDashPressed = Input.GetKeyDown(KeyCode.LeftShift);
            }

            _player.Update(new CurrentInputs
            {
                Horisontal = _horisontalInput,
                IsJumpPressed = _isJumpPressed,
                IsAttackPressed = _isAttackPressed,
                Vertical = _verticalInput,
                IsDashPressed = _isDashPressed
            }); ;
        }

        _horisontalInput = default;
        _verticalInput = default;
        _isJumpPressed = default;
        _isAttackPressed = default;
        _isDashPressed = default;
    }

    #endregion


    #region Methods

    public void Spawn(Character character, Vector3 position)
    {
        _player = _playerfactory.CreatePlayer(character);

        _player.NonHenshinStartAtPosition(position);
        _player.OnDeath += OnPlayerDeath;
        _player.OnLifePickup += LifePickup;
    }

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

    public void StopGame()
    {
        _isReady = false;
        if (_player != null)
        {
            _player.OnDeath -= OnPlayerDeath;
            _player.OnLifePickup -= LifePickup;
            _player.Dispose();
        }
        _player = null;
    }

    public void RespawnPlayer() => _player.Respawn();
    public void RespawnPlayerAtPosition(Vector3 position) => _player.RespawnAtPosition(position);

    private void LifePickup() => OnLifePickup?.Invoke();

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
    public float Vertical;
    public bool IsJumpPressed;
    public bool IsAttackPressed;
    public bool IsDashPressed;
}

