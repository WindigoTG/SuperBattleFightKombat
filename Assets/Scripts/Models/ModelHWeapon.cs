using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelHWeapon : PlayerWeapon
{
    private Dictionary<int, string> _attackNameByIndex = new Dictionary<int, string>
    {
        { 0, "SlashH1"},
        { 1, "SlashH2"},
        { 2, "SlashH3"},
        { 3, "SlashH4"},
        { 4, "SlashH5"},
    };

    public override bool Attack(Vector3 attackOrigin, float direction, bool spawnEffects = false, int attackIndex = 0)
    {
        var slash = PhotonNetwork.Instantiate(_attackNameByIndex[attackIndex], attackOrigin, Quaternion.identity).GetComponent<SlashView>();
        var scale = direction > 0 ? References.RightScale : References.LeftScale;
        slash.Activate(attackOrigin, scale, PhotonNetwork.LocalPlayer.UserId, spawnEffects);

        return true;
    }
}

