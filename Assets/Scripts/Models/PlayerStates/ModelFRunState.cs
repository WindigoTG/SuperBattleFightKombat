using UnityEngine;

public class ModelFRunState : PlayerState
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
        _view.StartAnimation(AnimationTrack.Run);
        _isAttacking = false;
        _isLastAttackAnimationPrimary = false;
    }

    public override void Update(CurrentInputs inputs)
    {
        if (!_contactPoller.IsGrounded)
        {
            _model.SetState(CharacterState.Fall);
            return;
        }
        else
            _model.ReserGroundCoyoteTime();

        if (_isAttacking && _view.IsAnimationDone)
        {
            _isAttacking = false;

            if (inputs.Horisontal != 0)
                _view.StartAnimation(AnimationTrack.Run);
        }

        if (inputs.IsJumpPressed && !_isAttacking)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        Move(inputs.Horisontal);
    }

    private void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            if (!_isAttacking)
            {
                _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

                if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                    (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                    (inputHor != 0))
                    newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
            }
        }
        else
        {
            _model.SetState(CharacterState.Idle);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity + (_contactPoller.IsGrounded ? _contactPoller.GroundVelocity.x : 0));
    }

    public override void Attack()
    {
        var vertical = Input.GetAxisRaw("Vertical");

        if (vertical > 0)
        {
            if (!_model.Weapon.Attack(_view.GroundUpAttackOrigin.position, 0))
                return;

            _isAttacking = true;

            if (!_isLastAttackAnimationPrimary)
            {
                _view.StartAnimation(AnimationTrack.AttackStandUp);
                _isLastAttackAnimationPrimary = true;
            }
            else
            {
                _view.StartAnimation(AnimationTrack.AttackStandUpAlter);
                _isLastAttackAnimationPrimary = false;
            }
        }
        else
        {
            if (!_model.Weapon.Attack(_view.GroundStandAttackOrigin.position, _view.transform.localScale.x))
                return;

            _isAttacking = true;

            if (!_isLastAttackAnimationPrimary)
            {
                _view.StartAnimation(AnimationTrack.AttackStand);
                _isLastAttackAnimationPrimary = true;
            }
            else
            {
                _view.StartAnimation(AnimationTrack.AttackStandAlter);
                _isLastAttackAnimationPrimary = false;
            }
        }
    }

    #endregion
}
