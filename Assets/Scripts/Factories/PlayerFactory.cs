using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory
{
    #region Methods

    public PlayerModel CreatePlayer(Character character)
    {
        return new PlayerModel(CreatePlayerView(character), CreatePlayerStates(character), CreatePlayerWeapon(character));
    }

    private PlayerView CreatePlayerView(Character character)
    {
        PlayerView view = PhotonNetwork.Instantiate(character.ToString(), new Vector3(0f, 0f, 0f), Quaternion.identity, 0).GetComponent<PlayerView>();
        view.SetPlayerID(PhotonNetwork.LocalPlayer.UserId);
        return view;
    }

    private Dictionary<CharacterState, PlayerState> CreatePlayerStates(Character character)
    {
        switch (character)
        {
            case Character.ModelX:
            default:
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
    }

    private PlayerWeapon CreatePlayerWeapon(Character character)
    {
        switch (character)
        {
            case Character.ModelX:
            default:
                return new PlayerWeapon();
        }
    }

    #endregion
}
