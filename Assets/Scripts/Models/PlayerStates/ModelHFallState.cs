using UnityEngine;

public class ModelHFallState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private bool _isAttacking;

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

        _isAttacking = false;

        if (_model.PreviousState == CharacterState.GroundDash || _model.PreviousState == CharacterState.AirDash)
        {
            _isBoosted = true;
            _boostDirection = _view.RigidBody.velocity.x;
        }
    }

    public override void Update(CurrentInputs inputs)
    {
        if (inputs.IsJumpPressed && (_model.IsGroundCoyoteTime || _model.IsWallCoyoteTime) && !_isAttacking)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        if (_isAttacking && _view.IsAnimationDone)
        {
            _isAttacking = false;
            _view.StartAnimation(AnimationTrack.Fall);
        }

        var horisontal = inputs.Horisontal;
        Move(horisontal);
        VerticalCheck(horisontal);

        if (inputs.IsDashPressed && !_model.HasAirDashed)
        {
            if (inputs.Vertical > 0)
                _model.SetState(CharacterState.AirUpDash);
            else if (inputs.Horisontal != 0)
                _model.SetState(CharacterState.AirDash);
        }
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
            if (!_isAttacking)
                _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                (inputHor != 0))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
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
        if (!_isAttacking)
        {
            if (!_model.Weapon.Attack(_view.GroundStandAttackOrigin.position, _view.transform.localScale.x, attackIndex: 2))
                return;

            _isAttacking = true;
            _view.StartAnimation(AnimationTrack.AttackJump);
        }
    }

    #endregion
}