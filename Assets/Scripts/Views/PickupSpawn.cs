using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawn : MonoBehaviour
{
    #region Fields

    private List<Pickup> _pickupsInProximity = new List<Pickup>();

    #endregion


    #region Properties

    public Vector3 Position => transform.position;

    public bool IsAvailable => _pickupsInProximity.Count == 0;

    #endregion


    #region Unity Methods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pickup = collision.GetComponent<Pickup>();

        if (pickup != null && !_pickupsInProximity.Contains(pickup))
            _pickupsInProximity.Add(pickup);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pickup = collision.GetComponent<Pickup>();

        if (pickup != null && _pickupsInProximity.Contains(pickup))
            _pickupsInProximity.Remove(pickup);
    }

    #endregion
}
