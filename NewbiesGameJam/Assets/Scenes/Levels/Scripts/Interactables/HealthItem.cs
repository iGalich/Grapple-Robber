using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private int _restoreValue = 1;
    [SerializeField] private BoxCollider2D _collider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Rope")) && !GameManager.Instance.playerHealth.IsFullHealth())
        {
            GameManager.Instance.playerHealth.AddHealth(_restoreValue);
            Destroy(gameObject);
        }
    }
}