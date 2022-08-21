using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Fields

    PlayerModel _player;
    PlayerFactory _playerfactory;

    bool _isJumpPressed;
    bool _isAttackPressed;
    float _horisontalInput;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _playerfactory = new PlayerFactory();
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;

        _player = _playerfactory.CreatePlayer();

        _player.StartAtPosition(new Vector3(Random.Range(-2f,2f), 0, 0));
    }

    public void Update()
    {
        if (photonView.IsMine)
        {
            _horisontalInput = Input.GetAxisRaw("Horizontal");
            _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
            _isAttackPressed = Input.GetMouseButtonDown(0);


            _player.Update(new CurrentInputs
            {
                Horisontal = _horisontalInput,
                IsJumpPressed = _isJumpPressed,
                IsAttackPressed = _isAttackPressed
            });
        }

        _horisontalInput = default;
        _isJumpPressed = default;
        _isAttackPressed = default;


        //_player.Move(Input.GetAxisRaw("Horizontal"));

        //if (Input.GetKeyDown(KeyCode.Space))
        //    _player.Jump();
    }

    #endregion


    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(_horisontalInput);
        //    stream.SendNext(_isJumpPressed);
        //    stream.SendNext(_isAttackPressed);
        //}
        //else
        //{
        //    _horisontalInput = (float)stream.ReceiveNext();
        //    _isJumpPressed = (bool)stream.ReceiveNext();
        //    _isAttackPressed = (bool)stream.ReceiveNext();
        //}
    }

    #endregion
}


public struct CurrentInputs
{
    public float Horisontal;
    public bool IsJumpPressed;
    public bool IsAttackPressed;
}

