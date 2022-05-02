using UnityEngine;
using UnityEngine.Events;

public class EnemyHitbox : MonoBehaviour
{
    public UnityEvent OnHit;
    public UnityEvent OnRopeHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnHit.Invoke();
        }
        else if (other.gameObject.CompareTag("Rope"))
        {
            OnRopeHit.Invoke();
        }
    }
}