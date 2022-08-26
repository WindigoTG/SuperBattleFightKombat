using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    #region Fields

    private List<PlayerView> _playersInProximity = new List<PlayerView>();

    #endregion


    #region Properties

    public Vector3 Position => transform.position;

    public bool IsAvailable => _playersInProximity.Count == 0;

    #endregion


    #region Unity Methods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerView>();

        if (player != null && !_playersInProximity.Contains(player))
            _playersInProximity.Add(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerView>();

        if (player != null && _playersInProximity.Contains(player))
            _playersInProximity.Remove(player);
    }

    #endregion
}
