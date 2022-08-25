using UnityEngine;

public class FallState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private bool _isBoosted;
    private float _boostDirection;
    private float _boostSpeedModifier = 1.75f;

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
        _view.StartAnimation(AnimationTrack.Fall);

        if (_model.PreviousState == CharacterState.GroundDash)
        {
            _isBoosted = true;
            _boostDirection = _view.RigidBody.velocity.x;
        }
    }

    public override void Update(CurrentInputs inputs)
    {
        if (inputs.IsJumpPressed && (_model.IsGroundCoyoteTime || _model.IsWallCoyoteTime))
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
        if (_isBoosted)
        {
            if (inputHor == 0 || _boostDirection > 0 && inputHor < 0 || _boostDirection < 0 && inputHor > 0)
                _isBoosted = false;
        }

        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                (inputHor != 0))
            {
                if (_isBoosted)
                    newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * _boostSpeedModifier * (inputHor < 0 ? -1 : 1);
                else
                    newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
            }

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

        _view.StartAnimation(AnimationTrack.AttackJump);
    }

    #endregion
}
