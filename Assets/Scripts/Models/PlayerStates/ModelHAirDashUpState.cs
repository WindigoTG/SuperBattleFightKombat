using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHAirDashUpState : PlayerState
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
        _view.StartAnimation(AnimationTrack.AirDashUp);
        _view.PlaySound(References.BOOST_SOUND);
        _model.SetHasAirDashed(true);
    }

    public override void Update(CurrentInputs inputs)
    {
        _view.RigidBody.velocity = new Vector2(0, Time.fixedDeltaTime * _model.CurrentSpeed); 

        if (_view.IsAnimationDone)
            _model.SetState(CharacterState.Fall);
    }

    public override void Attack() { }

    #endregion
}
