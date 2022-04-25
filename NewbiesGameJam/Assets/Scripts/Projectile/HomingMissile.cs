using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header ("References")]
    private Rigidbody2D _body;
    private Transform _target;

    [Header ("Projectile parameters")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _rotationLimit = 100f;
    
    [Header ("Explosion parameters")]
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _shakeIntensity = 10f;
    [SerializeField] private float _shakeTime = 0.5f;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _target = GameManager.Instance.player.transform;
    }

    private void FixedUpdate()
    {
        if (_rotationLimit <= 0) return;

        Vector2 direction = (Vector2)_target.position - _body.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _body.angularVelocity = -rotateAmount * _rotateSpeed;
        _rotationLimit -= Mathf.Abs(rotateAmount);
        _body.velocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            GameManager.Instance.player.GetComponent<PlayerHealth>().TakeDamage(_damage);

        GameManager.Instance.cinemachineShake.ShakeCamera(_shakeIntensity, _shakeTime);
        Instantiate(_explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}