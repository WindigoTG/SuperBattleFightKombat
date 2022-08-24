using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelXWeapon : PlayerWeapon
{
    private List<ProjectileView> _lemons = new List<ProjectileView>(3);
    private float _force = 20f;

    private const int MAX_LEMONS = 3;

    public override bool Attack(Vector3 attackOrigin, float direction, bool spawnEffects = false)
    {
        if (_lemons.Count >= MAX_LEMONS)
            return false;

        var lemon = PhotonNetwork.Instantiate("Lemon", attackOrigin, Quaternion.identity).GetComponent<ProjectileView>();
        lemon.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        lemon.Activate(attackOrigin, Vector2.right * (_force * direction), PhotonNetwork.LocalPlayer.UserId, RemoveLemon, spawnEffects);

        _lemons.Add(lemon);

        return true;
    }

    private void RemoveLemon(ProjectileView lemon)
    {
        if (_lemons.Contains(lemon))
            _lemons.Remove(lemon);
    }
}
