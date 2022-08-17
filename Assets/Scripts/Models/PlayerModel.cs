using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : IUpdateableRegular
{
    #region Fields

    PlayerView _view;

    private readonly ContactsPoller _contactsPoller;

    private PlayerHealth _health;
    private PlayerWeapon _weapon;

    private float _defaultSpeed = 200f;
    private float _currentSpeed;

    private const float INPUT_THRESHOLD = 0.1f;
    private const float _FALL_THRESHOLD = -1f;
    private float _jumpForce = 350.0f;
    private float _wallJumpForceMultiplier = 1;
    private const float COYOTE_TIME = 0.2f;

    private float _currentGroundCoyoteTime;
    private float _currentWallCoyoteTime;

    private Vector3 _leftScale = new Vector3(-1, 1, 1);
    private Vector3 _rightScale = new Vector3(1, 1, 1);

    bool _isReady;

    bool _isDead;
    
    bool _isShooting;

    float _shootingCD = 0.3f;
    float _currentShootingCD;

    ContactPoint2D[] _contacts = new ContactPoint2D[16];
    Vector3 _destination;
    Vector3 _lastCheckPoint;

    public event System.Action End;

    private Dictionary<CharacterState, PlayerState> _playerStates;

    private PlayerState _activetState;
    private CharacterState _currentState;
    private CharacterState _previousState;

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

    public void UpdateRegular()
    {
        _contactsPoller.UpdateRegular();

        if (_currentGroundCoyoteTime > 0)
            _currentGroundCoyoteTime -= Time.deltaTime;

        if (_currentWallCoyoteTime > 0)
            _currentWallCoyoteTime -= Time.deltaTime;

        _activetState.UpdateRegular();

        if (Input.GetMouseButtonDown(0))
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
    }

    public void ReserGroundCoyoteTime() => _currentGroundCoyoteTime = COYOTE_TIME;

    public void ResetWallCoyoteTime() => _currentWallCoyoteTime = COYOTE_TIME;

    public void StartAtPosition(Vector3 position)
    {
        _lastCheckPoint = position;

        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _view.transform.position = new Vector3(position.x, hit.collider.bounds.max.y, 0);

        _view.Activate();

        _isReady = true;
        SetState(CharacterState.Idle);

        if (_isDead)
        {
            _health.Reset();
            _isDead = false;
        }
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
    }

    private void Die()
    {
        //if (_isReady)
        //{
        //    _isReady = false;
        //    _isDead = true;
        //    ResetState();
        //    StartAtPosition(_lastCheckPoint);
        //}

        _isReady = false;
        _isDead = true;

        if (_contactsPoller.IsGrounded)
            SetState(CharacterState.Death);
        else
            Respawn();
    }

    public void Respawn()
    {
        ResetState();
        StartAtPosition(_lastCheckPoint);
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
