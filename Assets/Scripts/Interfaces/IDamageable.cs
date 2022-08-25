using System;

public interface IDamageable
{
    public string PlayerID { get; }
    public void TakeDamage(int damage, string attackerID, int priority);
}
