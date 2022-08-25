using UnityEngine;

public class ModelFIdleState : PlayerState
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
        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: 0.0f);
        _view.StartAnimation(AnimationTrack.Idle);
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
            _view.StartAnimation(AnimationTrack.Idle);
        }

        if (inputs.IsJumpPressed && !_isAttacking)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        if (Mathf.Abs(inputs.Horisontal) > References.InputThreshold && !_isAttacking)
        {
            _model.SetState(CharacterState.Run);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: (_contactPoller.GroundVelocity.x != 0 && _contactPoller.IsGrounded) ?
                                                                        _contactPoller.GroundVelocity.x : 0);
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