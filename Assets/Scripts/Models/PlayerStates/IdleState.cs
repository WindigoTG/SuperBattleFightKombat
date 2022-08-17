using UnityEngine;

public class IdleState : PlayerState
{
    #region Fields

    private PlayerModel _model;
    private PlayerView _view;
    private ContactsPoller _contactPoller;

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
        _view.StartIdleAnimation();
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
        if (Mathf.Abs(horisontal) > References.InputThreshold)
        {
            _model.SetState(CharacterState.Run);
            return;
        }

        if (_contactPoller.GroundVelocity.x != 0 && _contactPoller.IsGrounded)
            _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: _contactPoller.GroundVelocity.x);
    }

    #endregion
}
