using UnityEngine;

public class HenshinState : PlayerState
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
        _view.StartAnimation(AnimationTrack.Henshin);
    }

    public override void Update(CurrentInputs inputs)
    {
        if (_view.IsAnimationDone)
            _model.SetState(CharacterState.Idle);

        _view.RigidBody.velocity = _view.RigidBody.velocity.Change(x: (_contactPoller.GroundVelocity.x != 0 && _contactPoller.IsGrounded) ?
                                                                        _contactPoller.GroundVelocity.x : 0);
    }

    public override void Attack() { }

    #endregion
}
