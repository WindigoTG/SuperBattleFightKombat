using UnityEngine;

public class Spikes : MonoBehaviour, IAttack
{
    [SerializeField] int _damage = 1;
    [SerializeField] string _playerId = "Environment-Spikes";
    [SerializeField] int _priority = 0;
    public string PlayerID => _playerId;
    public int Damage => _damage;
    public int Priority => _priority;
}
