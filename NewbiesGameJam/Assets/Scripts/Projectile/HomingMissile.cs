using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private int _damage = 1;
    private Rigidbody2D _body;
    private Transform _target;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _target = GameManager.Instance.player.transform;
    }

    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)_target.position - _body.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _body.angularVelocity = -rotateAmount * _rotateSpeed;
        _body.velocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            GameManager.Instance.player.GetComponent<PlayerHealth>().TakeDamage(_damage);

        Instantiate(_explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}