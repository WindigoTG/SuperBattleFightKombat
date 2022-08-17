using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    #region Properties

    public static PlayerFactory Instance { get; private set; }

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    #endregion

    #region Methods

    public PlayerModel CreatePlayer()
    {
        return new PlayerModel(CreatePlayerView(), CreatePlayerStates());
    }

    private PlayerView CreatePlayerView()
    {
        SpriteAnimatorController animatorController = new SpriteAnimatorController(
                                                        Resources.Load<SpriteAnimationsConfig>("ModelXAnimationsConfig"));
        PlayerView view = Object.Instantiate(Resources.Load<PlayerView>("ModelX"));
        view.SetAnimatorController(animatorController);
        return view;
    }

    private Dictionary<CharacterState, PlayerState> CreatePlayerStates()
    {
        return new Dictionary<CharacterState, PlayerState> 
        { 
            { CharacterState.Idle, new IdleState() },
            { CharacterState.Run, new RunState() },
            { CharacterState.Jump, new JumpState() },
            { CharacterState.Fall, new FallState() },
            { CharacterState.WallCling, new WallClingState() },
            { CharacterState.Hurt, new HurtState() }
        };
    }

    #endregion
}
