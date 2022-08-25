using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerView : MonoBehaviourPunCallbacks, IDamageable
{
    #region Fields 

    private SpriteAnimatorController _animatorController;

    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 7.5f;
    [SerializeField] private float _dashAnimationSpeedModifier = 2f;
    [Space]
    [SerializeField] private BoxCollider2D _regularCollider;
    [SerializeField] private BoxCollider2D _dashCollider;
    [SerializeField] private CompositeCollider2D _mainCollider;
    [Space]
    [Header("Attack Ogigins")]
    [SerializeField] private Transform _groundStandAttack;
    [SerializeField] private Transform _groundRunAttack;
    [SerializeField] private Transform _groundDashAttack;
    [SerializeField] private Transform _wallAttack;
    [SerializeField] private Transform _airAttack;
    [SerializeField] private Transform _groundUpAttack;
    [Space]
    [SerializeField] private SpriteAnimationsConfig _spriteAnimationsConfig;
    [Space]
    [SerializeField] private PlayerInfoUI _playerUiPrefab;

    private string _playerID;

    public Action<int, string, int> OnDamageTaken;

    Coroutine _blinking;

    private PlayerInfoUI _playerUi;

    private AnimationTrack _currentAnimation;

    #endregion


    #region Properties

    public Rigidbody2D RigidBody => _rigidBody;
    public Collider2D Collider => _mainCollider;
    public bool IsAnimationDone => _animatorController.IsAnimationFinished(_spriteRenderer);
    public int CurrentFrame => _animatorController.GetCurrentFrame(_spriteRenderer);
    public Transform GroundStandAttackOrigin => _groundStandAttack;
    public Transform GroundRunAttackOrigin => _groundRunAttack;
    public Transform GroundDashAttackOrigin => _groundDashAttack;
    public Transform WallAttackOrigin => _wallAttack;
    public Transform AirAttackOrigin => _airAttack;
    public Transform GroundUpAttackOrigin => _groundUpAttack;
    public string PlayerID => _playerID;

    public AnimationTrack CurrentAnimation => _currentAnimation;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_spriteRenderer == null)
            GetComponent<SpriteRenderer>();

        _animatorController = new SpriteAnimatorController(_spriteAnimationsConfig);

        _playerUi = Instantiate(_playerUiPrefab);
        _playerUi.SetTarget(transform);
    }

    private void Start()
    {
        if (!photonView.IsMine)
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
        else
        {
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = transform;
            SetPlayerName(PhotonNetwork.LocalPlayer.NickName);
        }
    }

    private void Update()
    {
        _animatorController?.UpdateRegular();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;

        CheckForAttack(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine)
            return;

        CheckForAttack(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!photonView.IsMine)
            return;

        CheckForAttack(collision.gameObject);
    }

    #endregion



    #region Methods

    private void CheckForAttack(GameObject go)
    {
        var attack = go.GetComponent<IAttack>();
        if (attack != null && attack.PlayerID != _playerID)
            TakeDamage(attack.Damage, attack.PlayerID, attack.Priority);
    }

    public void SetPlayerID(string playerID)
    {
        _playerID = playerID;
        photonView.RPC(nameof(SetPlayerIdRPC), RpcTarget.Others, playerID);
    }

    [PunRPC]
    private void SetPlayerIdRPC(string playerID)
    {
        _playerID = playerID;
    }

    public void SetAnimatorController(SpriteAnimatorController animationController)
    {
        _animatorController = animationController;
    }

    public void StartAnimation(AnimationTrack animation)
    {
        ProcessAnimationRequest(animation);
        photonView.RPC(nameof(StartAnimationRPC), RpcTarget.Others, animation);
    }

    [PunRPC]
    private void StartAnimationRPC(AnimationTrack animation)
    {
        ProcessAnimationRequest(animation);
    }

    private void ProcessAnimationRequest(AnimationTrack animation)
    {
        switch (animation)
        {
            case AnimationTrack.Idle:
                StartIdleAnimation();
                break;
            case AnimationTrack.Run:
                StartRunAnimation();
                break;
            case AnimationTrack.Jump:
                StartJumpAnimation();
                break;
            case AnimationTrack.Fall:
                StartFallAnimation();
                break;
            case AnimationTrack.Henshin:
                StartHenshinAnimation();
                break;
            case AnimationTrack.WallCling:
                StartWallClingAnimation();
                break;
            case AnimationTrack.TakeHit:
                StartHurtAnimation();
                break;
            case AnimationTrack.AttackStand:
                StartAttackStandAnimation();
                break;
            case AnimationTrack.AttackRun:
                StartAttackRunAnimation();
                break;
            case AnimationTrack.AttackJump:
                StartAttackJumpAnimation();
                break;
            case AnimationTrack.AttakWallCling:
                StartAttackWallClingAnimation();
                break;
            case AnimationTrack.Death:
                StartDeathAnimation();
                break;
            case AnimationTrack.IdlePreHenshin:
                StartIdlePreHenshinAnimation();
                break;
            case AnimationTrack.AttackStandAlter:
                StartAttackStandAlterAnimation();
                break;
            case AnimationTrack.AttackJumpAlter:
                StartAttackJumpAlterAnimation();
                break;
            case AnimationTrack.AttackStandUp:
                StartAttackStandUpAnimation();
                break;
            case AnimationTrack.AttackStandUpAlter:
                StartAttackStandUpAlterAnimation();
                break;
            case AnimationTrack.GroundDash:
                StartGroundDashAnimation();
                break;
            case AnimationTrack.GroundDashAttack:
                StartGroundDashAttackAnimation();
                break;
            case AnimationTrack.AirDash:
                StartAirDashAnimation();
                break;
            case AnimationTrack.AirDashUp:
                StartAirDashUpAnimation();
                break;
            case AnimationTrack.AirDashAttack:
                StartAirDashAttackAnimation();
                break;
        }

        _currentAnimation = animation;
    }

    private void StartIdleAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);

    private void StartIdlePreHenshinAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.IdlePreHenshin, true, _animationSpeed);

    private void StartRunAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Run, true, _animationSpeed);

    private void StartJumpAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Jump, false, _animationSpeed);

    private void StartFallAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Fall, false, _animationSpeed);

    private void StartHenshinAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Henshin, false, _animationSpeed);

    private void StartWallClingAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.WallCling, false, _animationSpeed);

    private void StartHurtAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.TakeHit, false, _animationSpeed);

    private void StartAttackStandAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStand, false, _animationSpeed, true);

    private void StartAttackStandAlterAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStandAlter, false, _animationSpeed, true);

    private void StartAttackRunAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackRun, true, _animationSpeed);

    private void StartAttackJumpAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackJump, false, _animationSpeed, true);

    private void StartAttackJumpAlterAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackJumpAlter, false, _animationSpeed, true);

    private void StartAttackWallClingAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttakWallCling, false, _animationSpeed, true);

    private void StartAttackStandUpAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStandUp, false, _animationSpeed, true);

    private void StartAttackStandUpAlterAnimation() =>
            _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AttackStandUpAlter, false, _animationSpeed, true);

    private void StartDeathAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.Death, false, _animationSpeed);

    private void StartGroundDashAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.GroundDash, false, _animationSpeed * _dashAnimationSpeedModifier, false);

    private void StartAirDashAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AirDash, false, _animationSpeed * _dashAnimationSpeedModifier, false);

    private void StartAirDashUpAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AirDashUp, false, _animationSpeed * _dashAnimationSpeedModifier, false);

    private void StartGroundDashAttackAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.GroundDashAttack, false, _animationSpeed * _dashAnimationSpeedModifier, true);

    private void StartAirDashAttackAnimation() =>
        _animatorController?.StartAnimation(_spriteRenderer, AnimationTrack.AirDashAttack, false, _animationSpeed * _dashAnimationSpeedModifier, true);

    public void Activate() => photonView.RPC(nameof(SetActiveRPC), RpcTarget.All, true);

    public void Deactivate() => photonView.RPC(nameof(SetActiveRPC), RpcTarget.All, false);

    [PunRPC]
    private void SetActiveRPC(bool isActive)
    {
        _spriteRenderer.enabled = isActive;
        _regularCollider.enabled = isActive;
        _dashCollider.enabled = false;

        if (!isActive)
        {
            _animatorController.StopAnimation(_spriteRenderer);
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            _rigidBody.velocity = Vector2.zero;
        }
        else
            if (photonView.IsMine)
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void StartBlinking(float duration)
    {
        if (_blinking != null)
            StopCoroutine(_blinking);

        _blinking = StartCoroutine(Blinking(duration));

        photonView.RPC(nameof(StartBlinkingRPC), RpcTarget.Others, duration);
    }

    [PunRPC]
    private void StartBlinkingRPC(float duration)
    {
        if (_blinking != null)
            StopCoroutine(_blinking);

        _blinking = StartCoroutine(Blinking(duration));
    }

    private IEnumerator Blinking(float duration)
    {
        var color = _spriteRenderer.color;

        while (duration > 0)
        {
            color.a = color.a == 1f ? 0.5f : 1f;
            _spriteRenderer.color = color;
            duration -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        color.a = 1;
        _spriteRenderer.color = color;
    }

    public void SetPlayerName(string name) => photonView.RPC(nameof(SetPlayerNameRPC), RpcTarget.All, name);

    [PunRPC]
    private void SetPlayerNameRPC(string name) => _playerUi.SetPlayerName(name);

    public void SetPlayerHealth(int health) => photonView.RPC(nameof(SetPlayerHealthRPC), RpcTarget.All, health);

    [PunRPC]
    private void SetPlayerHealthRPC(int health) => _playerUi.SetPlayerHealth(health);

    public void SetUiEnabled(bool isEnabled) => photonView.RPC(nameof(SetUiEnabledRPC), RpcTarget.All, isEnabled);

    [PunRPC]
    private void SetUiEnabledRPC(bool isEnabled) => _playerUi.SetUiEnabled(isEnabled);

    public void PlaySound(string sound) => photonView.RPC(nameof(PlaySoundRPC), RpcTarget.All, sound);

    [PunRPC]
    private void PlaySoundRPC(string sound) => SoundManager.Instance?.PlaySound(sound);

    #endregion


    #region IDamageable

    public void TakeDamage(int damage, string attackerID, int priority)
    {
        OnDamageTaken?.Invoke(damage, attackerID, priority);
    }

    #endregion
}

