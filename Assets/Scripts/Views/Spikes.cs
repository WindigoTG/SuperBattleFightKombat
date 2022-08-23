using UnityEngine;

public class Spikes : MonoBehaviour, IAttack
{
    [SerializeField] int _damage = 1;
    [SerializeField] string _playerId = "Environment-Spikes";
    public string PlayerID => _playerId;

    public int Damage => _damage;
}
