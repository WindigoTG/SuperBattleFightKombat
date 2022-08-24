using UnityEngine;

public class ModelFFallState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private bool _isAttacking;
    private bool _isLastAttackAnimationPrimary;

    #endregion


    #region Methods

    public override void SetDependencies(PlayerModel model, PlayerView view, ContactsPoller contactPoller)
    {
        _model = model;
        _view = view;
        _contactPoller = contactPoller;
    }

    public override void Activate()
    {
        if (_isAttacking && _view.IsAnimationDone)
            _isAttacking = false;

        _view.StartAnimation(AnimationTrack.Fall);

        _isAttacking = false;
        _isLastAttackAnimationPrimary = false;
    }

    public override void Update(CurrentInputs inputs)
    {
        if (inputs.IsJumpPressed && (_model.IsGroundCoyoteTime || _model.IsWallCoyoteTime) && !_isAttacking)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        var horisontal = inputs.Horisontal;
        Move(horisontal);
        VerticalCheck(horisontal);
    }

    public void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            if (!_isAttacking)
                _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                (inputHor != 0))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);

            WallCheck(inputHor);
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity);
    }

    private void VerticalCheck(float inputHor)
    {
        if (_contactPoller.IsGrounded)
        {
            if (Mathf.Abs(inputHor) > References.InputThreshold)
                _model.SetState(CharacterState.Run);
            else
                _model.SetState(CharacterState.Idle);
        }
    }

    private void WallCheck(float inputHor)
    {
        if ((inputHor > 0 && _contactPoller.HasRightContacts) ||
            (inputHor < 0 && _contactPoller.HasLeftContacts))
        {
            _model.SetState(CharacterState.WallCling);
            return;
        }
    }

    public override void Attack()
    {
        if (!_model.Weapon.Attack(_view.AirAttackOrigin.position, _view.transform.localScale.x))
            return;

        _isAttacking = true;

        if (!_isLastAttackAnimationPrimary)
        {
            _view.StartAnimation(AnimationTrack.AttackJump);
            _isLastAttackAnimationPrimary = true;
        }
        else
        {
            _view.StartAnimation(AnimationTrack.AttackJumpAlter);
            _isLastAttackAnimationPrimary = false;
        }
    }

    #endregion
}
