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

    public void Spawn(Character character)
    {
        _player = _playerfactory.CreatePlayer(character);

        _player.NonHenshinStartAtPosition(new Vector3(Random.Range(-3f, 3f), 0, 0));
        _player.OnDeath += OnPlayerDeath;
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
    public float Vertical;
    public bool IsJumpPressed;
    public bool IsAttackPressed;
    public bool IsDashPressed;
}

