using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClingState : PlayerState
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

        _view.StartWallClingAnimation();
    }

    public override void UpdateRegular()
    {
        _model.ResetWallCoyoteTime();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        var horisontal = Input.GetAxisRaw("Horizontal");

        if (_contactPoller.IsGrounded)
        {
            if (Mathf.Abs(horisontal) > References.InputThreshold)
                _model.SetState(CharacterState.Run);
            else
                _model.SetState(CharacterState.Idle);

            return;
        }
        
        Move(horisontal);
        if (!(Mathf.Abs(horisontal) > References.InputThreshold) ||
            (horisontal > 0 && !_contactPoller.HasRightContacts) ||
            (horisontal < 0 && !_contactPoller.HasLeftContacts))
            _model.SetState(CharacterState.Fall);
    }

    public void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
            _view.transform.localScale = (inputHor < 0 ? References.LeftScale : References.RightScale);

            if ((inputHor > 0 && _contactPoller.HasRightContacts) ||
                (inputHor < 0 && _contactPoller.HasLeftContacts))
                newVelocity = Time.fixedDeltaTime * _model.CurrentSpeed * (inputHor < 0 ? -1 : 1);
        }
        
        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: newVelocity);

        if (newVelocity == 0)
            _model.SetState(CharacterState.Fall);
    }

    #endregion
}
