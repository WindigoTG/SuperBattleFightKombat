using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields

    PlayerModel _player;
    PlayerFactory _playerfactory;

    #endregion


    #region Unity Methods

    private void Start()
    {
        _playerfactory = PlayerFactory.Instance;
        _player = _playerfactory.CreatePlayer();

        _player.StartAtPosition(new Vector3(0, 0, 0));
    }

    public void Update()
    {
        _player.UpdateRegular();


        _player.Move(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Space))
            _player.Jump();
    }

    #endregion
}

