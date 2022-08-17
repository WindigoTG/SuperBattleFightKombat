using UnityEngine;

public class HurtState : PlayerState
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
        _view.StartHurtAnimation();
    }

    public override void UpdateRegular()
    {
        if (!_view.IsAnimationDone)
            return;

        if (_contactPoller.IsGrounded)
            _model.SetState(CharacterState.Idle);
        else
            _model.SetState(CharacterState.Fall);
    }

    #endregion
}
