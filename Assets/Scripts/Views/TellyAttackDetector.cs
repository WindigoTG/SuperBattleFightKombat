using System;
using UnityEngine;

public class TellyAttackDetector : MonoBehaviour
{
    public Action OnAttackReceived;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var attack = collision.gameObject.GetComponent<IAttack>();

        if (attack == null || attack.Priority <= 0)
            return;

        OnAttackReceived?.Invoke();
    }
}
