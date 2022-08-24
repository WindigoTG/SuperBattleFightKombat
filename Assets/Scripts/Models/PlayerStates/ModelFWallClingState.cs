using UnityEngine;

public class ModelFWallClingState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    bool _isAttacking;

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
        _view.StartAnimation(AnimationTrack.WallCling);
        _isAttacking = false;
    }

    public override void Update(CurrentInputs inputs)
    {
        _model.ResetWallCoyoteTime();

        if (_isAttacking && _view.IsAnimationDone)
            _isAttacking = false;

        if (inputs.IsJumpPressed && !_isAttacking)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        var horisontal = inputs.Horisontal;

        if (_contactPoller.IsGrounded)
        {
            if (Mathf.Abs(horisontal) > References.InputThreshold)
                _model.SetState(CharacterState.Run);
            else
                _model.SetState(CharacterState.Idle);

            return;
        }

        Move(horisontal);
        if (!(Mathf.Abs(horisontal) > References.InputThreshold) ||
            (horisontal > 0 && !_contactPoller.HasRightContacts) ||
            (horisontal < 0 && !_contactPoller.HasLeftContacts))
            _model.SetState(CharacterState.Fall);
    }

    public void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && _contactPoller.HasRightContacts) ||
                (inputHor < 0 && _contactPoller.HasLeftContacts))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity);

        if (newVelocity == 0)
            _model.SetState(CharacterState.Fall);
    }

    public override void Attack()
    {
        if (_isAttacking && !_view.IsAnimationDone)
            return;

        if (!_model.Weapon.Attack(_view.WallAttackOrigin.position, -_view.transform.localScale.x))
            return;

        _isAttacking = true;
        _view.StartAnimation(AnimationTrack.AttakWallCling);
    }

    #endregion
}
