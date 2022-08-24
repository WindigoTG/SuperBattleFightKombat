using UnityEngine;

public class ModelFJumpState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private float _buffer;

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
        _buffer = 0.1f;

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(y: 0f);

        Vector2 horisontalForce = Vector2.zero;

        if (_model.PreviousState == CharacterState.WallCling)
            horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * -_view.RigidBody.transform.localScale.x;
        else if (_model.IsWallCoyoteTime)
        {
            var horisontalInput = Input.GetAxisRaw("Horizontal");

            if (horisontalInput < 0 && _contactPoller.HasLeftContacts)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * 1;
            else if (horisontalInput > 0 && _contactPoller.HasRightContacts)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * -1;
            else if (horisontalInput < 0)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * -1;
            else if (horisontalInput > 0)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * 1;
            else if (_contactPoller.HasLeftContacts && !_contactPoller.HasRightContacts)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * 1;
            else if (_contactPoller.HasRightContacts && !_contactPoller.HasLeftContacts)
                horisontalForce = Vector2.right * _model.JumpForce * _model.WallJumpForceMultiplier * -1;
        }

        _view.RigidBody.AddForce((Vector2.up * _model.JumpForce) + horisontalForce);
        _view.StartAnimation(AnimationTrack.Jump);

        _isAttacking = false;
        _isLastAttackAnimationPrimary = false;
    }

    public override void Update(CurrentInputs inputs)
    {
        if (_isAttacking && _view.IsAnimationDone)
            _isAttacking = false;

        var horisontal = inputs.Horisontal;
        Move(horisontal);
        VerticalCheck(horisontal);

        if (_buffer > 0)
            _buffer -= Time.deltaTime;
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
        if (_view.RigidBody.velocity.y < References.FallThreshold && !_isAttacking)
        {
            _model.SetState(CharacterState.Fall);
            return;
        }

        if (_contactPoller.IsGrounded && _buffer <= 0)
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
