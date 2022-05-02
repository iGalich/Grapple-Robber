using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private int _restoreValue = 1;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _delay = 0.5f;
    private float _spawnTime;

    private void Start()
    {
        _spawnTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - _spawnTime <= _delay) return;

        if ((other.CompareTag("Player") || other.CompareTag("Rope")) && !GameManager.Instance.playerHealth.IsFullHealth())
        {
            GameManager.Instance.playerHealth.AddHealth(_restoreValue);
            Destroy(gameObject);
        }
    }
}