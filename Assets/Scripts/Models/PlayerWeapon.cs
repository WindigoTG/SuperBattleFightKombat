using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon
{
    List<LemonView> _lemons = new List<LemonView>(3);
    float _force = 20f;

    public PlayerWeapon()
    {
        var lemonPrefab = Resources.Load<LemonView>("Lemon");

        for (int i = 0; i < 3; i++)
        {
            var lemon  = GameObject.Instantiate(lemonPrefab);
            lemon.Deactivate();
            _lemons.Add(lemon);
        }
    }

    public bool Shoot(Vector3 attackOrigin, float direction)
    {
        var availableLemon = _lemons.Find(x => !x.IsActive);

        if (availableLemon == null)
            return false;

        availableLemon.transform.position = attackOrigin;
        availableLemon.transform.localScale = availableLemon.transform.localScale.Change(x: direction);
        availableLemon.Activate();
        availableLemon.RigidBody.velocity = Vector2.right * (_force * direction);

        return true;
    }
}
