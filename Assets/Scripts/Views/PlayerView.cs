using System;
using UnityEngine;

public class PlayerView : MonoBehaviour, IDamageable
{
    #region Fields 

    private SpriteAnimatorController _animatorController;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 7.5f;

    #endregion


    #region Properties

    public Transform Transform => transform;
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

