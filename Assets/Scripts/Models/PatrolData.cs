using UnityEngine;

[System.Serializable]
public class PatrolData
{
    [SerializeField] Transform[] _waypoints;

    public Transform SpawnPoint => _waypoints[0];
    public Vector3 SpawnPosition => SpawnPoint.position;
    public Transform[] Waypoints => _waypoints;
    public Vector3[] WaipointsPositions
    {
        get
        {
            var positions = new Vector3[_waypoints.Length];
            for (int i = 0; i < _waypoints.Length; i++)
                positions[i] = _waypoints[i].position;

            return positions;
        }
    }
}
