using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerPrefab;

    void Start()
    {

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(1);

            return;
        }

        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }
}
