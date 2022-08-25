using Photon.Pun;
using System.Collections;
using UnityEngine;

public class SlashView : MonoBehaviourPunCallbacks, IAttack
{
    #region Fields

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;

    [SerializeField] float _lifetime = 2f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private int _priority = 1;
    [Space]
    [SerializeField] GameObject _effectsPrefab;
    [Space]
    [SerializeField] private string _soundEffectName;

    private Coroutine _disableCoroutine;

    private SpriteAnimatorController _animatorController;

    private string _playerID;

    private PlayerView _anchor;
    private AnimationTrack _anchorAnimation;

    #endregion


    #region Properties

    public string PlayerID => _playerID;
    public int Damage => _damage;
    public int Priority => _priority;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        if (_spriteConfig != null)
            _animatorController = new SpriteAnimatorController(_spriteConfig);
    }

    void Update()
    {
        if (_animatorController != null)
            _animatorController.UpdateRegular();

        if (_anchor != null && _anchor.CurrentAnimation != _anchorAnimation)
            Deactivate();
    }

    private void LateUpdate()
    {
        if (_anchor != null)
        {
            transform.localScale = _anchor.transform.localScale;
            transform.position = _anchor.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<IDamageable>();
        if (player != null && (player is PlayerView) && player.PlayerID.Equals(_playerID))
        {
            _anchor = player as PlayerView;
            _anchorAnimation = _anchor.CurrentAnimation;
        }

    }

    #endregion


    #region Methods

    public void Activate(Vector3 position, Vector3 scale, string playerID, bool spawnEffects = false, bool playSound = true)
    {

        photonView.RPC(nameof(ActivateRPC), RpcTarget.All, position, scale, playerID, spawnEffects, playSound);
    }

    [PunRPC]
    private void ActivateRPC(Vector3 position, Vector3 scale, string playerID, bool spawnEffects = false, bool playSound = true)
    {
        _disableCoroutine = StartCoroutine(DisableSlash());

        transform.position = position;
        transform.localScale = scale;

        _playerID = playerID;

        if (_animatorController != null)
            _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, false, _animationSpeed);

        if (spawnEffects && _effectsPrefab != null)
        {
            var effect = Instantiate(_effectsPrefab, position, Quaternion.identity);
            effect.transform.localScale = transform.localScale;
        }

        if (!playSound)
            return;

        if (string.IsNullOrEmpty(_soundEffectName))
            SoundManager.Instance?.PlaySound(References.SLASH_SOUND);
        else
            SoundManager.Instance?.PlaySound(_soundEffectName);
    }

    public void Deactivate()
    {
        Destroy(gameObject);
    }

    private IEnumerator DisableSlash()
    {
        yield return new WaitForSeconds(_lifetime);

        Deactivate();
    }

    #endregion
}
