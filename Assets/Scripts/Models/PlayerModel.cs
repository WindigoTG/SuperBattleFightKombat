using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    #region Fields

    PlayerView _view;

    private readonly ContactsPoller _contactsPoller;

    private PlayerHealth _health;
    private PlayerWeapon _weapon;

    private float _defaultSpeed = 200f;
    private float _currentSpeed;

    private float _jumpForce = 350.0f;
    private float _wallJumpForceMultiplier = 1;
    private const float COYOTE_TIME = 0.2f;

    private float _currentGroundCoyoteTime;
    private float _currentWallCoyoteTime;

    bool _isReady;

    bool _isDead;

    Vector3 _lastCheckPoint;

    public event System.Action End;

    private Dictionary<CharacterState, PlayerState> _playerStates;

    private PlayerState _activetState;
    private CharacterState _currentState;
    private CharacterState _previousState;

    public Action OnReady;
    public Action<string> OnDeath;

    #endregion


    #region Properties

    public Transform Transform => _view.transform;

    public bool IsGroundCoyoteTime => _currentGroundCoyoteTime > 0;

    public bool IsWallCoyoteTime => _currentWallCoyoteTime > 0;

    public float CurrentSpeed => _currentSpeed;

    public CharacterState PreviousState => _previousState;

    public float JumpForce => _jumpForce;

    public float WallJumpForceMultiplier => _wallJumpForceMultiplier;

    public PlayerWeapon Weapon => _weapon;

    #endregion


    #region Class Life Cycle

    public PlayerModel(PlayerView view, Dictionary<CharacterState, PlayerState> playerStates, PlayerWeapon weapon)
    {
        _view = view;
        _playerStates = playerStates;
        _weapon = weapon;

        _health = new PlayerHealth();
        _health.Damage += TakeHit;
        _health.Death += Die;

        _view.OnDamageTaken += _health.TakeDamage;

        _contactsPoller = new ContactsPoller(view.Collider);

        _currentSpeed = _defaultSpeed;

        foreach (var kvp in _playerStates)
            kvp.Value.SetDependencies(this, _view, _contactsPoller);
    }

    #endregion


    #region IUpdateableRegular

    public void Update(CurrentInputs inputs)
    {
        _contactsPoller.UpdateRegular();

        if (_currentGroundCoyoteTime > 0)
            _currentGroundCoyoteTime -= Time.deltaTime;

        if (_currentWallCoyoteTime > 0)
            _currentWallCoyoteTime -= Time.deltaTime;

        _activetState.Update(inputs);

        if (inputs.IsAttackPressed)
            Shoot();
    }

    #endregion


    #region Methods

    public void SetState(CharacterState state)
    {
        if (!_playerStates.ContainsKey(state))
            return;

        _previousState = _currentState;
        _currentState = state;
        _activetState = _playerStates[state];
        _activetState.Activate();

        if (state == CharacterState.Idle && !_isReady)
        {
            _isReady = true;
            OnReady?.Invoke();
        }
    }

    public void ReserGroundCoyoteTime() => _currentGroundCoyoteTime = COYOTE_TIME;

    public void ResetWallCoyoteTime() => _currentWallCoyoteTime = COYOTE_TIME;

    public void StartAtPosition(Vector3 position)
    {
        _lastCheckPoint = position;

        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _view.transform.position = new Vector3(position.x, hit.point.y + 1.0f, 0);

        _view.Activate();

        SetState(CharacterState.Idle);

        if (_isDead)
        {
            _health.Reset();
            _isDead = false;
        }
    }

    public void NonHenshinStartAtPosition(Vector3 position)
    {
        _lastCheckPoint = position;

        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _view.transform.position = new Vector3(position.x, hit.point.y + 1f, 0);

        _view.Activate();

        SetState(CharacterState.IdlePreHenshin);
        _health.SetPlayerView(_view);
    }

    private void ResetState()
    {
        _currentSpeed = _defaultSpeed;
        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: 0.0f, y: 0.0f);
    }

    private void TakeHit()
    {
        SetState(CharacterState.Hurt);
        _view.StartBlinking(1f);
        _view.PlaySound(References.HURT_SOUND);
    }

    private void Die(string attackerID)
    {
        _isDead = true;

        _view.PlaySound(References.DEATH_SOUND);

        SetState(CharacterState.Death);
        _view.SetUiEnabled(false);

        OnDeath?.Invoke(attackerID);
    }

    public void Respawn()
    {
        ResetState();
        StartAtPosition(_lastCheckPoint);
        _view.Activate();
        _view.SetUiEnabled(true);
    }

    public void Shoot()
    {
        _activetState.Attack();
        //_weapon.Shoot(_view.GroundStandAttakOrigin.position, _view.transform.localScale.x);

        //if (_isReady && !_isShooting)
        //{
            //if (!_isGrounded)
            //    _view.StartShootJumpAnimation();
            //else if (_isMoving)
            //    _view.StartShootRunAnimation();
            //else
            //    _view.StartShootStandAnimation();

            //_weapon.Shoot(_player.transform.localScale.x);

        //    _isShooting = true;
        //    _currentShootingCD = _shootingCD;
        //}
    }

    #endregion
}
