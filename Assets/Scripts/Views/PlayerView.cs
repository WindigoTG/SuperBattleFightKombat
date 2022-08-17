using System;
using UnityEngine;

public class PlayerView : MonoBehaviour, IDamageable
{
    #region Fields 

    private SpriteAnimatorController _animatorController;

    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 7.5f;
    [Space]
    [SerializeField] private BoxCollider2D _regularCollider;
    [SerializeField] private BoxCollider2D _dashCollider;
    [SerializeField] private CompositeCollider2D _mainCollider;
    [Space]
    [Header("Attack Ogigins")]
    [SerializeField] private Transform _groundStandAttack;
    [SerializeField] private Transform _groundRunAttack;
    [SerializeField] private Transform _groundDashAttack;
    [SerializeField] private Transform _wallAttack;
    [SerializeField] private Transform _airAttack;

    #endregion


    #region Properties

    public Transform Transform => transform;
    public Rigidbody2D RigidBody => _rigidBody;
    public Collider2D Collider => _mainCollider;
    public bool IsAnimationDone => _animatorController.IsAnimationFinished(_spriteRenderer);

    Action<int> IDamageable.OnDamageTaken => throw new NotImplementedException();

    public Action<int> OnDamageTaken;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_spriteRenderer == null)
            GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _animatorController?.UpdateRegular();
    }

    #endregion



    #region Methods

    public void SetAnimatorController(SpriteAnimatorController animationController)
    {
        _animatorController = animationController;
    }

    public void StartIdleAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);
    }

    public void StartRunAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Run, true, _animationSpeed);
    }

    public void StartJumpAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Jump, false, _animationSpeed);
    }

    public void StartFallAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Fall, false, _animationSpeed);
    }

    public void StartHenshinAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Henshin, false, _animationSpeed);
    }

    public void StartWallClingAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.WallCling, false, _animationSpeed);
    }

    public void StartHurtAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.TakeHit, false, _animationSpeed);
    }

    public void StartShootStandAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStand, false, _animationSpeed);
    }

    public void StartShootRunAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackRun, true, _animationSpeed);
    }

    public void StartShootJumpAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackJump, false, _animationSpeed);
    }

    public void Activate()
    {
        _spriteRenderer.enabled = true;
    }

    public void Deactivate()
    {
        _animatorController.StopAnimation(_spriteRenderer);
        _spriteRenderer.enabled = false;
    }

    #endregion


    #region IDamageable

    public void TakeDamage(int damage)
    {
        OnDamageTaken?.Invoke(damage);
    }

    #endregion
}

