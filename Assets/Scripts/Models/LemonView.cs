using System.Collections;
using UnityEngine;

public class LemonView : MonoBehaviour
{
    #region Fields

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;
    [SerializeField] private Rigidbody2D _rigidBody;

    [SerializeField ]float _lifetime = 2f;

    private Coroutine _disableCoroutine;

    private bool _isActive;

    private SpriteAnimatorController _animatorController;

    #endregion


    #region Properties

    public Rigidbody2D RigidBody => _rigidBody;
    public Transform Transform => transform;
    public float Lifetime => _lifetime;
    public bool IsActive => _isActive;

    #endregion


    #region UnityMethods

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    public void Activate()
    {
        gameObject.SetActive(true);
        _rigidBody.velocity = Vector2.zero;

        _isActive = true;
        _disableCoroutine = StartCoroutine(DisableLemon());

        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        _isActive = false;

        _animatorController.StopAnimation(_spriteRenderer);
    }

    private IEnumerator DisableLemon()
    {
        yield return new WaitForSeconds(_lifetime);

        Deactivate();
    }

    #endregion
}
