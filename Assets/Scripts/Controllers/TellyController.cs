using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[Serializable]
public class TellyController : IUpdateableRegular
{
    [SerializeField] private TellyBomb _tellyPrefab;
    [SerializeField] private List<PatrolData> _patrolRoutes;
    [SerializeField] private float _spawnInterwal;

    private float _spawnTimer;

    private Dictionary<Vector3, Transform> _tellysBySpawnOrigin = new Dictionary<Vector3, Transform>();

    private bool _isActive;

    #region IUpdateableRegular

    public void UpdateRegular()
    {
        if (!_isActive)
            return;

        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;
        else
            SpawnTellys();
    }

    #endregion


    #region Methods

    public void StartSpawningTellys()
    {
        _isActive = true;
        SpawnTellys();
    }

    public void KillAndStopSpawningTellys()
    {
        _isActive = false;

        _tellysBySpawnOrigin.Clear();

        var activeTellys = GameObject.FindObjectsOfType<TellyBomb>();

        foreach (var telly in activeTellys)
            PhotonNetwork.Destroy(telly.photonView);
    }

    private void SpawnTellys()
    {
        _spawnTimer = _spawnInterwal;

        foreach (var route in _patrolRoutes)
        {
            if (!_tellysBySpawnOrigin.ContainsKey(route.SpawnPosition))
            {
                var newTelly = PhotonNetwork.InstantiateRoomObject(_tellyPrefab.name, route.SpawnPosition, Quaternion.identity).GetComponent<TellyBomb>();
                newTelly.SetWaypoints(route.WaipointsPositions);
                _tellysBySpawnOrigin.Add(route.SpawnPosition, newTelly.transform);
            }
            else if (_tellysBySpawnOrigin[route.SpawnPosition] == null)
            {
                var newTelly = PhotonNetwork.InstantiateRoomObject(_tellyPrefab.name, route.SpawnPosition, Quaternion.identity).GetComponent<TellyBomb>();
                newTelly.SetWaypoints(route.WaipointsPositions);
                _tellysBySpawnOrigin[route.SpawnPosition] = newTelly.transform;
            }
        }
    }

    public void TakeOver()
    {
        _isActive = true;

        var activeTellys = GameObject.FindObjectsOfType<TellyBomb>();

        foreach (var telly in activeTellys)
            _tellysBySpawnOrigin.Add(telly.OriginWaypoint, telly.transform);
    }

    #endregion
}
