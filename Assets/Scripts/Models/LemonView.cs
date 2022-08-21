using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class LemonView : MonoBehaviourPunCallbacks, IAttack
{
    #region Fields

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Collider2D _collider;

    [SerializeField] float _lifetime = 2f;
    [SerializeField] private int _damage = 1;

    private Coroutine _disableCoroutine;

    private SpriteAnimatorController _animatorController;

    private Action<LemonView> _onCollisionCallback;

    private string _playerID;

    #endregion


    #region Properties

    public Rigidbody2D RigidBody => _rigidBody;
    public Transform Transform => transform;
    public float Lifetime => _lifetime;
    public string PlayerID => _playerID;
    public int Damage => _damage;

    #endregion


    #region UnityMethods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<IDamageable>();
        var attack = collision.GetComponent<IAttack>();
        if ((damageable != null && damageable.PlayerID.Equals(_playerID)) ||
            attack != null)
            return;

        if (_disableCoroutine != null)
            StopCoroutine(_disableCoroutine);

        Deactivate();
    }

    private void Awake()
    {
        _animatorController = new SpriteAnimatorController(_spriteConfig);
    }

    void Update()
    {
        _animatorController.UpdateRegular();
    }

    #endregion


    #region Methods

    public void Activate(Vector3 position, Vector2 velocity, string playerID, Action<LemonView> onCollisionCallback)
    {
        _onCollisionCallback = onCollisionCallback;

        _disableCoroutine = StartCoroutine(DisableLemon());

        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);

        transform.position = position;
        transform.localScale = velocity.x > 0 ? References.RightScale : References.LeftScale;
        _rigidBody.velocity = velocity;

        _playerID = playerID;

        photonView.RPC(nameof(ActivateRPC), RpcTarget.Others, position, velocity, playerID);

        SoundManager.Instance?.PlaySound(References.PEW_SOUND);
    }

    [PunRPC]
    private void ActivateRPC(Vector3 position, Vector2 velocity, string playerID)
    {
        _disableCoroutine = StartCoroutine(DisableLemon());

        transform.position = position;
        transform.localScale = velocity.x > 0 ? References.RightScale : References.LeftScale;
        _rigidBody.velocity = velocity;

        _playerID = playerID;

        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);

        SoundManager.Instance?.PlaySound(References.PEW_SOUND);
    }

    public void Deactivate()
    {
        if (!photonView.IsMine)
            Destroy(gameObject);
        else
        {
            _onCollisionCallback?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private IEnumerator DisableLemon()
    {
        yield return new WaitForSeconds(_lifetime);

        Deactivate();
    }

    #endregion
}
