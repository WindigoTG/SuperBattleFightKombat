using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerState
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
        _view.StartDeathAnimation();
    }

    public override void UpdateRegular()
    {
        if (_view.IsAnimationDone)
            _model.Respawn();
    }

    public override void Attack() { }

    #endregion
}
