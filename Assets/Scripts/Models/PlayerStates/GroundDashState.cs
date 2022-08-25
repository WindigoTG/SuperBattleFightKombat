using UnityEngine;

public class GroundDashState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

    private float _speedModifier = 2.0f;

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
        _view.StartAnimation(AnimationTrack.GroundDash);
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

        Move(inputs.Horisontal);

        if (_view.IsAnimationDone)
        {
            if (inputs.Horisontal != 0)
                _model.SetState(CharacterState.Run);
            else
                _model.SetState(CharacterState.Idle);
        }
    }

    private void Move(float inputHor)
    {
        if (inputHor == 0)
        {
            _model.SetState(CharacterState.Idle);
            return;
        }

        if (_view.RigidBody.velocity.x > 0 && inputHor < 0 || _view.RigidBody.velocity.x < 0 && inputHor > 0)
        {
            _model.SetState(CharacterState.Run);
            return;
        }

        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && !_contactPoller.HasRightContacts) ||
                (inputHor < 0 && !_contactPoller.HasLeftContacts) ||
                (inputHor != 0))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * _speedModifier * (inputHor < 0 ? -1 : 1);
        }
        else
        {
            _model.SetState(CharacterState.Idle);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity + (_contactPoller.IsGrounded ? _contactPoller.GroundVelocity.x : 0));
    }

    public override void Attack() { }

    #endregion
}

