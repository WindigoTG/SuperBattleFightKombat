using UnityEngine;

public class IdleState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private bool _isAttacking;

    #endregion


    #region Methods

    public override void SetDependencies(PlayerModel model, PlayerView view, ContactsPoller contactPoller)
    {
        _model = model;
        _view = view;
        _contactPoller = contactPoller;
        _isAttacking = false;
    }

    public override void Activate()
    {
        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: 0.0f);
        _view.StartAnimation(AnimationTrack.Idle);
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

        if (inputs.IsJumpPressed)
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        if (Mathf.Abs(inputs.Horisontal) > References.InputThreshold)
        {
            _model.SetState(CharacterState.Run);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: (_contactPoller.GroundVelocity.x != 0 && _contactPoller.IsGrounded) ?
                                                                        _contactPoller.GroundVelocity.x : 0);

        if (_isAttacking && _view.IsAnimationDone)
        {
            _isAttacking = false;
            _view.StartAnimation(AnimationTrack.Idle);
        }
    }

    public override void Attack()
    {
        if (!_model.Weapon.Shoot(_view.GroundStandAttackOrigin.position, _view.transform.localScale.x))
            return;

        _isAttacking = true;
        _view.StartAnimation(AnimationTrack.AttackStand);
    }

    #endregion
}
