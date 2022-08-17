using UnityEngine;

public class RunState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private int _lastFrame;
    private int _frameCount;
    private bool _isAttacking;
    private static int _attackFrameCount = 5;

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
        _view.StartRunAnimation();
        _isAttacking = false;
    }

    public override void UpdateRegular()
    {
        if (!_contactPoller.IsGrounded)
        {
            _model.SetState(CharacterState.Fall);
            return;
        }
        else
            _model.ReserGroundCoyoteTime();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        var horisontal = Input.GetAxisRaw("Horizontal");
        Move(horisontal);

        if (_isAttacking)
        {
            if (_view.CurrentFrame != _lastFrame)
            {
                _lastFrame = _view.CurrentFrame;
                _frameCount++;
            }

            if (_frameCount > _attackFrameCount)
            {
                _view.StartRunAnimation();
                _isAttacking = false;
            }
        }
    }

    private void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                (inputHor != 0))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
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
        if (!_model.Weapon.Shoot(_view.GroundRunAttackOrigin.position, _view.transform.localScale.x))
            return;

        _view.StartShootRunAnimation();
        _frameCount = 0;
        _lastFrame = _view.CurrentFrame;
        _isAttacking = true;
    }

    #endregion
}
