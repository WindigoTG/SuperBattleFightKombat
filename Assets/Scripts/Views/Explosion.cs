using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviourPunCallbacks, IAttack
{
    #region Fields

    [SerializeField] private int _damage = 1;
    [SerializeField] private int _priority = 0;
    [SerializeField] private string _playerId = "Environment-Explosion";
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteAnimationsConfig _spriteAnimationsConfig;
    [SerializeField] private float _animationSpeed = 1.0f;

    private SpriteAnimatorController _animatorController;

    #endregion


    #region Properties

    public string PlayerID => _playerId;
    public int Damage => _damage;
    public int Priority => _priority;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_spriteRenderer == null)
            GetComponent<SpriteRenderer>();

        _animatorController = new SpriteAnimatorController(_spriteAnimationsConfig);
    }

    void Start()
    {
        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, false, _animationSpeed);
        SoundManager.Instance.PlaySound(References.BOOM_SOUND);
    }

    void Update()
    {
        _animatorController.UpdateRegular();

        if (_animatorController.IsAnimationFinished(_spriteRenderer))
            Destroy(gameObject);
    }

    #endregion


    #region Methods

    public void SetPosition(Vector3 position) => photonView.RPC(nameof(SetPositionRPC), RpcTarget.All, position);

    [PunRPC]
    private void SetPositionRPC(Vector3 position) => transform.position = position;

    #endregion
}
