using UnityEngine;

public class Destructible : MonoBehaviour
{
    private Rigidbody2D _body;
    [SerializeField] private BoxCollider2D _collider;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Debug.Log("test");
    }
}