using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header ("References")]
    private Transform _target;
    private Rigidbody2D _body;

    [Header ("Projectile parameters")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _rotationLimit = 100f;
    [SerializeField] private bool _isHoming;
    [SerializeField] private float _lifetime = 5f;
    private float _initialRotationLimit;
    private float _lifetimeCount = float.NegativeInfinity;
    private bool _gotDirection = false;
    
    [Header ("Explosion parameters")]
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;
    [SerializeField] private bool _doShake = false;
    private GameObject _explosionParticles;

    [Header ("Sfx")]
    [SerializeField] private AudioClip _explosionSfx;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _target = GameManager.Instance.player.transform;
        _initialRotationLimit = _rotationLimit;
    }

    private void Update()
    {
        _lifetimeCount += Time.deltaTime;
        if (_lifetimeCount > _lifetime)
            gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_rotationLimit <= 0)
        {
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        if (_isHoming)
        {
            Vector2 direction = (Vector2)_target.position - _body.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            _body.angularVelocity = -rotateAmount * _rotateSpeed;
            _rotationLimit -= Mathf.Abs(rotateAmount);
            _body.velocity = transform.up * _speed;
        }
        else if (!_gotDirection)
        {
            Vector2 direction = ((Vector2)(_target.transform.position - transform.position)).normalized * _speed;
            _body.velocity = direction;
            _gotDirection = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Range" || other.name == "LevelBounds" || other.name == "Boss" || other.name == "LevelBoundsPostCutscene") return;
        
        if (other.gameObject.CompareTag("Player"))
            GameManager.Instance.player.GetComponent<PlayerHealth>().TakeDamage(_damage);

        if (_doShake)
            GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);

        _explosionParticles = Instantiate(_explosionEffectPrefab, transform.position, transform.rotation);
        GetComponent<CircleCollider2D>().enabled = false;
        _body.constraints = RigidbodyConstraints2D.None;
        AudioManager.Instance.PlaySound(_explosionSfx);
        gameObject.SetActive(false);
        _rotationLimit = _initialRotationLimit;
        Destroy(_explosionParticles, _explosionParticles.GetComponent<ParticleSystem>().main.duration);
    }

    private void OnEnable()
    {
        _lifetimeCount = 0f;
        GetComponent<CircleCollider2D>().enabled = true;
        _gotDirection = false;
        if (!_isHoming)
            CalculateAngle();
    }

    private void CalculateAngle()
    {
        Vector3 offset = GameManager.Instance.player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);

       Quaternion rotation = Quaternion.LookRotation(Vector3.forward, offset);

       transform.rotation = rotation * Quaternion.Euler(0f, 0f, 90f); 
    }
}