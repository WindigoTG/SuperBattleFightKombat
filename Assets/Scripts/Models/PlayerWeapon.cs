using UnityEngine;

public abstract class PlayerWeapon
{
    public abstract bool Attack(Vector3 attackOrigin, float direction, bool spawnEffects = false);
}
