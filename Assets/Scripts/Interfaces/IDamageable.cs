using System;

public interface IDamageable
{
    public Action<int> OnDamageTaken { get; }

    public void TakeDamage(int damage);
}
