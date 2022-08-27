using UnityEngine;

public class ModelHAirDashState : PlayerState
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
        _view.StartAnimation(AnimationTrack.AirDash);
        _view.PlaySound(References.BOOST_SOUND);
        _model.SetHasAirDashed(true);
    }

    public override void Update(CurrentInputs inputs)
    {
        Move(inputs.Horisontal);

        if (_view.IsAnimationDone)
            _model.SetState(CharacterState.Fall);
    }

    private void Move(float inputHor)
    {
        if (inputHor == 0 || _view.RigidBody.velocity.x > 0 && inputHor < 0 || _view.RigidBody.velocity.x < 0 && inputHor > 0)
        {
            _model.SetState(CharacterState.Fall);
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
            _model.SetState(CharacterState.Fall);
            return;
        }

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity + (_contactPoller.IsGrounded ? _contactPoller.GroundVelocity.x : 0),
                                                                    y: (_contactPoller.IsGrounded ? _contactPoller.GroundVelocity.y : 0));

        WallCheck(inputHor);
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

    public override void Attack() { }

    #endregion
}
