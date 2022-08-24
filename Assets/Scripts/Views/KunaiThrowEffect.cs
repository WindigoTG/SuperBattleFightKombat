using UnityEngine;

public class KunaiThrowEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;

    private SpriteAnimatorController _animatorController;

    private void Awake()
    {
        _animatorController = new SpriteAnimatorController(_spriteConfig);
    }

    // Start is called before the first frame update
    void Start()
    {
        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, false, _animationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        _animatorController.UpdateRegular();

        if (_animatorController.IsAnimationFinished(_spriteRenderer))
            Destroy(gameObject);
    }
}
