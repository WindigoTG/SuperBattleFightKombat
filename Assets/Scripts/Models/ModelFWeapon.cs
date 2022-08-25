using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelFWeapon : PlayerWeapon
{
    private List<ProjectileView> _melons = new List<ProjectileView>(3);
    private float _force = 15f;

    private float _offset = 0.35f;

    private const int MAX_MELONS = 4;

    public override bool Attack(Vector3 attackOrigin, float direction, bool spawnEffects = false, int _ = 0)
    {
        if (_melons.Count >= MAX_MELONS)
            return false;

        var force = direction != 0 ? Vector2.right * (_force * direction) : Vector2.up * _force;

        var firstPos = direction != 0 ? attackOrigin + Vector3.up * _offset : attackOrigin + Vector3.right * _offset;
        var secondPos = direction != 0 ? attackOrigin - Vector3.up * _offset : attackOrigin - Vector3.right * _offset;

        Debug.LogWarning(firstPos + "  |  " + secondPos);

        var melon = PhotonNetwork.Instantiate("Melon", firstPos, Quaternion.identity).GetComponent<ProjectileView>();
        melon.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        melon.Activate(firstPos, force, PhotonNetwork.LocalPlayer.UserId, RemoveMelon, spawnEffects);
        _melons.Add(melon);

        melon = PhotonNetwork.Instantiate("Melon", secondPos, Quaternion.identity).GetComponent<ProjectileView>();
        melon.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        melon.Activate(secondPos, force, PhotonNetwork.LocalPlayer.UserId, RemoveMelon, spawnEffects, false);
        _melons.Add(melon);

        return true;
    }

    private void RemoveMelon(ProjectileView lemon)
    {
        if (_melons.Contains(lemon))
            _melons.Remove(lemon);
    }
}
