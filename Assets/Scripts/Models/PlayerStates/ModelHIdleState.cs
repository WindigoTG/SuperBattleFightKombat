using UnityEngine;


public class ModelHIdleState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private bool _isAttacking;
    private bool _isLastAttackAnimationPrimary;

    private int _lastFrame;
    private int _frameCount;
    private static int _attackFrameCountToAct = 3;
    private static int _attackFrameCountToFollowUp = 1;

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
        _model.SetHasAirDashed(false);
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

        if (_isAttacking)
        {
            if (_view.CurrentFrame != _lastFrame)
            {
                _lastFrame = _view.CurrentFrame;
                _frameCount++;
            }

            if (_view.IsAnimationDone)
            {
                _isAttacking = false;
                _view.StartAnimation(AnimationTrack.Idle);
            }
        }

        if (inputs.IsJumpPressed && (!_isAttacking || _frameCount > _attackFrameCountToAct))
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        if (Mathf.Abs(inputs.Horisontal) > References.InputThreshold && (!_isAttacking || _frameCount > _attackFrameCountToAct))
        {
            _model.SetState(CharacterState.Run);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: (_contactPoller.GroundVelocity.x != 0 && _contactPoller.IsGrounded) ?
                                                                        _contactPoller.GroundVelocity.x : 0);
    }

    public override void Attack()
    {
        if (!_isAttacking)
        {
            if (!_model.Weapon.Attack(_view.GroundStandAttackOrigin.position, _view.transform.localScale.x))
                return;

            _isAttacking = true;
            _view.StartAnimation(AnimationTrack.AttackStand);
            _isLastAttackAnimationPrimary = true;
            _frameCount = 0;
            _lastFrame = _view.CurrentFrame;
        }
        else if (_isLastAttackAnimationPrimary && _frameCount > _attackFrameCountToFollowUp)
        {
            if (!_model.Weapon.Attack(_view.GroundStandAttackOrigin.position, _view.transform.localScale.x, attackIndex: 1))
                return;

            _isAttacking = true;

            _view.StartAnimation(AnimationTrack.AttackStandAlter);
            _isLastAttackAnimationPrimary = false;
            _frameCount = 0;
            _lastFrame = _view.CurrentFrame;
        }
    }

    #endregion
}
