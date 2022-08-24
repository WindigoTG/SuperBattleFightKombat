using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class ProjectileView : MonoBehaviourPunCallbacks, IAttack
{
    #region Fields

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;
    [SerializeField] private Rigidbody2D _rigidBody;

    [SerializeField] float _lifetime = 2f;
    [SerializeField] private int _damage = 1;
    [Space]
    [SerializeField] GameObject _effectsPrefab;
    [Space]
    [SerializeField] private string _soundEffectName;

    private Coroutine _disableCoroutine;

    private SpriteAnimatorController _animatorController;

    private Action<ProjectileView> _onCollisionCallback;

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
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        var attack = collision.gameObject.GetComponent<IAttack>();
        if ((damageable != null && damageable.PlayerID.Equals(_playerID)) ||
            attack != null || collision.isTrigger)
            return;

        if (_disableCoroutine != null)
            StopCoroutine(_disableCoroutine);

        Deactivate();
    }

    private void Awake()
    {
        if (_spriteConfig != null)
            _animatorController = new SpriteAnimatorController(_spriteConfig);
    }

    void Update()
    {
        if (_animatorController != null)
            _animatorController.UpdateRegular();
    }

    #endregion


    #region Methods

    public void Activate(Vector3 position, Vector2 velocity, string playerID, Action<ProjectileView> onCollisionCallback, bool spawnEffects = false, bool playSound = true)
    {
        _onCollisionCallback = onCollisionCallback;

        photonView.RPC(nameof(ActivateRPC), RpcTarget.All, position, velocity, playerID, spawnEffects, playSound);
    }

    [PunRPC]
    private void ActivateRPC(Vector3 position, Vector2 velocity, string playerID, bool spawnEffects = false, bool playSound = true)
    {
        _disableCoroutine = StartCoroutine(DisableLemon());

        transform.position = position;
        transform.localScale = velocity.x > 0 ? References.RightScale : References.LeftScale;
        _rigidBody.velocity = velocity;

        _playerID = playerID;

        if (_animatorController != null)
            _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);

        if (spawnEffects && _effectsPrefab != null)
        {
            var effect = Instantiate(_effectsPrefab, position, Quaternion.identity);
            effect.transform.localScale = transform.localScale;
        }

        if (!playSound)
            return;

        if (string.IsNullOrEmpty(_soundEffectName))
            SoundManager.Instance?.PlaySound(References.PEW_SOUND);
        else
            SoundManager.Instance?.PlaySound(_soundEffectName);
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
