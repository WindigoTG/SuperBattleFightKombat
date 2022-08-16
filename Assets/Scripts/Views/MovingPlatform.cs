using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private SliderJoint2D _sliderJoint;
    [SerializeField] private SpriteAnimationsConfig _spriteConfig;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _animationSpeed = 1f;

    private SpriteAnimatorController _animatorController;

    private bool HasReachedLimit => ((Vector2)transform.position - _sliderJoint.connectedAnchor).magnitude >= _sliderJoint.limits.max;

    private void Awake()
    {
        _animatorController = new SpriteAnimatorController(_spriteConfig);
    }

    void Start()
    {
        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);
    }

    void Update()
    {
        _animatorController.UpdateRegular();
    }

    private void FixedUpdate()
    {
        if (HasReachedLimit)
            Invert();
    }

    public void Invert()
    {
        var motor = _sliderJoint.motor;
        motor.motorSpeed *= -1;
        _sliderJoint.motor = motor;
    }
}
