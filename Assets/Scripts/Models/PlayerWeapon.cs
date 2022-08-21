using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon
{
    List<LemonView> _lemons = new List<LemonView>(3);
    float _force = 20f;

    private const int MAX_LEMONS = 3;

    public bool Shoot(Vector3 attackOrigin, float direction)
    {
        if (_lemons.Count >= MAX_LEMONS)
            return false;

        var lemon = PhotonNetwork.Instantiate("Lemon", attackOrigin, Quaternion.identity).GetComponent<LemonView>();
        lemon.Activate(attackOrigin, Vector2.right * (_force * direction), PhotonNetwork.LocalPlayer.UserId, RemoveLemon);

        _lemons.Add(lemon);

        return true;
    }

    private void RemoveLemon(LemonView lemon)
    {
        if (_lemons.Contains(lemon))
            _lemons.Remove(lemon);
    }
}
