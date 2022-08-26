using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnPointHandler
{
    [SerializeField] private List<PlayerSpawn> _spawnPoints;
    
    public Vector3 GetRandomSpawnPoint()
    {
        if (_spawnPoints.Count == 0)
            return Vector3.zero;

        var availableSpawns = _spawnPoints.FindAll(x => x.IsAvailable);

        if (availableSpawns.Count == 0)
            availableSpawns = _spawnPoints;

        return availableSpawns[UnityEngine.Random.Range(0, availableSpawns.Count)].Position;
    }
}
