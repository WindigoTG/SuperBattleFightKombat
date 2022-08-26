using Photon.Pun;
using UnityEngine;

public class Pickup : MonoBehaviourPunCallbacks, IPickup
{
    private SpriteAnimatorController _animatorController;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 10f;
    [SerializeField] private SpriteAnimationsConfig _spriteAnimationsConfig;

    private PickupType _type;

    public PickupType Type => _type;

    private void Awake()
    {
        if (_spriteRenderer == null)
            GetComponent<SpriteRenderer>();

        _animatorController = new SpriteAnimatorController(_spriteAnimationsConfig);
    }

    private void Update()
    {
        _animatorController.UpdateRegular();
    }

    public void PickUp()
    {
        PhotonNetwork.Destroy(photonView);
    }

    public void SetPickupType(PickupType type) => photonView.RPC(nameof(SetPickupTypeRPC), RpcTarget.All, type);

    [PunRPC]
    private void SetPickupTypeRPC(PickupType type)
    {
        _type = type;

        switch (type)
        {
            case PickupType.HealthSmall:
                _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);
                break;
            case PickupType.HealthLarge:
                _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Run, true, _animationSpeed);
                break;
            case PickupType.Life:
                _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Jump, true, _animationSpeed);
                break;
        }
    }
}
