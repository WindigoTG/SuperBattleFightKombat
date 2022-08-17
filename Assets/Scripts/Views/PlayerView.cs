using System;
using System.Collections;
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

    public Action<int> OnDamageTaken;

    Coroutine _blinking;

    #endregion


    #region Properties

    public Rigidbody2D RigidBody => _rigidBody;
    public Collider2D Collider => _mainCollider;
    public bool IsAnimationDone => _animatorController.IsAnimationFinished(_spriteRenderer);
    public int CurrentFrame => _animatorController.GetCurrentFrame(_spriteRenderer);
    public Transform GroundStandAttackOrigin => _groundStandAttack;
    public Transform GroundRunAttackOrigin => _groundRunAttack;
    public Transform GroundDashAttackOrigin => _groundDashAttack;
    public Transform WallAttackOrigin => _wallAttack;
    public Transform AirAttackOrigin => _airAttack;
    

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
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStand, false, _animationSpeed, true);
    }

    public void StartShootRunAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackRun, true, _animationSpeed);
    }

    public void StartShootJumpAnimation()
    {
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackJump, false, _animationSpeed, true);
    }

    public void StartShootWallClingAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttakWallCling, false, _animationSpeed, true);
    }

    public void StartDeathAnimation()
    {
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Death, false, _animationSpeed);
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

    public void StartBlinking(float duration)
    {
        if (_blinking != null)
            StopCoroutine(_blinking);

        _blinking = StartCoroutine(Blinking(duration));
    }

    private IEnumerator Blinking(float duration)
    {
        var color = _spriteRenderer.color;

        while (duration > 0)
        {
            color.a = color.a == 1f ? 0.5f : 1f;
            _spriteRenderer.color = color;
            duration -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        color.a = 1;
        _spriteRenderer.color = color;
    }

    #endregion


    #region IDamageable

    public void TakeDamage(int damage)
    {
        OnDamageTaken?.Invoke(damage);
    }

    #endregion
}

