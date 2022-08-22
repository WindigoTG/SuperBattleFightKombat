using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory
{
    #region Methods

    public PlayerModel CreatePlayer()
    {
        return new PlayerModel(CreatePlayerView(), CreatePlayerStates(), CreatePlayerWeapon());
    }

    private PlayerView CreatePlayerView()
    {
        PlayerView view = PhotonNetwork.Instantiate("ModelX", new Vector3(0f, 0f, 0f), Quaternion.identity, 0).GetComponent<PlayerView>();
        view.SetPlayerID(PhotonNetwork.LocalPlayer.UserId);
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
            { CharacterState.Hurt, new HurtState() },
            { CharacterState.Death, new DeathState() },
            { CharacterState.IdlePreHenshin, new IdlePreHenshinState() },
            { CharacterState.Henshin, new HenshinState() }
        };
    }

    private PlayerWeapon CreatePlayerWeapon()
    {
        return new PlayerWeapon();
    }

    #endregion
}
