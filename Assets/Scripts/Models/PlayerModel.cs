using UnityEngine;

public class PlayerModel : IUpdateableRegular
{
    #region Fields

    PlayerView _currentForm;
    PlayerView _transformedForm;

    Rigidbody2D _player;
    BoxCollider2D _collider;
    BoxCollider2D _colliderStand;

    private readonly ContactsPoller _contactsPoller;

    private PlayerHealth _health;
    //private PlayerWeapon _weapon;

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
    bool _isMoving;
    bool _isGrounded;
    bool _isJumping;
    bool _isFalling;
    bool _isWallClinging;
    bool _isHurt;
    bool _isDead;
    
    bool _isShooting;

    float _shootingCD = 0.3f;
    float _currentShootingCD;

    Vector2 _raycastStart;
    Vector2 _raycastEnd;
    Vector2 _raycastDirection;
    private float _heightDifference;

    ContactPoint2D[] _contacts = new ContactPoint2D[16];
    Vector3 _destination;
    Vector3 _lastCheckPoint;

    public event System.Action End;

    #endregion


    #region Properties

    public Transform Transform => _player.transform;

    #endregion


    #region Class Life Cycle

    public PlayerModel(GameObject player, PlayerView transformedForm)
    {
        _transformedForm = transformedForm;
        _currentForm = _transformedForm;

        _player = player.GetComponent<Rigidbody2D>();
        var colliders = player.GetComponents<BoxCollider2D>();
        _colliderStand = colliders[0];
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].offset.y > _colliderStand.offset.y)
                _colliderStand = colliders[i];
        }
        _collider = player.GetComponent<BoxCollider2D>();

        _heightDifference = _collider.bounds.max.y + _collider.edgeRadius - _player.transform.position.y;

        _transformedForm.Transform.parent = player.transform;
        _transformedForm.Transform.localPosition = Vector3.zero;

        _health = new PlayerHealth();
        _health.Damage += TakeHit;
        _health.Death += Die;

        _transformedForm.OnDamageTaken += _health.TakeDamage;

        _contactsPoller = new ContactsPoller(_collider);

        //_weapon = new PlayerWeapon(player.GetComponentInChildren<Grid>().transform);

        _currentSpeed = _defaultSpeed;
        _isGrounded = true;
    }

    #endregion


    #region IUpdateableRegular

    public void UpdateRegular()
    {
        _contactsPoller.UpdateRegular();

        if (_currentGroundCoyoteTime > 0 && !_isGrounded)
            _currentGroundCoyoteTime -= Time.deltaTime;

        if (_currentWallCoyoteTime > 0 && !_isWallClinging)
            _currentWallCoyoteTime -= Time.deltaTime;

        if (_isShooting)
        {
            if (_currentShootingCD > 0)
                _currentShootingCD -= Time.deltaTime;
            else
            {
                _isShooting = false;

                if (!_isGrounded)
                    _currentForm.StartFallAnimation();
                else if (_isMoving)
                    _currentForm.StartRunAnimation();
                else
                    _currentForm.StartIdleAnimation();
            }
        }

        if (_isHurt && _currentForm.IsAnimationDone)
        {
            Recover();
        }

        VerticalCheck();
    }

    #endregion


    #region Methods

    public void StartAtPosition(Vector3 position)
    {
        _lastCheckPoint = position;

        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _player.transform.position = new Vector3(position.x, hit.collider.bounds.max.y, 0);

        _currentForm.Activate();

        _isReady = true;
        _currentForm.StartIdleAnimation();

        if (_isDead)
        {
            _health.Reset();
            _isDead = false;
        }
    }

    private bool CheckForCeiling()
    {
        _raycastStart = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y);
        _raycastEnd = new Vector2(_collider.bounds.center.x, _player.transform.position.y + _heightDifference);
        _raycastDirection = _raycastEnd - _raycastStart;
        RaycastHit2D hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
        Debug.DrawLine(_raycastStart, _raycastEnd, Color.red);

        if (hit.collider == null)
        {
            _raycastStart = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
            _raycastEnd = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y + _heightDifference);
            _raycastDirection = _raycastEnd - _raycastStart;
            hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
            Debug.DrawLine(_raycastStart, _raycastEnd, Color.blue);

            if (hit.collider == null)
            {
                _raycastStart = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);
                _raycastEnd = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y + _heightDifference);
                _raycastDirection = _raycastEnd - _raycastStart;
                hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
                Debug.DrawLine(_raycastStart, _raycastEnd, Color.green);
            }
        }

        return hit.collider != null;
    }


    public void Move(float inputHor)
    {
        if (_isReady)
        {
            var newVelocity = 0f;

            if (_isGrounded)
                _currentGroundCoyoteTime = COYOTE_TIME;

            if (Mathf.Abs(inputHor) > INPUT_THRESHOLD)
            {
                _player.transform.localScale = (inputHor < 0 ? _leftScale : _rightScale);

                if ((inputHor > 0 && !_contactsPoller.HasRightContacts && _isGrounded) ||
                    (inputHor < 0 && !_contactsPoller.HasLeftContacts && _isGrounded) ||
                    (inputHor != 0 && !_isGrounded))
                    newVelocity = Time.fixedDeltaTime * _currentSpeed * (inputHor < 0 ? -1 : 1);

                if (!_isMoving && _isGrounded)
                {
                    _currentForm.StartRunAnimation();
                }

                if (!_isGrounded &&
                    ((inputHor > 0 && _contactsPoller.HasRightContacts) ||
                    (inputHor < 0 && _contactsPoller.HasLeftContacts))
                    && !_isWallClinging)
                {
                        StartgWallCling();
                }

                _isMoving = true;
            }
            else
            {
                if (_isWallClinging)
                    StopWallCling();

                if (_isMoving && _isGrounded)
                {
                    _currentForm.StartIdleAnimation();
                }

                _isMoving = false;
            }

            _player.velocity = _player.velocity.Change(x: newVelocity + (_contactsPoller.IsGrounded ? _contactsPoller.GroundVelocity.x : 0)); 
            //if (_isGrounded)
            //    _player.velocity = _player.velocity.Change(y: _contactsPoller.GroundVelocity.y);
        }
    }

    private void VerticalCheck()
    {
        if (_isReady)
        {
            if (_player.velocity.y < _FALL_THRESHOLD && ((!_isFalling) ||
                (_isGrounded && !_contactsPoller.IsGrounded)) && !_isWallClinging)
            {
                Fall();
            }

            if ((_isFalling || _isWallClinging) && _contactsPoller.IsGrounded)
            {
                Land();
            }
        }
    }

    private void StartgWallCling()
    {
        _isWallClinging = true;
        _currentWallCoyoteTime = COYOTE_TIME;
        if (_player.velocity.y < _FALL_THRESHOLD)
            _player.velocity = _player.velocity.Change(y: 0f);
        _currentForm.StartWallClingAnimation();
    }

    private void StopWallCling()
    {
        _isWallClinging = false;
        if (_isGrounded)
            Land();
        else
            Fall();
    }

    private void Fall()
    {
        _isGrounded = false;
        _isJumping = false;
        _isFalling = true;
        _currentForm.StartFallAnimation();
    }

    private void Land()
    {
        _isGrounded = true;
        _isFalling = false;
        if (_isMoving)
            _currentForm.StartRunAnimation();
        else
            _currentForm.StartIdleAnimation();
    }

    public void Jump()
    {
        if (_isReady)
        {
            if (_isWallClinging || _currentWallCoyoteTime > 0)
            {
                _player.velocity = _player.velocity.Change(y: 0f);
                _isWallClinging = false;
                _player.AddForce((Vector2.up * _jumpForce) + (Vector2.right * _jumpForce * _wallJumpForceMultiplier * -_player.transform.localScale.x));
                _currentForm.StartJumpAnimation();
            } else if (_contactsPoller.IsGrounded || _currentGroundCoyoteTime > 0)
            {
                _isGrounded = false;
                _isJumping = true;
                _player.AddForce(Vector2.up * _jumpForce);
                _currentForm.StartJumpAnimation();
                ResetState();
            }
        }
    }

    private void ResetState()
    {
        _isMoving = false;
        _currentSpeed = _defaultSpeed;
        _player.velocity = _player.velocity.Change(x: 0.0f, y: 0.0f);
    }

    private void TakeHit()
    {
        if (_isReady)
        {
            _isHurt = true;
            _isReady = false;
            _isShooting = false;

            _currentForm.StartHurtAnimation();
            ResetState();
        }
    }

    private void Die()
    {
        if (_isReady)
        {
            _isReady = false;
            _isDead = true;
            ResetState();
            StartAtPosition(_lastCheckPoint);
        }
    }

    private void Recover()
    {
        _isReady = true;
        _isHurt = false;
        if (!_isGrounded)
            _currentForm.StartFallAnimation();
        else if (_isMoving)
            _currentForm.StartRunAnimation();
        else
            _currentForm.StartIdleAnimation();
    }

    public void Shoot()
    {
        if (_isReady && !_isShooting && !_isWallClinging)
        {
            if (!_isGrounded)
                _currentForm.StartShootJumpAnimation();
            else if (_isMoving)
                _currentForm.StartShootRunAnimation();
            else
                _currentForm.StartShootStandAnimation();

            //_weapon.Shoot(_player.transform.localScale.x);

            _isShooting = true;
            _currentShootingCD = _shootingCD;
        }
    }

    #endregion
}
