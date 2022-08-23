using UnityEngine;
using Pathfinding;
using Photon.Pun;

public class TellyBomb : MonoBehaviourPunCallbacks
{
    #region Fields

    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private float _pathRecalculationInterval = 1.0f;
    [SerializeField] private float _speed = 50.0f;
    [SerializeField] private float _sqrChaseRadius = 50.0f;
    [SerializeField] Seeker _seeker;
    [SerializeField] private float _minSqrDistanceToTarget = 0.2f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteAnimationsConfig _spriteAnimationsConfig;
    [SerializeField] private float _animationSpeed = 10.0f;
    [SerializeField] private Explosion _explosionPrefab;

    private SpriteAnimatorController _animatorController;

    private Vector3[] _wayPoints;
    private Vector3 _wayPoint;

    private float _recalculationTime;
    private int _currentPointIndex;

    private Transform _target;

    private Path _path;

    #endregion


    #region Properties

    public Vector3 OriginWaypoint => _wayPoints[0];

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_spriteRenderer == null)
            GetComponent<SpriteRenderer>();

        _animatorController = new SpriteAnimatorController(_spriteAnimationsConfig);
    }

    private void Start()
    {
        _animatorController.StartAnimation(_spriteRenderer, AnimationTrack.Idle, true, _animationSpeed);
    }

    private void Update()
    {
        _animatorController.UpdateRegular();
    }

    private void FixedUpdate()
    {
        if (_wayPoints == null || _wayPoints.Length == 0 || !photonView.IsMine)
            return;

        if (_target != null && (_target.position - transform.position).sqrMagnitude > _sqrChaseRadius)
            _target = null;

        if (_recalculationTime > 0)
            _recalculationTime -= Time.fixedDeltaTime;
        else
        { 
            RecalculatePath();
            _recalculationTime = _pathRecalculationInterval;
        }

        var newVelocity = CalculateVelocity(transform.position) * Time.fixedDeltaTime;
        _rigidBody.velocity = newVelocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        if (_target == null)
            _target = collision.transform;
        else if (_target = collision.transform)
            return;
        else
        {
            if ((_target.position - transform.position).sqrMagnitude > (collision.transform.position - transform.position).sqrMagnitude)
                _target = collision.transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine)
            return;

        var damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        PhotonNetwork.Instantiate(_explosionPrefab.name, transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(photonView);
    }

    #endregion


    #region Methods

    public void SetWaypoints(Vector3[] wayPoints)
    {
        photonView.RPC(nameof(SetWaypointsRPC), RpcTarget.All, wayPoints);
    }

    [PunRPC]
    private void SetWaypointsRPC(Vector3[] wayPoints)
    {
        _wayPoints = wayPoints;
        _wayPoint = wayPoints[0];
    }

    public void RecalculatePath()
    {
        if (_seeker.IsDone())
        {
            var target = _target != null ? _target.position : GetWaypoint(transform.position);
            _seeker.StartPath(_rigidBody.position, target, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;
        _path = p;
        _currentPointIndex = 0;
    }


    public Vector2 CalculateVelocity(Vector2 fromPosition)
    {
        if (_path == null) return Vector2.zero;
        if (_currentPointIndex >= _path.vectorPath.Count) return Vector2.zero;

        var direction = ((Vector2)_path.vectorPath[_currentPointIndex] - fromPosition).normalized;
        var result = _speed * direction;
        var sqrDistance = Vector2.SqrMagnitude((Vector2)_path.vectorPath[_currentPointIndex] - fromPosition);
        if (sqrDistance <= _minSqrDistanceToTarget)
        {
            _currentPointIndex++;
        }
        return result;
    }

    public Vector3 GetWaypoint(Vector2 fromPosition)
    {
        var sqrDistance = Vector2.SqrMagnitude((Vector2)_wayPoint - fromPosition);
        if (sqrDistance <= _minSqrDistanceToTarget)
        {
            _wayPoint = GetNextWaypoint();
        }

        return _wayPoint;
    }

    private Vector3 GetNextWaypoint()
    {
        Vector3 newWaypoint;

        do
        {
            newWaypoint = _wayPoints[Random.Range(0, _wayPoints.Length)];
        } while (newWaypoint == _wayPoint);

        return newWaypoint;
    }

    public void ResetPatrolling()
    {
        _wayPoint = GetNextWaypoint();
    }

    #endregion
}
