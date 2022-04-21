using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private GameObject _explosionEffect;
    private Rigidbody2D _body;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)_target.position - _body.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _body.angularVelocity = -rotateAmount * _rotateSpeed;
        _body.velocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D()
    {
        Instantiate(_explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}