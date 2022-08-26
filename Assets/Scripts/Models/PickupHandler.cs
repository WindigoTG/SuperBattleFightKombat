using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupHandler : IUpdateableRegular
{
    [SerializeField] private Pickup _pickupPrefab;
    [SerializeField] private List<PickupSpawn> _pickupSpawns;
    [SerializeField] private float _spawnInterval;

    private float _spawnTimer;

    public void UpdateRegular()
    {
        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;
        else
            SpawnPickup();
    }

    private void SpawnPickup()
    {
        _spawnTimer = _spawnInterval;

        var availableSpawns = _pickupSpawns.FindAll(x => x.IsAvailable);

        if (availableSpawns.Count == 0)
            return;

        var spawnPoint = availableSpawns[Random.Range(0, availableSpawns.Count)].Position;

        var chance = Random.Range(0.0f, 100.0f);

        PickupType type;
        if (chance < 10)
            type = PickupType.Life;
        else if (chance > 75)
            type = PickupType.HealthLarge;
        else
            type = PickupType.HealthSmall;

        var pickup = PhotonNetwork.Instantiate(_pickupPrefab.name, spawnPoint, Quaternion.identity).GetComponent<Pickup>();
        pickup.SetPickupType(type);
    }
}
