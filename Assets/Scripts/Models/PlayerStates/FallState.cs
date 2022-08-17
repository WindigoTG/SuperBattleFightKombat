using UnityEngine;

public class FallState : PlayerState
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

        _view.StartFallAnimation();
    }

    public override void UpdateRegular()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (_model.IsGroundCoyoteTime || _model.IsWallCoyoteTime))
        {
            _model.SetState(CharacterState.Jump);
            return;
        }

        var horisontal = Input.GetAxisRaw("Horizontal");
        Move(horisontal);
        VerticalCheck(horisontal);
    }

    public void Move(float inputHor)
    {
        var newVelocity = 0f;

        if (Mathf.Abs(inputHor) > References.InputThreshold)
        {
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
        if (_contactPoller.IsGrounded)
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

    #endregion
}
