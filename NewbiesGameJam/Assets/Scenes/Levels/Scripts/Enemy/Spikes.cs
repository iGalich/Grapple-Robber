using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = GameManager.Instance.playerHealth;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == GameManager.Instance.player)
            _playerHealth.TakeDamage(_damage);
    }
}