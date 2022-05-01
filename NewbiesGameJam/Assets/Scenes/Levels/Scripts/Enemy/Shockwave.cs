using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private int _damage = 1;
    private bool _isMoving = true;
    private Animator _anim;
    private Rigidbody2D _body;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!_isMoving) return;
        _body.velocity = new Vector3(-Mathf.Sign(transform.localScale.x) * _speed, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return;
        _isMoving = false;
        _body.velocity = Vector3.zero;
        if (other.gameObject == GameManager.Instance.player)
            GameManager.Instance.playerHealth.TakeDamage(_damage);
        _anim.SetTrigger("Hit");
    }

    private void Deactivate()
    {
        Destroy(gameObject);
    }

    private static readonly int HitKey = Animator.StringToHash("Hit");
}