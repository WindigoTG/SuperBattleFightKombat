using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelPWeapon : PlayerWeapon
{
    private List<ProjectileView> _kunais = new List<ProjectileView>(3);
    private float _force = 10f;

    private const int MAX_KUNAIS = 5;

    public override bool Attack(Vector3 attackOrigin, float direction, bool spawnEffects = false, int _ = 0)
    {
        if (_kunais.Count >= MAX_KUNAIS)
            return false;

        var kunai = PhotonNetwork.Instantiate("Kunai", attackOrigin, Quaternion.identity).GetComponent<ProjectileView>();
        kunai.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        kunai.Activate(attackOrigin, Vector2.right * (_force * direction), PhotonNetwork.LocalPlayer.UserId, RemoveKunai, spawnEffects);
        _kunais.Add(kunai);

        kunai = PhotonNetwork.Instantiate("Kunai", attackOrigin, Quaternion.Euler(0, 0, 10.0f)).GetComponent<ProjectileView>();
        kunai.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        kunai.Activate(attackOrigin, kunai.transform.right * _force * direction, PhotonNetwork.LocalPlayer.UserId, RemoveKunai, spawnEffects, false);
        _kunais.Add(kunai);

        kunai = PhotonNetwork.Instantiate("Kunai", attackOrigin, Quaternion.Euler(0, 0, -10.0f)).GetComponent<ProjectileView>();
        kunai.transform.localScale = direction > 0 ? References.RightScale : References.LeftScale;
        kunai.Activate(attackOrigin, kunai.transform.right * _force * direction, PhotonNetwork.LocalPlayer.UserId, RemoveKunai, spawnEffects, false);
        _kunais.Add(kunai);

        return true;
    }

    private void RemoveKunai(ProjectileView lemon)
    {
        if (_kunais.Contains(lemon))
            _kunais.Remove(lemon);
    }
}
